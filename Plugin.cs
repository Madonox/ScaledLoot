using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace ScaledLoot
{
    [BepInPlugin("Madonox.ScaledLoot","Scaled Loot","0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource? logger;
        internal static int QuotaScale = 2500; // quota scale, can be configured
        public static Config? PluginConfig;

        private void Awake()
        {
            logger = Logger;
            On.DepositItemsDesk.SellItemsOnServer += DepositItemsDesk_SellItemsOnServer;
            PluginConfig = new(Config);
            QuotaScale = PluginConfig.getConfigQuotaScale();
            logger.LogInfo("ScaledLoot started.");
        }

        private void DepositItemsDesk_SellItemsOnServer(On.DepositItemsDesk.orig_SellItemsOnServer orig, DepositItemsDesk self)
        {
            int profQuota = TimeOfDay.Instance.profitQuota;
            if (profQuota > QuotaScale)
            {
                // correct scrap value multiplier
                int delta = profQuota / QuotaScale;
                for (int i = 0; i < self.itemsOnCounter.Count; i++)
                {
                    GrabbableObject item = self.itemsOnCounter[i];
                    item.SetScrapValue((int)item.scrapValue * delta);
                }
            }


            // call original method
            orig(self);
        }
    };

    public class Config
    {
        public static ConfigEntry<int>? configQuotaScale;
        public Config(ConfigFile cfg)
        {
            configQuotaScale = cfg.Bind(
                "General",
                "QuotaScale",
                2500,
                "The amount all scrap value is multiplied by, following the equation newScrapValue = initialScrapValue * (currentQuota / QuotaScale)"
                );
        }

        public int getConfigQuotaScale()
        {
            if (configQuotaScale != null)
            {
                return configQuotaScale.Value;
            }
            return 2500; // defaults
        }
    }
}
