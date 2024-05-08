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
        private string _description = $"Produces t-shirts automatically";
        private string _filename = "SewingMachine1.png";
        private string _filename2 = "SewingMachine2.png";


        public UpgradeTypes UpgradeType => UpgradeTypes.AutoSewingMachine;

        public int? RateOfMake => _rate;

        public int Cost { get => _cost; set => _cost = value; }
        public int Amount { get => _amount; set => _amount = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; }
        public string IconFileName { get => _filename; }
        public string IconFileName2 { get => _filename2; }


        public SewMachine()
        {
        }
        public SewMachine(int c, int a)
        {
            Cost = c;
            Amount = a;
        }

        public bool Purchase(PlayerInformation player)
        {
            if (player.Treasury < Cost)
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
