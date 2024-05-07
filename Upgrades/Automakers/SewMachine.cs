using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShirtSim.Upgrades;

namespace TShirtSim.Upgrades.Automakers
{
    internal class SewMachine : AutoMaker
    {
        private int _rate = 1;
        private int _cost = 250;
        private int _amount = 0;
        private string _name = "Sewing Machine";


        public UpgradeTypes upgradeType => UpgradeTypes.AutoSewingMachine;

        public int? rateOfMake => _rate;

        public int cost { get => _cost; set => _cost = value; }
        public int amount { get => _amount; set => _amount = value; }
        public string Name { get => _name; set => _name = value; }

        public SewMachine()
        {
        }
        public SewMachine(int c, int a)
        {
            cost = c;
            amount = a;
        }

        public bool Purchase(PlayerInformation player)
        {
            if (player.Treasury < cost)
            {
                return false;
            }
            player.Treasury -= _cost;
            _amount++;

            /*            _cost += (int)Math.Floor((_cost * 0.05));
            */
            return true;
        }


    }
}
