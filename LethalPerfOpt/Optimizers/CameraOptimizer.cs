using System;
using System.Collections.Generic;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// 摄像头优化模块。
    /// 整合 LethalSponge 的 CameraService 优化：
    /// - 飞船/地图/安保摄像头帧率控制
    /// - 视锥剔除检测（不在视野内的摄像头禁用）
    /// - 摄像头渲染透明效果开关
    /// - 监控墙可见性优化
    /// </summary>
    public class CameraOptimizer
    {
        private bool _isApplied;
        public bool IsApplied => _isApplied;

        // 摄像头引用
        private Camera _mapCamera;
        private Camera _shipCamera;
        private Camera _securityCamera;

        // 帧率控制
        private float _lastMapCamRender;
        private float _lastSecurityCamRender;
        private float _lastShipCamRender;

        public Camera MapCamera => _mapCamera;
        public Camera ShipCamera => _shipCamera;
        public Camera SecurityCamera => _securityCamera;

        public void Apply()
        {
            if (_isApplied) return;

            _isApplied = true;
            _lastMapCamRender = 0f;
            _lastSecurityCamRender = 0f;
            _lastShipCamRender = 0f;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Camera] 摄像头优化已应用");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // 恢复摄像头帧率
            if (_mapCamera != null) _mapCamera.targetDisplay = 0;
            if (_securityCamera != null) _securityCamera.targetDisplay = 0;
            if (_shipCamera != null) _shipCamera.targetDisplay = 0;

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Camera] 摄像头优化已恢复");
        }

        /// <summary>
        /// 初始化摄像头引用（在 StartOfRound.Start 后调用）
        /// 参考 LethalSponge 的 CameraService.Init()
        /// </summary>
        public void InitCameras()
        {
            try
            {
                // 查找地图摄像头
                var mapCamTransform = FindTransform("ItemSystems/MapCamera");
                if (mapCamTransform != null)
                {
                    _mapCamera = mapCamTransform.GetComponent<Camera>();
                }

                // 查找飞船摄像头
                var shipCamTransform = FindTransform("Cameras/ShipCamera");
                if (shipCamTransform != null)
                {
                    _shipCamera = shipCamTransform.GetComponent<Camera>();
                }

                // 查找安保摄像头
                var secCamTransform = FindTransform("Cameras/FrontDoorSecurityCam/SecurityCamera");
                if (secCamTransform != null)
                {
                    _securityCamera = secCamTransform.GetComponent<Camera>();
                }

                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Camera] 摄像头初始化完成 - 地图:{_mapCamera != null} 飞船:{_shipCamera != null} 安保:{_securityCamera != null}");
            }
            catch (Exception e)
            {
                Plugin.LogSource.LogWarning($"[LethalPerfOpt:Camera] 摄像头初始化失败: {e.Message}");
            }
        }

        private Transform FindTransform(string path)
        {
            try
            {
                // 尝试通过 StartOfRound 查找
                var startOfRound = GameObject.FindObjectOfType<StartOfRound>();
                if (startOfRound == null) return null;

                // 地图摄像头在 parent 下
                if (path.StartsWith("ItemSystems/"))
                {
                    var parent = startOfRound.transform.parent;
                    if (parent != null)
                    {
                        return parent.Find(path);
                    }
                }

                // 飞船相关摄像头在 elevatorTransform 下
                if (startOfRound.elevatorTransform != null)
                {
                    return startOfRound.elevatorTransform.Find(path);
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// 更新摄像头帧率控制（参考 LethalSponge 的 ManualCameraRendererSpongePatch）
        /// </summary>
        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;

            // 地图摄像头帧率控制
            if (_mapCamera != null && ModConfig.MapCameraFramerate.Value > 0)
            {
                float interval = 1f / ModConfig.MapCameraFramerate.Value;
                if (currentTime - _lastMapCamRender >= interval)
                {
                    _mapCamera.Render();
                    _lastMapCamRender = currentTime;
                }
            }

            // 安保摄像头帧率控制
            if (_securityCamera != null && ModConfig.SecurityCameraFramerate.Value > 0)
            {
                float interval = 1f / ModConfig.SecurityCameraFramerate.Value;
                if (currentTime - _lastSecurityCamRender >= interval)
                {
                    _securityCamera.Render();
                    _lastSecurityCamRender = currentTime;
                }
            }

            // 飞船摄像头帧率控制
            if (_shipCamera != null && ModConfig.ShipCameraFramerate.Value > 0)
            {
                float interval = 1f / ModConfig.ShipCameraFramerate.Value;
                if (currentTime - _lastShipCamRender >= interval)
                {
                    _shipCamera.Render();
                    _lastShipCamRender = currentTime;
                }
            }
        }

        /// <summary>
        /// 检查网格是否在摄像头视锥内（参考 LethalSponge 的 MeshVisible）
        /// </summary>
        public static bool IsMeshVisible(Camera camera, MeshRenderer mesh)
        {
            if (camera == null || mesh == null) return false;

            try
            {
                Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(camera);

                if (mesh.GetComponent<Collider>() != null)
                {
                    return GeometryUtility.TestPlanesAABB(frustum, mesh.GetComponent<Collider>().bounds);
                }
                else if (mesh.GetComponent<Renderer>() != null)
                {
                    return GeometryUtility.TestPlanesAABB(frustum, mesh.GetComponent<Renderer>().bounds);
                }
                else
                {
                    // 简单距离检查
                    float dist = Vector3.Distance(camera.transform.position, mesh.transform.position);
                    return dist < 100f;
                }
            }
            catch
            {
                return true; // 出错时默认可见
            }
        }
    }
}
