using System;
using System.Collections.Generic;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Memory and GC optimization module.
    /// Consolidates optimizations from LethalPerformance (component caching) and general GC tuning.
    /// Features: Periodic GC, component caching, memory cleanup, allocation reduction.
    /// </summary>
    public class MemoryOptimizer
    {
        private float _lastGC;
        private bool _isApplied;

        // Component cache (inspired by LethalPerformance's component caching)
        private static readonly Dictionary<(GameObject, Type), Component> _componentCache
            = new Dictionary<(GameObject, Type), Component>();
        private static int _cacheHits;
        private static int _cacheMisses;

        public void Apply()
        {
            if (_isApplied) return;

            // Force initial GC cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _lastGC = Time.realtimeSinceStartup;
            _isApplied = true;

            // Clear and reset cache
            _componentCache.Clear();
            _cacheHits = 0;
            _cacheMisses = 0;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Memory] Memory optimizations applied");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            _componentCache.Clear();
            _isApplied = false;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Memory] Memory optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;

            // Periodic GC collection
            if (currentTime - _lastGC >= ModConfig.GCInterval.Value)
            {
                PerformGC();
                _lastGC = currentTime;
            }

            // Aggressive GC mode
            if (ModConfig.AggressiveGC.Value)
            {
                // More frequent but lighter collection
                if (currentTime - _lastGC >= ModConfig.GCInterval.Value * 0.5f)
                {
                    GC.Collect(0, GCCollectionMode.Optimized, false);
                    _lastGC = currentTime;
                }
            }
        }

        private void PerformGC()
        {
            // Incremental GC to avoid frame spikes
            GC.Collect(0, GCCollectionMode.Optimized, false);

            // Clean up stale cache entries
            CleanCache();
        }

        /// <summary>
        /// Cached GetComponent - reduces GC pressure from repeated GetComponent calls.
        /// Inspired by LethalPerformance's component caching approach.
        /// </summary>
        public static T GetCachedComponent<T>(GameObject go) where T : Component
        {
            if (!ModConfig.EnableComponentCaching.Value)
            {
                return go.GetComponent<T>();
            }

            var key = (go, typeof(T));

            if (_componentCache.TryGetValue(key, out var cached))
            {
                _cacheHits++;
                if (cached != null)
                {
                    return (T)cached;
                }
            }

            _cacheMisses++;
            var component = go.GetComponent<T>();

            // Enforce cache size limit
            if (_componentCache.Count >= ModConfig.MaxCachedComponents.Value)
            {
                CleanCache();
            }

            if (component != null)
            {
                _componentCache[key] = component;
            }

            return component;
        }

        private static void CleanCache()
        {
            var keysToRemove = new List<(GameObject, Type)>();

            foreach (var kvp in _componentCache)
            {
                if (kvp.Key.Item1 == null || kvp.Value == null)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _componentCache.Remove(key);
            }
        }

        public bool IsApplied => _isApplied;

        public static int CacheHits => _cacheHits;
        public static int CacheMisses => _cacheMisses;
        public static int CacheSize => _componentCache.Count;

        public static float GetCacheHitRate()
        {
            int total = _cacheHits + _cacheMisses;
            return total > 0 ? (float)_cacheHits / total * 100f : 0f;
        }

        public long GetMemoryUsageMB()
        {
            return GC.GetTotalMemory(false) / (1024 * 1024);
        }
    }
}
