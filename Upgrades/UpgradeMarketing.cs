﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TShirtSim.Upgrades
{
    internal class UpgradeMarketing : Upgrade
    {
        private int _cost = 700;
        private int _amount = 0;
        private string _name = "Marketing 1";
        public int cost { get => _cost; set => _cost = value; }

        public UpgradeTypes upgradeType => UpgradeTypes.UpgradeMarketing;

        public int amount { get { return _amount; } set { _amount = value; } }

        public string Name { get => _name; set => _name = value; }

        public bool Purchase(PlayerInformation player)
        {
            if (player.Treasury < cost)
            {
                return false;
            }
            player.Treasury -= cost;
            player.PublicInterestScalar += 0.25;
            _name = $"Marketing {amount + 2}";
            cost *= 2;
            return true;
        }
    }
}
