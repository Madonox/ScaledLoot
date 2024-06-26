using BepInEx;

namespace ScaledLoot
{
    [BepInPlugin("Madonox.ScaledLoot", "ScaledLoot", "0.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        private static int QuotaScale = 2500; // quota scale, can be configured
        //private ConfigEntry<int> configQuota;
        //public static Config? PluginConfig { get; internal set; }

        private void Awake()
        {
            Logger.LogInfo("Initializing ScaledLoot.");
            On.DepositItemsDesk.SellItemsOnServer += SellItemsOnServer;

            //configQuota = Config.Bind("General", "QuotaScale",2500,"The quota scale value.");
            QuotaScale = Config.Bind("General", "QuotaScale", 2500, "The quota scale value.").Value;
            if (QuotaScale < 1)
            {
                QuotaScale = 1;
            }

            Logger.LogInfo("ScaledLoot has started.  Using a QuotaScale of " + QuotaScale.ToString());
        }

        private static void SellItemsOnServer(On.DepositItemsDesk.orig_SellItemsOnServer orig, DepositItemsDesk self)
        {
            TimeOfDay instance = TimeOfDay.Instance;
            if (self.IsServer && instance != null)
            {
                int profQuota = TimeOfDay.Instance.profitQuota;
                if (profQuota > QuotaScale) // make sure we don't divide because of <1 multiplicatives
                {
                    // correct scrap value multiplier
                    int delta = profQuota / QuotaScale;
                    foreach (GrabbableObject item in self.itemsOnCounter)
                    {
                        item.SetScrapValue(item.scrapValue * delta);
                    }
                }
            }

            // call original method
            orig(self);
        }
    };
}
