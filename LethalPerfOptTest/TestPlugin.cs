using BepInEx;
using BepInEx.Logging;

namespace TAIGU_TestPlugin
{
    [BepInPlugin("TAIGU.LC_TestPlugin", "TAIGU-LC_TestPlugin", "1.0.0")]
    public class TestPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("TAIGU 测试插件已加载！");
        }
    }
}