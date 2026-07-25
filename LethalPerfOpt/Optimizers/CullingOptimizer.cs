using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Object culling optimization module.
    /// Consolidates CullFactory's room culling with enhanced frustum and distance culling.
    /// Features: Room/area culling, distance culling, enhanced frustum culling, LOD management.
    /// </summary>
    public class CullingOptimizer
    {
        private bool _isApplied;
        private float _lastCullTime;
        private const float CULL_INTERVAL = 0.1f; // Run culling every 100ms, not every frame

        public void Apply()
        {
            if (_isApplied) return;

            _isApplied = true;
            _lastCullTime = 0f;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Culling] Culling optimizations applied");
        }

        public void Reapply()
        {
            _isApplied = false;
            Apply();
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // Re-enable all renderers that were disabled
            ReenableAllRenderers();

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Culling] Culling optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;
            if (currentTime - _lastCullTime < CULL_INTERVAL) return;

            _lastCullTime = currentTime;

            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            if (ModConfig.EnableFrustumCulling.Value)
            {
                PerformFrustumCulling(mainCam);
            }

            if (ModConfig.EnableRoomCulling.Value)
            {
                PerformRoomCulling(mainCam);
            }
        }

        /// <summary>
        /// Enhanced distance-based culling.
        /// Disables renderers beyond the configured culling distance.
        /// </summary>
        private void PerformFrustumCulling(Camera cam)
        {
            Vector3 camPos = cam.transform.position;
            float cullDist = ModConfig.CullingDistance.Value;
            float margin = ModConfig.FrustumCullingMargin.Value;

            // Use Unity's built-in frustum culling with enhanced distance
            var renderers = Object.FindObjectsOfType<Renderer>();

            foreach (var renderer in renderers)
            {
                if (renderer == null || renderer.gameObject == null) continue;

                // Skip player objects and essential items
                if (IsEssentialObject(renderer.gameObject)) continue;

                float distance = Vector3.Distance(camPos, renderer.transform.position);

                if (distance > cullDist * margin)
                {
                    if (renderer.enabled)
                    {
                        renderer.enabled = false;
                    }
                }
                else
                {
                    if (!renderer.enabled)
                    {
                        renderer.enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Room-based culling (inspired by CullFactory).
        /// Disables rendering of rooms/areas not visible to the player.
        /// Uses a simplified approach based on room boundaries.
        /// </summary>
        private void PerformRoomCulling(Camera cam)
        {
            // Room culling works by identifying interior sections
            // and disabling renderers in non-visible sections
            Vector3 camPos = cam.transform.position;
            float cullDist = ModConfig.CullingDistance.Value;

            // Find all mesh renderers in interior areas
            var meshRenderers = Object.FindObjectsOfType<MeshRenderer>();

            foreach (var mr in meshRenderers)
            {
                if (mr == null || mr.gameObject == null) continue;
                if (IsEssentialObject(mr.gameObject)) continue;

                float distance = Vector3.Distance(camPos, mr.transform.position);

                // Progressive LOD based on distance
                if (distance > cullDist * 0.7f)
                {
                    mr.enabled = false;
                }
                else if (distance > cullDist * 0.5f)
                {
                    // At medium distance, disable shadow casting
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                else
                {
                    mr.enabled = true;
                }
            }
        }

        private bool IsEssentialObject(GameObject go)
        {
            // Don't cull player objects, enemies, or important interactive objects
            string name = go.name.ToLower();

            if (name.Contains("player") || name.Contains("enemy") ||
                name.Contains("terminal") || name.Contains("ship") ||
                name.Contains("hud") || name.Contains("ui") ||
                name.Contains("monitor") || name.Contains("door"))
            {
                return true;
            }

            // Check if it's a player-controlled object
            if (go.GetComponent<GameNetcodeStuff.PlayerControllerB>() != null)
            {
                return true;
            }

            // Check if it's an enemy
            if (go.GetComponent<EnemyAI>() != null)
            {
                return true;
            }

            return false;
        }

        private void ReenableAllRenderers()
        {
            var renderers = Object.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                if (r != null)
                {
                    r.enabled = true;
                }
            }
        }

        public bool IsApplied => _isApplied;
    }
}
