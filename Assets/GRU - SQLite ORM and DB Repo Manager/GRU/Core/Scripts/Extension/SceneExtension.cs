using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using UnityEngine.SceneManagement;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class SceneExtension
    {
        public static bool IsGRUDemo(this Scene scene)
        {
            var isDemo = scene.name == GeneralConfig.Defaults.DEMO_SCENE_NAME;
            return isDemo;
        }
    }
}
