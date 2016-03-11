namespace Yasuo.Modules.Auto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using Yasuo.Common.Data;
    using Yasuo.Common.Classes;

    internal class Potions : Child<Modules>
    {
        public Potions(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        private List<string> dots; 

        private readonly List<PotionStruct> potions = new List<PotionStruct>
        {
            //TODO: I forgot refillable potion

            // Jungle version of Crystal Flask
            new PotionStruct("ItemCrystalFlaskJungle", (ItemId) 2032, 3, 12, 120, 0, true, new []{ PotionMode.Delayed }),

            // Dark Crystal Flask
            new PotionStruct("ItemDarkCrystalFlask", (ItemId) 2033, 3, 12, 60, 0, true, new[] { PotionMode.Delayed }),

            // Normal Health Potion
            new PotionStruct("RegenerationPotion", (ItemId) 2003, 1, 15, 150, 0, false, new[] { PotionMode.Delayed }),

            // Cookies
            new PotionStruct("ItemMiniRegenPotion", (ItemId) 2052, 1, 15, 150, 20, false, new[] { PotionMode.Delayed, PotionMode.Instant })

        }; 

        public override string Name => "Potions";

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "MinHealthPercentage", "Min Health %").SetValue(new Slider(35, 0, 99)));
            this.Menu.AddItem(new MenuItem(this.Name + "MinEnemies", "Min Enemies").SetValue(new Slider(1, 1, 5)));
            this.Menu.AddItem(new MenuItem(this.Name + "Range", "In Range Of").SetValue(new Slider(1000, 0, 4000)));
            this.Menu.AddItem(
                new MenuItem(this.Name + "AutoRefillableFirst", "Use potions that will refill first").SetValue(true)
                    .SetTooltip(
                        "If this is enabled and the assembly wants to cast a potion it will use potions that will refill at base first. Hint: It is worth enabling this, because you can sell unused potions then!"));

            this.Menu.AddItem(new MenuItem(this.Name + "AutoDOTS", "Auto Heal against DOTs").SetValue(true)
                .SetTooltip("If this is enabled and the player is for example ignited the assemblie will use a potion if you won't die from the DOT (Damage Over Time) anyway"));


            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            List<PotionStruct> availablePotions = new List<PotionStruct>();

            // Loops trough all items in the current inventory and adds all potions to the availablePotions list.
            foreach (var item in Variables.Player.InventoryItems)
            {
                if (potions.Any(x => x.ItemId == item.Id))
                {
                    availablePotions.Add(potions.FirstOrDefault(x => x.ItemId == item.Id));
                }
            }

            // No potions, our Health is near Max Health or it is Max Health, or we are recalling or teleporting, or we are in Fountain
            if (availablePotions.Count == 0 ||
                Variables.Player.Health == Variables.Player.MaxHealth - Variables.Player.HPRegenRate * 2
                || Variables.Player.InFountain() 
                || Variables.Player.Buffs.Any
                (buff => buff.Name.Contains("Recall") || buff.Name.Contains("Teleport") || buff.Name.Contains("Healing")))
            {
                return;
            }

            // Anti Damage over Time
            // TODO: Write Wrapper to access .json file containing most debuffs and dots more easily
            if (this.Menu.Item(this.Name + "AutoDOTS").GetValue<bool>())
            {
                // PSEUDO CODE
                //foreach (var debuff in SDK.SpellDatabase.Spells.Where(debuff => Variables.Player.HasBuff(debuff.AppliedBuffName)))
                //{
                //    var potion = potions.FirstOrDefault(x => x.HealValue > debuff && x.Time < debuff.Time);
                //    if (potion != null)
                //    {
                //        this.Execute(potion.ItemId);
                //    }
                //}
            }

            // Auto use on low X% Health with X enemies near
            if (Variables.Player.HealthPercent >= this.Menu.Item(this.Name + "MinHealthPercentage").GetValue<Slider>().Value 
                && Variables.Player.CountEnemiesInRange(this.Menu.Item(this.Name + "Range").GetValue<Slider>().Value) >= this.Menu.Item(this.Name + "MinEnemies").GetValue<Slider>().Value)
            {
                if (availablePotions.Any(x => x.AutoRefill) && this.Menu.Item(this.Name +"AutoRefillableFirst").GetValue<bool>())
                {
                    this.Execute(availablePotions.FirstOrDefault(x => x.AutoRefill).ItemId);
                }
                else
                {
                    this.Execute(availablePotions.MaxOrDefault(x => x.HealValue).ItemId);
                }
            }



        }

        public void OnDraw(EventArgs args)
        {

        }

        public void Execute(ItemId itemId)
        {
            InventorySlot first = null;
            foreach (var potion in this.potions)
            {
                foreach (var item in ObjectManager.Player.InventoryItems.Where(item => itemId == potion.ItemId))
                {
                    first = item;
                    break;
                }
            }
            Variables.Player.Spellbook.CastSpell(first.SpellSlot);
        }
    }

    public enum PotionMode
    {
        Instant, Delayed
    }

    public struct PotionStruct
    {
        public readonly string BuffName;

        public readonly int HealValue;

        public readonly int InstantHealValue;

        public readonly ItemId ItemId;

        public readonly int MinCharges;

        public readonly int Time;

        public readonly bool AutoRefill;

        public readonly PotionMode[] Modes;

        public PotionStruct(string buffName, ItemId itemId, int minCharges, int time, int healValue, int instanHealValue, bool autoRefill, PotionMode[] modes)
        {
            BuffName = buffName;
            ItemId = itemId;
            MinCharges = minCharges;
            Time = time;
            HealValue = healValue;
            InstantHealValue = instanHealValue;
            AutoRefill = autoRefill;
            Modes = modes;
        }
    }
}

