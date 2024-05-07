using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Globalization;
using TShirtSim.Upgrades;
using TShirtSim.Upgrades.Automakers;

namespace TShirtSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer_fast;
        private readonly string filename = "tshirt_sim.json";

        private BitmapImage image1 {  get; set; }
        private BitmapImage image2 { get; set; }

        private TimeSpan _timespan = TimeSpan.FromMilliseconds(50);
        private GameState _gameState;




        public MainWindow()
        {
            InitializeComponent();
            InitializeSewingMachineAnimation();
            LoadGameState();
            SetupTimers();
            
            _gameState.PlayerInformation.TShirtMade += HandleTShirtMadeAnimation;
            _gameState.PlayerInformation.SewMachinePurchased += HandlePlaceSewMachine;

            _gameState.PlayerInformation.FireInitialEvents();


        }
        private void LoadGameState()
        {
            string jsonString;
            GameState? gameState;
            try
            {
                jsonString = File.ReadAllText(filename);
                gameState = JsonSerializer.Deserialize<GameState>(jsonString);
            }
            catch
            {
                File.Create(filename);
                _gameState = new GameState();
                _gameState.PlayerInformation = new PlayerInformation();
                _gameState.PlayerInformation.LoadData();
                _gameState.InitializeTimers();
                return;
            }
            
            _gameState = gameState ?? new GameState();
            _gameState.PlayerInformation.LoadData();
            _gameState.InitializeTimers();
        }
        private void InitializeSewingMachineAnimation()
        {

            image1 = LoadBitmap("SewingMachine1.png", 1920.00);
            image2 = LoadBitmap("SewingMachine2.png", 1920.00);
            SewingMachine.Source = image2;
        }
        private void CycleAnimation(int milli)
        {
            SewingMachine.Source = image1;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milli) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                SewingMachine.Source = image2;
            };
            SewingMachine.Source = image1;


        }
        private BitmapImage LoadBitmap(string assetsRelativePath, double decodeWidth)
        {
            BitmapImage theBitmap = new BitmapImage();
            theBitmap.BeginInit();
            String basePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"images\");
            String path = System.IO.Path.Combine(basePath, assetsRelativePath);
            theBitmap.UriSource = new Uri(path, UriKind.Absolute);
            theBitmap.DecodePixelWidth = (int)decodeWidth;
            theBitmap.EndInit();
            return theBitmap;
        }


        private void SetupTimers()
        {
            _timer_fast = new DispatcherTimer();
            _timer_fast.Interval = _timespan;
            _timer_fast.Tick += new EventHandler(UpdateLabel);
            _timer_fast.Tick += new EventHandler(UpdateVisibility);


            _timer_fast.Start();
           
        }
        private void HandleTShirtMadeAnimation(object sender, int e)
        {
            CycleAnimation(e);
        }
        private void HandlePlaceSewMachine(object sender, int e)
        {
            Random random = new Random();
            var x = random.Next(0, (int)(SewMachineCanvas.ActualWidth/1.4));
            var y = random.Next(0, (int)(SewMachineCanvas.ActualHeight/1.4));

            Image image = new Image() { Source = LoadBitmap("SewingMachine1.png", 100) };
            image.Width = SewMachineCanvas.ActualWidth/2;
            image.Height = SewMachineCanvas.ActualHeight/2;
            Canvas.SetTop(image, y);
            Canvas.SetLeft(image, x);
            SewMachineCanvas.Children.Add(image);

        }


        private void Button_MakeTShirt(object sender, RoutedEventArgs e)
        {
            if (_gameState.PlayerInformation.MakeTShirt(1))
            {
            }
        }

        private void UpdateLabel(object? sender, EventArgs e)
        {
            var funds = _gameState.PlayerInformation.Treasury.ToString("C", CultureInfo.CurrentCulture);
            var sold = _gameState.PlayerInformation.AmountSold;
            
            Treasury.Content = $"Available Funds: {funds}";
            TotalSold.Content = $"Total Sold: {sold}";
            
            UpdateSewingMachineGrid();
            UpdateProductionGrid();
            UpdateBusinessGrid();
        }
        private void UpdateBusinessGrid()
        {
            UnsoldInv.Content = _gameState.PlayerInformation.UnsoldInventory;
            Interest.Content = String.Format("{0:P0}", _gameState.PlayerInformation.PublicInterest);
            PriceLabel.Content = _gameState.PlayerInformation.Price.ToString("C0", CultureInfo.CurrentCulture);
        }
        private void UpdateSewingMachineGrid()
        {
            var upgrade = _gameState.PlayerInformation.Upgrades.Find(upgrade => upgrade is SewMachine) as AutoMaker;
            var cost = upgrade.cost.ToString("C", CultureInfo.CurrentCulture);
            var amount = upgrade.amount;
            var rate = upgrade.rateOfMake;
            SewingMachinePurchase.ToolTip = $"Cost: {cost}\nAmount: {amount}\nRate: {rate} per sec";
           }
        private void UpdateProductionGrid()
        {
            var material_cost = _gameState.PlayerInformation.MaterialPrice.ToString("C0", CultureInfo.CurrentCulture);
            MaterialsLabel.Content = _gameState.PlayerInformation.Material;
            if (_gameState.PlayerInformation.Material > 0)
            {
                MaterialsLabel.Background = Brushes.LightGreen;
            }
            else if (_gameState.PlayerInformation.Material == 0)
            {
                MaterialsLabel.Background = Brushes.Red;

            }
            PurchaseMaterialButton.Content = $"Purchase Materials: {material_cost}";
        }
        private void UpdateVisibility(object? sender, EventArgs e)
        {
            if (_gameState.HasUnlockedSewingMachine == true)
            {
                SewingMachGrid.Visibility = Visibility.Visible;
            }
            if (_gameState.HasUnlockedBiggerMaterial1 == true && _gameState.HasPurchasedBiggerMaterial1 == false) {
                if (!CheckButtonExistence("BiggerMaterial1UpgradeButton"))
                {
                    var bundle = _gameState.PlayerInformation.Upgrades.Find(upgrade => upgrade.upgradeType == UpgradeTypes.UpgradeMaterialBundle) as UpgradedMaterialBundle;

                    Button button = SetUpButton("BiggerMaterial1UpgradeButton", 
                        LoadBitmap(bundle.IconFileName, 800),
                        $"{bundle.Name} ${bundle.cost}\n{bundle.Description}");
                    button.Click += (object? sender, RoutedEventArgs e) =>
                    {
                        if (_gameState.HandleUpgradePurchase(UpgradeTypes.UpgradeMaterialBundle))
                        {
                            _gameState.HasPurchasedBiggerMaterial1 = true;
                            UpgradePanel.Children.Remove(button);
                        }
                       
                    };
                    UpgradePanel.Children.Add(button);
                }
            }
            if (_gameState.HasUnlockedAutoMaterial && !_gameState.HasPurchasedAutoMaterial)
            {
                if (!CheckButtonExistence("AutoMaterialUpgradeButton"))
                {
                    var automat = _gameState.PlayerInformation.Upgrades.Find(upgrade => upgrade.upgradeType == UpgradeTypes.UpgradeAutobuyMaterial) as UpgradeAutobuyMaterial;
                    Button button = SetUpButton("AutoMaterialUpgradeButton", 
                        LoadBitmap("AutobuyMaterial.png", 800), 
                        $"{automat.Name} ${automat.cost}\nPurchases Materials automatically when you run out");
                    button.Click += (object? sender, RoutedEventArgs e) =>
                    {
                        if (_gameState.HandleUpgradePurchase(UpgradeTypes.UpgradeAutobuyMaterial))
                        {
                            _gameState.HasPurchasedAutoMaterial = true;
                            UpgradePanel.Children.Remove(button);
                        }
                        
                    };
                    UpgradePanel.Children.Add(button);
                }
            }
            if (_gameState.HasUnlockedMarketingLevel1 && !_gameState.HasPurchasedMarketingLevel1)
            {
                if (!CheckButtonExistence("MarketingLevel1UpgradeButton"))
                {
                    var marketing = _gameState.PlayerInformation.Upgrades.Find(upgrade => upgrade.upgradeType == UpgradeTypes.UpgradeMarketing) as UpgradeMarketing;
                    
                    Button button = SetUpButton("MarketingLevel1UpgradeButton", LoadBitmap("Marketing.png", 800), $"{marketing.Name} ${marketing.cost}\nHigher public interest for your T-Shirts");
                    button.Click += (object? sender, RoutedEventArgs e) =>
                    {
                        if (_gameState.HandleUpgradePurchase(UpgradeTypes.UpgradeMarketing))
                        {
                            _gameState.HasPurchasedMarketingLevel1 = true;
                            UpgradePanel.Children.Remove(button);
                        }

                    };
                    UpgradePanel.Children.Add(button);
                }
            }
            
        }
        
        private Button SetUpButton(string name, BitmapImage image, string tooltip)
        {
            Button button = new Button();
            button.Name = name;
            button.Content = new Image { Source = image };
            button.ToolTip = tooltip;
            button.FontSize = 16;
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.Margin = new Thickness(7);
            button.Width = button.Height = 75;
            return button;

        }



        private bool CheckButtonExistence(string name)
        {
            UIElementCollection collection = UpgradePanel.Children;
            foreach (UIElement element in collection)
            {
                if (element is Button && element != null)
                {
                    Button? button = element as Button;
                    if (button.Name == name)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        private void Button_RaisePrice(object sender, RoutedEventArgs e)
        {
            _gameState.HandlePriceChange(1);
        }

        private void Button_LowerPrice(object sender, RoutedEventArgs e)
        {
            _gameState.HandlePriceChange(-1);
        }

        private void Button_BuyMaterial(object sender, RoutedEventArgs e)
        {
            _gameState.HandleMaterialChange();
        }

        private void Button_BuySewMachine(object sender, RoutedEventArgs e)
        {
            _gameState.HandleUpgradePurchase(UpgradeTypes.AutoSewingMachine);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}