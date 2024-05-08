using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShirtSim.Upgrades;
using TShirtSim.Upgrades.Automakers;

namespace TShirtSim
{
    internal class PlayerInformation
    {

        private int BasePrice = 5;
        public List<Upgrade> Upgrades {  get; }
        public Dictionary<UpgradeTypes, int> UpgradeAmounts { get; set; }
        public event EventHandler<int> TShirtMade;
        public event EventHandler<int> SewMachinePurchased;
        public event EventHandler<int> SewMachineMake;

        public double PublicInterest {  get; set; }
        public double PublicInterestScalar { get; set; }
        public double Treasury { get; set; }

        public int AmountSold { get; set; }

        public int? Material {  get; set; }
        public int MaterialPrice { get; set; }
        public int MaterialBundle { get; set; }

        public int UnsoldInventory { get; set; }
        
        public int Price {  get; set; }


        public PlayerInformation()
        {
            Upgrades = [];
            UpgradeAmounts =[];
            PublicInterest = 1.00;
            PublicInterestScalar = 0.00;
            Treasury = 0.00;
            AmountSold = 0;
            Material = 75;
            MaterialBundle = 50;
            Price = BasePrice;
            MaterialPrice = 150;
            Upgrades.Add(new SewMachine());
            Upgrades.Add(new UpgradedMaterialBundle());
            Upgrades.Add(new UpgradeAutobuyMaterial());
            Upgrades.Add(new UpgradeMarketing());
            InitializeData();

        }
        public void InitializeData()
        {
            UpgradeAmounts.Add(UpgradeTypes.AutoSewingMachine, 0);
            UpgradeAmounts.Add(UpgradeTypes.UpgradeAutobuyMaterial, 0);
            UpgradeAmounts.Add(UpgradeTypes.UpgradeMaterialBundle, 0);
            UpgradeAmounts.Add(UpgradeTypes.UpgradeMarketing, 0);
        }
        public void LoadData()
        {
            foreach (Upgrade upgrade in Upgrades)
            {
                
                upgrade.Amount = UpgradeAmounts[upgrade.UpgradeType];
                upgrade.Cost = (int)(upgrade.Cost * (upgrade.Amount * 0.05)) + upgrade.Cost;
                if (upgrade.UpgradeType == UpgradeTypes.AutoSewingMachine)
                {
                    
                }
                
            }
        }
        public void CalculateTShirt()
        {
            Random random = new Random();
            foreach (AutoMaker maker in Upgrades.FindAll(upgrade => upgrade is AutoMaker))
            {
                switch (maker.UpgradeType)
                {
                    case UpgradeTypes.AutoSewingMachine: OnRaiseSewMachineMake(100/(int)maker.rateOfMake * 5); break; 
                }
                MakeTShirt(maker.rateOfMake * maker.Amount);
            }
            MaterialPrice = random.Next(70, 120);

        }
        public bool PurchaseUpgrade(UpgradeTypes upgradeType)
        {
            Upgrade? upgrade = Upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType);
            if (upgrade == null)
            {
                return false;
            }
            if (upgrade.Purchase(this))
            {
                switch (upgrade.UpgradeType)
                {
                    case UpgradeTypes.AutoSewingMachine: OnRaiseSewMachinePurchased(1); break;
                }
                upgrade.Cost = (int)(upgrade.Cost * 0.05) + (upgrade.Cost);
                UpgradeAmounts[upgrade.UpgradeType] = upgrade.Amount;
                return true;
            }
            return false;
        }

        public bool MakeTShirt(int? amount)
        {
            if (amount <= 0 || amount == null)
            {
                return false;
            }
            if (Material >= amount)
            {
                UnsoldInventory += (int) amount;
                Material -= amount;
                OnRaiseTShirtMake((int)amount);
                return true;
            }
            if (amount >= Material && Material != 0)
            {
                var temp = (int)Material;
                UnsoldInventory += (int)Material;
                Material = 0;
                OnRaiseTShirtMake(temp);
                return true;
            }
            return false;
           
        }
        protected virtual void OnRaiseTShirtMake(int e)
        {
            EventHandler<int> raiseEvent = TShirtMade;
            if (raiseEvent != null )
            {
                var milliseconds = (1000 / e);
                raiseEvent(this, milliseconds);
            }
        }
        protected virtual void OnRaiseSewMachinePurchased(int e)
        {
            EventHandler<int> raiseEvent = SewMachinePurchased;
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }
        protected virtual void OnRaiseSewMachineMake(int e)
        {
            EventHandler<int> raiseEvent = SewMachineMake;
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }
        public void AutoPurchaseMaterial()
        {
            if (Material == 0)
            {
                PurchaseMaterial();
            }
        }

        public void PurchaseMaterial()
        {
            if (Treasury >= MaterialPrice)
            {
                Material += MaterialBundle;
                Treasury -= MaterialPrice;
            }
            
        }
        public void DoSales()
        {
            Random random = new Random();
            int functionValue = (int)Math.Floor(Math.Pow(10, (1 / PublicInterest))+(3/(2*PublicInterest)))+10;
            if (functionValue < 0)
            {
                functionValue = 1000;
            }
            int selectedValue = random.Next(1,Math.Max(functionValue, 1));
            if (selectedValue == 1)
            {
                int sold = Math.Min(random.Next(1+(AmountSold/50), 10+(AmountSold/50)), UnsoldInventory);
                UnsoldInventory -= sold;
                Treasury += sold * Price;
                AmountSold += sold;
            }
            

        }
        public void CalculateInterest()
        {
            int delta = BasePrice - Price;
            PublicInterest = Math.Pow(1.33, delta) + PublicInterestScalar;
        }
        
     
       
       

    }
}
