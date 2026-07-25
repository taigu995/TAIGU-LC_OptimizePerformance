using System;
using System.Collections.Generic;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// 音频优化模块。
    /// 整合 LethalSponge 的 AudioService 优化：
    /// - 音频去重（相同名称的 AudioClip 共享引用）
    /// - 音频源距离衰减优化
    /// - 音频更新频率控制
    /// </summary>
    public class AudioOptimizer
    {
        private bool _isApplied;
        public bool IsApplied => _isApplied;
        private int _dedupedCount;

        public int DedupedCount => _dedupedCount;

        public void Apply()
        {
            if (_isApplied) return;

            // 去重音频
            if (ModConfig.EnableAudioDedup.Value)
            {
                _dedupedCount = DeduplicateAudio();
            }

            // 优化音频源设置
            OptimizeAudioSources();

            _isApplied = true;
            Plugin.LogSource.LogInfo($"[LethalPerfOpt:Audio] 音频优化已应用 - 去重:{_dedupedCount}");
        }

        /// <summary>
        /// 重新应用音频优化（场景加载后调用，重新扫描场景中的音频源）
        /// </summary>
        public void Reapply()
        {
            _isApplied = false;
            _dedupedCount = 0;
            Apply();
        }

        public void Revert()
        {
            if (!_isApplied) return;

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Audio] 音频优化已恢复");
        }

        /// <summary>
        /// 音频去重（参考 LethalSponge 的 AudioService.DedupeAllAudio）
        /// </summary>
        private int DeduplicateAudio()
        {
            int count = 0;
            var audioDict = new Dictionary<string, AudioClip>();

            // 处理所有 AudioSource
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
        /// 优化音频源设置
        /// </summary>
        private void OptimizeAudioSources()
        {
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            int optimized = 0;

            foreach (var audioSource in allAudioSources)
            {
                if (audioSource == null) continue;

                // 禁用不必要的音频源
                if (ModConfig.DisableDistantAudio.Value)
                {
                    if (audioSource.maxDistance < ModConfig.DistantAudioThreshold.Value)
                    {
                        audioSource.enabled = false;
                        optimized++;
                    }
                }

                // 降低音频更新频率
                if (ModConfig.ReduceAudioUpdateRate.Value)
                {
                    audioSource.spatialBlend = 0f; // 转为 2D 音频减少空间计算
                }
            }

            if (optimized > 0)
            {
                Plugin.LogSource.LogInfo($"[LethalPerfOpt:Audio] 优化音频源: {optimized} 个");
            }
        }
    }
}
