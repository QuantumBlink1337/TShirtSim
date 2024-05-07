using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShirtSim.Upgrades
{
    internal class UpgradeAutobuyMaterial : Upgrade
    {
        private int _cost = 600;

        private int _amount = 0;
        private string _name = "Auto Purchase Materials";
        private string _description =



        public int cost { get => _cost; set { _cost = value; } }

        public UpgradeTypes upgradeType => UpgradeTypes.UpgradeAutobuyMaterial;

        public int amount { get { return _amount; } set { _amount = value; } }

        public string Name { get => _name; }
        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Purchase(PlayerInformation player)
        {
            if (player.Treasury < cost)
            {
                return false;
            }
            player.Treasury -= cost;
            return true;
        }
    }
}
