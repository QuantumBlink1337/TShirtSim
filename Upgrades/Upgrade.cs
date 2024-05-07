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
        int cost { get; set; }
        UpgradeTypes upgradeType { get; }
        int amount { get; set; }
        public bool Purchase(PlayerInformation player);
    }
}
