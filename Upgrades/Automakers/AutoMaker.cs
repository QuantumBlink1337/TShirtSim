using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShirtSim.Upgrades;

namespace TShirtSim.Upgrades.Automakers
{
    internal interface AutoMaker : Upgrade
    {
        int? rateOfMake { get; }

    }
}
