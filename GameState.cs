using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Text.Json;
using System.IO;
using System.Windows;
using TShirtSim.Upgrades;

namespace TShirtSim
{

    internal class GameState
    {
        private readonly string filename = "tshirt_sim.json";
        public PlayerInformation PlayerInformation { get; set; }

        private DispatcherTimer AutomationTimer = new DispatcherTimer();

        private DispatcherTimer UpdateTimer = new DispatcherTimer();
        private DispatcherTimer SaveTimer = new DispatcherTimer();
        private Dictionary<UpgradeTypes, UnlockPurchase> unlockPurchases = [];


        public Dictionary<UpgradeTypes, UnlockPurchase> UnlockPurchases { get { return unlockPurchases; } }
    

        public GameState() {

            
            InitializeUnlockPurchases();

        }
        private void InitializeUnlockPurchases()
        {
            UnlockPurchases.Add(UpgradeTypes.AutoSewingMachine, new UnlockPurchase(false, false));
            UnlockPurchases.Add(UpgradeTypes.UpgradeAutobuyMaterial, new UnlockPurchase(false, false));
            UnlockPurchases.Add(UpgradeTypes.UpgradeMarketing, new UnlockPurchase(false, false));
            UnlockPurchases.Add(UpgradeTypes.UpgradeMaterialBundle, new UnlockPurchase(false, false));

        }
        public void InitializeTimers()
        {
            UpdateTimer.Interval = TimeSpan.FromMilliseconds(50);
            UpdateTimer.Tick += new EventHandler(DoSales);
            UpdateTimer.Tick += new EventHandler(CheckProgression);
            UpdateTimer.Tick += new EventHandler(AutoBuyMaterial);
/*            UpdateTimer.Tick += new EventHandler(CalculateUpgradeCost);
*/            UpdateTimer.Start();

            AutomationTimer.Interval = TimeSpan.FromSeconds(1);
            AutomationTimer.Tick += new EventHandler(ProduceTShirt);

            AutomationTimer.Start();

            SaveTimer.Interval = TimeSpan.FromSeconds(10);
            SaveTimer.Tick += new EventHandler(Save);
            SaveTimer.Start();
        }
        

        private async void Save(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    // Handle the case where filename is empty or null
                    return;
                }

                // Debug output to inspect serialized JSON
               

                using (var stream = File.Open(filename, FileMode.Create)) // Use FileMode.Create to create a new file or overwrite an existing one
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    await JsonSerializer.SerializeAsync(stream, this, options);
                }

                // Optionally, show a success message or perform other actions after saving
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file handling or serialization
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void ProduceTShirt(object? sender, EventArgs e)
        {
            PlayerInformation.CalculateTShirt();
        }
        public void HandlePriceChange(int newprice)
        {
            if (PlayerInformation.Price + newprice < 1)
            {
                return;
            }
            PlayerInformation.Price += newprice;
            PlayerInformation.CalculateInterest();
        }
        public bool HandleUpgradePurchase(UpgradeTypes upgrade)
        {
            return PlayerInformation.PurchaseUpgrade(upgrade);
        }
        public void HandleMaterialChange()
        {
            PlayerInformation.PurchaseMaterial();
        } 
        private void DoSales(object? sender, EventArgs e)
        {
            PlayerInformation.DoSales();
        }
        private void AutoBuyMaterial(object? sender, EventArgs e)
        {
            if (UnlockPurchases[UpgradeTypes.UpgradeAutobuyMaterial].Purchase)
            {
                PlayerInformation.AutoPurchaseMaterial();
            }
        }
        
        private void CheckProgression(object? sender, EventArgs e)
        {
            UnlockPurchase unlockPurchase;
            if (PlayerInformation.AmountSold >= 150)
            {
                unlockPurchase = UnlockPurchases[UpgradeTypes.AutoSewingMachine];
                unlockPurchase.Unlocked = true;
                UnlockPurchases[UpgradeTypes.AutoSewingMachine] = unlockPurchase;
            }
            if (PlayerInformation.AmountSold >= 450)
            {
                unlockPurchase = UnlockPurchases[UpgradeTypes.UpgradeMaterialBundle];
                unlockPurchase.Unlocked = true;
                UnlockPurchases[UpgradeTypes.UpgradeMaterialBundle] = unlockPurchase;
            }
            if (PlayerInformation.AmountSold >= 600)
            {
                unlockPurchase = UnlockPurchases[UpgradeTypes.UpgradeAutobuyMaterial];
                unlockPurchase.Unlocked = true;
                UnlockPurchases[UpgradeTypes.UpgradeAutobuyMaterial] = unlockPurchase;
            }
            if (PlayerInformation.AmountSold >= 100)
            {
                unlockPurchase = UnlockPurchases[UpgradeTypes.UpgradeMarketing];
                unlockPurchase.Unlocked = true;
                UnlockPurchases[UpgradeTypes.UpgradeMarketing] = unlockPurchase;
            }
            if (PlayerInformation.AmountSold > 500 && UnlockPurchases[UpgradeTypes.UpgradeMarketing].Purchase)
            {
                unlockPurchase = UnlockPurchases[UpgradeTypes.UpgradeMarketing];
                unlockPurchase.Purchase = false;
                UnlockPurchases[UpgradeTypes.UpgradeMarketing] = unlockPurchase;
            }
             
        }
    
    }
}
