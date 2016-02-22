//TODO:  Djikstra

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
            new PotionStruct("ItemCrystalFlaskJungle", (ItemId) 2032, 3, 12, 120, 0, new []{ PotionMode.Delayed }),

            // Dark Crystal Flask
            new PotionStruct("ItemDarkCrystalFlask", (ItemId) 2033, 3, 12, 60, 0, new[] { PotionMode.Delayed }),

            // Normal Health Potion
            new PotionStruct("RegenerationPotion", (ItemId) 2003, 1, 15, 150, 0, new[] { PotionMode.Delayed }),

            // Cookies
            new PotionStruct("ItemMiniRegenPotion", (ItemId) 2052, 1, 15, 150, 20, new[] { PotionMode.Delayed, PotionMode.Instant })

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
            if (Variables.Player.Health >= Variables.Player.MaxHealth - Variables.Player.HPRegenRate * 2
                || !Items.HasItem(2003, Variables.Player)
                || Variables.Player.InFountain() 
                || Variables.Player.Buffs.Any
                (buff => buff.Name.Contains("Recall") || buff.Name.Contains("Teleport") || buff.Name.Contains("Healing")))
            {
                return;
            }

            if (this.Menu.Item(this.Name + "AutoDOTS").GetValue<bool>())
            {
                foreach (var debuff in SDK.SpellDatabase.Spells.Where(debuff => Variables.Player.HasBuff(debuff.AppliedBuffName)))
                {
                    var potion = potions.FirstOrDefault(x => x.HealValue > debuff && x.Time < debuff.Time);
                    if (potion != null)
                    {
                        this.Execute(potion.ItemId);
                    }
                }
            }

            if (Variables.Player.HealthPercent >= this.Menu.Item(this.Name + "MinHealthPercentage").GetValue<Slider>().Value 
                && Variables.Player.CountEnemiesInRange(this.Menu.Item(this.Name + "Range").GetValue<Slider>().Value) >= this.Menu.Item(this.Name + "MinEnemies").GetValue<Slider>().Value)
            {
                this.Execute();
            }



        }

        public void OnDraw(EventArgs args)
        {

        }

        public void Execute(int itemId)
        {
            Items.UseItem(itemId);
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

        public readonly PotionMode[] Modes;

        public PotionStruct(string buffName, ItemId itemId, int minCharges, int time, int healValue, int instanHealValue, PotionMode[] modes)
        {
            BuffName = buffName;
            ItemId = itemId;
            MinCharges = minCharges;
            Time = time;
            HealValue = healValue;
            InstantHealValue = instanHealValue;
            Modes = modes;
        }
    }
}

