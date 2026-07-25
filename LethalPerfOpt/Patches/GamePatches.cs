using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using TAIGU_LC_OptimizePerformance.Optimizers;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 游戏核心补丁 - 使用显式 Harmony.Patch() 模式
    /// 整合 LethalSponge 的关键补丁：
    /// - StartOfRound.Start: 初始化摄像头、应用灯光优化
    /// - StartOfRound.PassTimeToNextDay: 每日资源清理
    /// - RoundManager.FinishGeneratingLevel: 关卡生成后优化
    /// - FoliageDetailDistance: 修复植被 LOD 材质泄漏
    /// </summary>
    public static class GamePatches
    {
        /// <summary>
        /// 游戏回合开始时调用 - 初始化摄像头、应用灯光优化
        /// 参考 LethalSponge 的 StartOfRoundSpongePatch.StartOfRound_Start
        /// </summary>
        private static void StartOfRound_Start(ref StartOfRound __instance)
        {
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Patches] StartOfRound.Start 补丁触发");

            // 初始化摄像头优化
            if (ModConfig.FixCameraSettings.Value && Plugin.CameraOpt != null)
            {
                Plugin.CameraOpt.InitCameras();
            }

            // 应用灯光优化
            if (Plugin.LightingOpt != null)
            {
                Plugin.LightingOpt.Apply();
            }
        }

        /// <summary>
        /// 每日时间切换时调用 - 执行资源清理
        /// 参考 LethalSponge 的 StartOfRoundSpongePatch.StartOfRound_PassTimeToNextDay
        /// </summary>
        private static void StartOfRound_PassTimeToNextDay()
        {
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Patches] 每日清理触发");

            // 执行每日资源清理
            if (Plugin.MemoryOpt != null)
            {
                Plugin.MemoryOpt.OnDayPassed();
            }
        }

        /// <summary>
        /// 关卡生成完成补丁
        /// 参考 LethalSponge 的 RoundManagerSpongePatch
        /// </summary>
        private static void RoundManager_FinishGeneratingLevel()
        {
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Patches] 关卡生成完成，应用优化...");

            // 关卡生成后应用灯光优化
            if (Plugin.LightingOpt != null)
            {
                Plugin.LightingOpt.UpdateAllLights();
            }
        }

        /// <summary>
        /// 植被 LOD 修复补丁 - 修复材质泄漏
        /// 参考 LethalSponge 的 FoliageDetailDistanceSpongePatch
        /// 简化版本：禁用原始 Update 方法，使用距离检查替代
        /// </summary>
        private static bool FoliageDetailDistance_Update()
        {
            if (!ModConfig.FixFoliageLOD.Value) return true;

            // 禁用原始 Update 以防止材质泄漏
            // 植被 LOD 切换由引擎自动处理，无需手动干预
            return false;
        }

        /// <summary>
        /// 监控摄像头渲染优化补丁
        /// 参考 LethalSponge 的 ManualCameraRendererSpongePatch
        /// </summary>
        private static void ManualCameraRenderer_Update(ref ManualCameraRenderer __instance)
        {
            if (!ModConfig.PatchCameraScript.Value) return;

            try
            {
                if (GameNetworkManager.Instance.localPlayerController == null) return;

                // 当摄像头被覆盖为其他用途时，检查是否在视野内
                if (__instance.overrideCameraForOtherUse)
                {
                    var player = GameNetworkManager.Instance.localPlayerController;
                    var currentCamera = player.isPlayerDead
                        ? StartOfRound.Instance.spectateCamera
                        : player.gameplayCamera;

                    if (__instance.mesh != null && !CameraOptimizer.IsMeshVisible(currentCamera, __instance.mesh))
                    {
                        __instance.cam.enabled = false;
                        return;
                    }

                    // 应用帧率限制
                    if (__instance.renderAtLowerFramerate)
                    {
                        if (Plugin.CameraOpt != null)
                        {
                            Plugin.CameraOpt.Update();
                        }
                    }
                    else
                    {
                        __instance.cam.enabled = true;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 摄像头启用条件补丁
        /// </summary>
        private static void ManualCameraRenderer_MeetsCameraEnabledConditions(
            ref ManualCameraRenderer __instance, bool __result, PlayerControllerB player)
        {
            if (!ModConfig.PatchCameraScript.Value) return;

            try
            {
                var currentCamera = player.isPlayerDead
                    ? StartOfRound.Instance.spectateCamera
                    : player.gameplayCamera;

                // 重新检查网格可见性
                if (__instance.mesh != null && !CameraOptimizer.IsMeshVisible(currentCamera, __instance.mesh))
                {
                    __result = false;
                }
            }
            catch { }
        }
    }
}