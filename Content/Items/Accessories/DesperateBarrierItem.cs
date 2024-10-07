using lenen.Common;
using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class DesperateBarrierItem : ModItem
    {
        private float dmgReduction = 0.1f;
        private Barrier barrier = BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.DesperateBarrier];
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.accessory = true;

            Item.rare = ItemRarityID.Red;
            Item.defense = 10;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            barrier.MaxLife, barrier.MaxCooldown, barrier.MaxRecovery,
            dmgReduction* 100);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SpellCardManagement management = player.GetModPlayer<SpellCardManagement>();
            barrier.State = 1;
            /*
            if (!barrierManager.barriers.Contains(barrier))
            {
                Main.NewText("Does not have Desperate barrier, adding one");
                barrierManager.AddBarrier(barrier);
            } else
            {
                Main.NewText("Does have Desperate barrier, doing nothing");
            }
            int barrierIndex = barrierManager.barriers.IndexOf(barrier);
            barrierManager.barriers[barrierIndex].State = 1;*/
            //barrierManager.GetBarrier(typeof(DesperateBarrier)).State = 1;

            if (player.statLife <= (player.statLifeMax2 / 2))
            {
                barrier.State = 2;
                //barrierManager.barriers[barrierIndex].State = 2;
                //barrierManager.GetBarrier(typeof(DesperateBarrier)).State = 2;
                management.desperateBomb = barrier.IsAvailable();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddIngredient(ItemID.FragmentVortex, 5)
                .AddIngredient(ItemID.PaladinsShield, 1)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient<DesperateBarrier>()
                .Register();
        }
    }
}
