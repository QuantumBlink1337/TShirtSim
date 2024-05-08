using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShirtSim
{
    public struct UnlockPurchase
    {
        public UnlockPurchase(bool unlock, bool purchase)
        {
            Unlocked = unlock;
            Purchase = purchase;
        }
        public bool Unlocked { get; set;}
        public bool Purchase { get; set;}
    }
}
