using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Physics optimization module.
    /// Features: Timestep tuning, solver iterations, sleeping body optimization, collision layer tuning.
    /// </summary>
    public class PhysicsOptimizer
    {
        private float _originalFixedTimeStep;
        private int _originalSolverIterations;
        private float _originalSleepThreshold;
        private bool _isApplied;

        public void Apply()
        {
            if (_isApplied) return;

            // Save original values
            _originalFixedTimeStep = Time.fixedDeltaTime;
            _originalSolverIterations = Physics.defaultSolverIterations;
            _originalSleepThreshold = Physics.sleepThreshold;

            // Apply physics timestep (higher = less CPU but less accurate)
            Time.fixedDeltaTime = ModConfig.PhysicsTimeStep.Value;

            // Reduce solver iterations for performance
            Physics.defaultSolverIterations = ModConfig.MaxPhysicsIterations.Value;

            // Adjust sleep threshold
            Physics.sleepThreshold = ModConfig.SleepThreshold.Value;

            // Reduce default contact offset to reduce collision checks
            Physics.defaultContactOffset = 0.01f;

            // Disable auto-sync transforms if not needed
            Physics.autoSyncTransforms = false;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Physics] Physics optimizations applied");
        }

        public void Reapply()
        {
            _isApplied = false;
            Apply();
        }

        public void Revert()
        {
            if (!_isApplied) return;

            Time.fixedDeltaTime = _originalFixedTimeStep;
            Physics.defaultSolverIterations = _originalSolverIterations;
            Physics.sleepThreshold = _originalSleepThreshold;
            Physics.autoSyncTransforms = true;

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Physics] Physics optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            // Dynamic physics optimization: reduce updates for distant objects
            if (ModConfig.DisableSleepingBodies.Value)
            {
                OptimizeSleepingBodies();
            }
        }

        private void OptimizeSleepingBodies()
        {
            // Find all rigidbodies and optimize sleeping ones
            var rigidbodies = Object.FindObjectsOfType<Rigidbody>();
            Camera mainCam = Camera.main;

            if (mainCam == null) return;

            Vector3 camPos = mainCam.transform.position;

            foreach (var rb in rigidbodies)
            {
                if (rb == null || rb.gameObject == null) continue;

                float distance = Vector3.Distance(camPos, rb.transform.position);

                // For distant objects, force sleep and increase drag
                if (distance > ModConfig.CullingDistance.Value * 0.8f)
                {
                    if (!rb.IsSleeping() && rb.velocity.sqrMagnitude < 0.01f)
                    {
                        rb.Sleep();
                    }
                }
            }
        }

        public bool IsApplied => _isApplied;
    }
}
