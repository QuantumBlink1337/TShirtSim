using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShirtSim.Upgrades
{
    internal class UpgradedMaterialBundle : Upgrade
    {
        private int _cost = 450;
        private int _amount = 0;
        private string _name = "Upgraded Material Bundle";
        private string _description = "50% larger Material bundles";
        private string _filename = "LargerMaterialBundle1.png";



        public int Cost { get { return _cost; } set { _cost = value; } }

        public UpgradeTypes upgradeType => UpgradeTypes.UpgradeMaterialBundle;

        public int amount { get { return _amount; } set => _amount = value; }

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
            player.MaterialBundle += (int)(player.MaterialBundle * .5);
            return true;
        }
    }
}
