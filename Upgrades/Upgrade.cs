using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShirtSim.Upgrades
{
    internal interface Upgrade
    {
        string Name { get; }
        string Description { get; }
        string IconFileName { get; }
        int Cost { get; set; }
        UpgradeTypes UpgradeType { get; }
        int Amount { get; set; }
        public bool Purchase(PlayerInformation player);
    }
}
