﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TShirtSim.Upgrades
{
    internal class UpgradeAutobuyMaterial : Upgrade
    {
        private int _cost = 600;

        private int _amount = 0;
        private string _name = "Auto Purchase Materials";
        private string _description = "Purchases Materials automatically when you run out";
        private string _filename = "AutobuyMaterial.png";



        public int Cost { get => _cost; set { _cost = value; } }

        public UpgradeTypes UpgradeType => UpgradeTypes.UpgradeAutobuyMaterial;

        public int Amount { get { return _amount; } set { _amount = value; } }

        public string Name { get => _name; }
        public string Description { get => _description; }
        public string IconFileName { get => _filename; }    

        public bool Purchase(PlayerInformation player)
        {
            if (player.Treasury < Cost)
            {
                return false;
            }
            player.Treasury -= Cost;
            return true;
        }
    }
}
