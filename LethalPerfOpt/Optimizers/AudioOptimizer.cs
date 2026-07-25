using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Audio optimization module.
    /// Features: Distance-based audio culling, max audio source limiting, reverb control.
    /// </summary>
    public class AudioOptimizer
    {
        private bool _isApplied;
        private float _lastUpdateTime;
        private const float UPDATE_INTERVAL = 0.25f;

        public void Apply()
        {
            if (_isApplied) return;

            _isApplied = true;
            _lastUpdateTime = 0f;

            // Disable reverb if configured
            if (ModConfig.DisableReverb.Value)
            {
                DisableReverb();
            }

            // Set audio listener settings
            AudioListener.pause = false;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Audio] Audio optimizations applied");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // Re-enable all audio sources
            EnableAllAudio();

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Audio] Audio optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;
            if (currentTime - _lastUpdateTime < UPDATE_INTERVAL) return;

            _lastUpdateTime = currentTime;

            // Perform audio culling
            PerformAudioCulling();
        }

        private void PerformAudioCulling()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Vector3 camPos = mainCam.transform.position;
            float cullDist = ModConfig.AudioCullingDistance.Value;
            int maxSources = ModConfig.MaxAudioSources.Value;

            var audioSources = Object.FindObjectsOfType<AudioSource>();
            int activeCount = 0;

            // Sort by distance
            System.Array.Sort(audioSources, (a, b) =>
            {
                if (a == null || b == null) return 0;
                float distA = Vector3.Distance(camPos, a.transform.position);
                float distB = Vector3.Distance(camPos, b.transform.position);
                return distA.CompareTo(distB);
            });

            foreach (var source in audioSources)
            {
                if (source == null || source.gameObject == null) continue;

                // Always keep player audio active
                if (source.gameObject.GetComponent<GameNetcodeStuff.PlayerControllerB>() != null)
                {
                    source.priority = 0; // Highest priority
                    continue;
                }

                float distance = Vector3.Distance(camPos, source.transform.position);

                if (distance > cullDist || activeCount >= maxSources)
                {
                    if (source.isPlaying)
                    {
                        source.Pause();
                    }
                }
                else
                {
                    activeCount++;

                    // Adjust volume based on distance
                    float normalizedDist = distance / cullDist;
                    source.volume = Mathf.Clamp01(1.0f - normalizedDist * 0.5f);
                }
            }
        }

        private void DisableReverb()
        {
            AudioReverbZone[] reverbZones = Object.FindObjectsOfType<AudioReverbZone>();
            foreach (var zone in reverbZones)
            {
                if (zone != null)
                {
                    zone.enabled = false;
                }
            }

            AudioListener.volume = 1.0f;
        }

        private void EnableAllAudio()
        {
            var audioSources = Object.FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                if (source != null && !source.isPlaying && source.gameObject.activeSelf)
                {
                    source.UnPause();
                }
            }

            // Re-enable reverb zones
            AudioReverbZone[] reverbZones = Object.FindObjectsOfType<AudioReverbZone>();
            foreach (var zone in reverbZones)
            {
                if (zone != null)
                {
                    zone.enabled = true;
                }
            }
        }

        public bool IsApplied => _isApplied;
    }
}
