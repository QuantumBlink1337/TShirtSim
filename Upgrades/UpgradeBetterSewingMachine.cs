using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShirtSim.Upgrades
{
    internal class UpgradeBetterSewingMachine : Upgrade
    {
        private string _name;
        private string _description;
        private string _filename = "UpgradeBetterSewingMachine.png";
        private int cost = 800;

        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public string IconFileName => throw new NotImplementedException();

        public int Cost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UpgradeTypes UpgradeType => throw new NotImplementedException();


        public bool Purchase(PlayerInformation player)
        {
            throw new NotImplementedException();
        }
    }
}
