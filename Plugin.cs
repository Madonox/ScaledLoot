using BepInEx;
using BepInEx.Configuration;

namespace ScaledLoot
{
    [BepInPlugin("Madonox.ScaledLoot", "ScaledLoot", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private static int QuotaScale = 2500; // quota scale, can be configured
        //private ConfigEntry<int> configQuota;
        //public static Config? PluginConfig { get; internal set; }

        private void Awake()
        {
            Logger.LogInfo("Initializing ScaledLoot.");
            On.DepositItemsDesk.SellItemsOnServer += DepositItemsDesk_SellItemsOnServer;

            //configQuota = Config.Bind("General", "QuotaScale",2500,"The quota scale value.");
            QuotaScale = Config.Bind("General", "QuotaScale", 2500, "The quota scale value.").Value;

            Logger.LogInfo("ScaledLoot has started.  Using a QuotaScale of " + QuotaScale.ToString());
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
}
