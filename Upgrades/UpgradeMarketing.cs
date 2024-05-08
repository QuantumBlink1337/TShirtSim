using System;
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
        private string _description = "25% more public interest for your T-Shirts";
        private string _filename = "Marketing.png";
        public int Cost { get => _cost; set => _cost = value; }

        public UpgradeTypes UpgradeType => UpgradeTypes.UpgradeMarketing;

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
            player.PublicInterestScalar += 0.25;
            _name = $"Marketing {Amount + 2}";
            Cost *= 2;
            return true;
        }
    }
}
