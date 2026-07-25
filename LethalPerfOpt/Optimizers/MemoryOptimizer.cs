using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// 内存与泄漏清理优化模块。
    /// 整合 LethalSponge 的内存泄漏清理机制：
    /// - 每日自动 UnloadUnusedAssets
    /// - 资产包泄漏检测与清理
    /// - 重复网格/纹理/音频去重
    /// - 组件缓存减少 GetComponent 分配
    /// </summary>
    public class MemoryOptimizer
    {
        private float _lastGC;
        private bool _isApplied;
        private int _initialObjectCount;
        private int _lastCleanedCount;

        public bool IsApplied => _isApplied;

        // 组件缓存（参考 LethalPerformance 的组件缓存机制）
        private static readonly Dictionary<(GameObject, Type), Component> _componentCache
            = new Dictionary<(GameObject, Type), Component>();

        // 缓存统计
        private static int _cacheHits;
        private static int _cacheMisses;

        public static int CacheHits => _cacheHits;
        public static int CacheMisses => _cacheMisses;
        public static int CacheSize => _componentCache.Count;

        public static float GetCacheHitRate()
        {
            int total = _cacheHits + _cacheMisses;
            return total > 0 ? (_cacheHits * 100f / total) : 0f;
        }

        public float GetMemoryUsageMB()
        {
            return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        }

        // 去重统计
        private int _dedupedMeshes;
        private int _dedupedTextures;
        private int _dedupedAudio;

        public int DedupedMeshes => _dedupedMeshes;
        public int DedupedTextures => _dedupedTextures;
        public int DedupedAudio => _dedupedAudio;
        public int InitialObjectCount => _initialObjectCount;
        public int LastCleanedCount => _lastCleanedCount;

        public void Apply()
        {
            if (_isApplied) return;

            // 记录初始对象数量
            try
            {
                var allObjects = Resources.FindObjectsOfTypeAll<UnityEngine.Object>();
                _initialObjectCount = allObjects.Length;
                allObjects = null;
            }
            catch (Exception e)
            {
                Plugin.LogSource.LogWarning($"[LethalPerfOpt:Memory] 无法获取初始对象数量: {e.Message}");
            }

            // 强制初始 GC 清理
            PerformFullGC();

            _lastGC = Time.realtimeSinceStartup;
            _isApplied = true;

            _componentCache.Clear();
            _dedupedMeshes = 0;
            _dedupedTextures = 0;
            _dedupedAudio = 0;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Memory] 内存优化已应用");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            _componentCache.Clear();
            _isApplied = false;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Memory] 内存优化已恢复");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;

            // 定期 GC 回收
            if (currentTime - _lastGC >= ModConfig.GCInterval.Value)
            {
                PerformGC();
                _lastGC = currentTime;
            }
        }

        /// <summary>
        /// 每日清理 - 在日期切换时调用（参考 LethalSponge 的 PassTimeToNextDay 补丁）
        /// </summary>
        public void OnDayPassed()
        {
            if (!_isApplied) return;

            if (ModConfig.EnableDailyCleanup.Value)
            {
                Plugin.LogSource.LogInfo("[LethalPerfOpt:Memory] 执行每日资源清理...");
                PerformDailyCleanup();
            }
        }

        /// <summary>
        /// 执行每日资源清理（参考 LethalSponge 的 SpongeService.ApplySponge）
        /// </summary>
        private void PerformDailyCleanup()
        {
            // 1. 去重网格
            if (ModConfig.EnableMeshDedup.Value)
            {
                _dedupedMeshes = DeduplicateMeshes();
                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Memory] 去重网格: {_dedupedMeshes} 个");
            }

            // 2. 去重纹理
            if (ModConfig.EnableTextureDedup.Value)
            {
                _dedupedTextures = DeduplicateTextures();
                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Memory] 去重纹理: {_dedupedTextures} 个");
            }

            // 3. 去重音频
            if (ModConfig.EnableAudioDedup.Value)
            {
                _dedupedAudio = DeduplicateAudio();
                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Memory] 去重音频: {_dedupedAudio} 个");
            }

            // 4. 缩放纹理
            if (ModConfig.EnableTextureResize.Value)
            {
                ResizeTextures();
            }

            // 5. 卸载未使用资源
            PerformFullGC();

            // 记录清理后对象数量
            try
            {
                var allObjects = Resources.FindObjectsOfTypeAll<UnityEngine.Object>();
                _lastCleanedCount = allObjects.Length;
                allObjects = null;
            }
            catch { }
        }

        /// <summary>
        /// 去重网格（参考 LethalSponge 的 MeshService.DedupeAllMeshes）
        /// </summary>
        private int DeduplicateMeshes()
        {
            int count = 0;
            var meshDict = new Dictionary<string, Mesh>();
            var allMeshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();

            foreach (var mf in allMeshFilters)
            {
                if (mf == null || mf.sharedMesh == null) continue;

                string meshName = mf.sharedMesh.name;
                if (meshDict.TryGetValue(meshName, out Mesh original))
                {
                    if (mf.sharedMesh != original)
                    {
                        mf.sharedMesh = original;
                        count++;
                    }
                }
                else
                {
                    meshDict[meshName] = mf.sharedMesh;
                }
            }

            // 也处理 SkinnedMeshRenderer
            var allSkinned = Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>();
            foreach (var smr in allSkinned)
            {
                if (smr == null || smr.sharedMesh == null) continue;

                string meshName = smr.sharedMesh.name;
                if (meshDict.TryGetValue(meshName, out Mesh original))
                {
                    if (smr.sharedMesh != original)
                    {
                        smr.sharedMesh = original;
                        count++;
                    }
                }
                else
                {
                    meshDict[meshName] = smr.sharedMesh;
                }
            }

            return count;
        }

        /// <summary>
        /// 去重纹理（参考 LethalSponge 的 TextureService）
        /// </summary>
        private int DeduplicateTextures()
        {
            int count = 0;
            var textureDict = new Dictionary<string, Texture2D>();
            var allRenderers = Resources.FindObjectsOfTypeAll<Renderer>();

            foreach (var renderer in allRenderers)
            {
                if (renderer == null) continue;

                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;

                    var shader = materials[i].shader;
                    if (shader == null) continue;

                    int propCount = shader.GetPropertyCount();
                    for (int p = 0; p < propCount; p++)
                    {
                        if (shader.GetPropertyType(p) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                        {
                            try
                            {
                                var tex = materials[i].GetTexture(shader.GetPropertyNameId(p)) as Texture2D;
                                if (tex == null) continue;

                                string texName = tex.name;
                                if (textureDict.TryGetValue(texName, out Texture2D original))
                                {
                                    if (tex != original)
                                    {
                                        materials[i].SetTexture(shader.GetPropertyNameId(p), original);
                                        count++;
                                    }
                                }
                                else
                                {
                                    textureDict[texName] = tex;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 去重音频（参考 LethalSponge 的 AudioService.DedupeAllAudio）
        /// </summary>
        private int DeduplicateAudio()
        {
            int count = 0;
            var audioDict = new Dictionary<string, AudioClip>();
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();

            foreach (var audioSource in allAudioSources)
            {
                if (audioSource == null || audioSource.clip == null) continue;

                string clipName = audioSource.clip.name;
                if (audioDict.TryGetValue(clipName, out AudioClip original))
                {
                    if (audioSource.clip != original)
                    {
                        audioSource.clip = original;
                        count++;
                    }
                }
                else
                {
                    audioDict[clipName] = audioSource.clip;
                }
            }

            return count;
        }

        /// <summary>
        /// 缩放纹理（参考 LethalSponge 的 TextureService.ResizeAllTextures）
        /// </summary>
        private void ResizeTextures()
        {
            int maxSize = ModConfig.MaxTextureSize.Value;
            var allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            int resized = 0;

            foreach (var texture in allTextures)
            {
                if (texture == null) continue;
                if (texture.width <= maxSize && texture.height <= maxSize) continue;
                if (!texture.isReadable) continue;

                try
                {
                    // 计算缩放比例
                    float scale = (float)maxSize / Mathf.Max(texture.width, texture.height);
                    int newWidth = Mathf.Max(1, Mathf.RoundToInt(texture.width * scale));
                    int newHeight = Mathf.Max(1, Mathf.RoundToInt(texture.height * scale));

                    // 缩放纹理
                    TextureScale.Bilinear(texture, newWidth, newHeight);
                    resized++;
                }
                catch { }
            }

            if (resized > 0)
            {
                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Memory] 缩放纹理: {resized} 个 (最大尺寸: {maxSize})");
            }
        }

        private void PerformGC()
        {
            GC.Collect(2, GCCollectionMode.Optimized, false);
        }

        private void PerformFullGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 组件缓存 - 减少 GetComponent 调用分配
        /// </summary>
        public static T GetCachedComponent<T>(GameObject go) where T : Component
        {
            var key = (go, typeof(T));
            if (_componentCache.TryGetValue(key, out Component cached))
            {
                return cached as T;
            }

            var component = go.GetComponent<T>();
            if (component != null)
            {
                _componentCache[key] = component;
            }
            return component;
        }

        public long GetCurrentMemoryMB()
        {
            return GC.GetTotalMemory(false) / (1024 * 1024);
        }
    }

    /// <summary>
    /// 纹理缩放工具类（双线性插值）
    /// </summary>
    public static class TextureScale
    {
        public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
        {
            if (tex == null || !tex.isReadable) return;
            if (newWidth <= 0 || newHeight <= 0) return;

            Color[] srcColors = tex.GetPixels();
            int srcWidth = tex.width;
            int srcHeight = tex.height;

            Color[] dstColors = new Color[newWidth * newHeight];

            float ratioX = (float)srcWidth / newWidth;
            float ratioY = (float)srcHeight / newHeight;

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    float gx = x * ratioX;
                    float gy = y * ratioY;

                    int srcX = (int)gx;
                    int srcY = (int)gy;

                    float fx = gx - srcX;
                    float fy = gy - srcY;

                    int x1 = Mathf.Min(srcX, srcWidth - 1);
                    int x2 = Mathf.Min(srcX + 1, srcWidth - 1);
                    int y1 = Mathf.Min(srcY, srcHeight - 1);
                    int y2 = Mathf.Min(srcY + 1, srcHeight - 1);

                    Color c1 = srcColors[y1 * srcWidth + x1];
                    Color c2 = srcColors[y1 * srcWidth + x2];
                    Color c3 = srcColors[y2 * srcWidth + x1];
                    Color c4 = srcColors[y2 * srcWidth + x2];

                    dstColors[y * newWidth + x] = Color.Lerp(
                        Color.Lerp(c1, c2, fx),
                        Color.Lerp(c3, c4, fx),
                        fy
                    );
                }
            }

            tex.Resize(newWidth, newHeight);
            tex.SetPixels(dstColors);
            tex.Apply();
        }
    }
}
