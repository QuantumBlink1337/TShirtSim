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
using System.Xml.Linq;
using System.Reflection.Metadata;

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

        private BitmapImage tshirt1 { get; set; }
        private BitmapImage tshirt2 {  get; set; }

        private TimeSpan _timespan = TimeSpan.FromMilliseconds(50);
        private GameState _gameState;

        private Image? draggedImage;
        private Point mousePosition;




        public MainWindow()
        {
            InitializeComponent();
            InitializeMakeTShirtAni();
            LoadGameState();
            SetupTimers();
            
/*            _gameState.PlayerInformation.TShirtMade += HandleTShirtMadeAnimation;
*/            _gameState.PlayerInformation.SewMachinePurchased += HandlePlaceSewMachine;
            _gameState.PlayerInformation.SewMachineMake += HandleSewMachineMake;



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
        private void InitializeMakeTShirtAni()
        {

            image1 = LoadBitmap("SewingMachine1.png", 1920.00);
            image2 = LoadBitmap("SewingMachine2.png", 1920.00);


            
            tshirt1 = LoadBitmap("EmptyTShirt.png", 1000);
            tshirt2 = LoadBitmap("EmptyTShirt2.png", 1000);


            TShirtMake.Content = new Image { Source = tshirt1};
            

/*            SewingMachine.Source = image2;
*/        }
       /* private void CycleAnimation(int milli)
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
        }*/
        private void CycleSewMachineAnimation(int milli)
        {
            var collection = SewMachineCanvas.Children;
            foreach (Image item in collection)
            {
                item.Source = image1;
            }
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milli) };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                foreach (Image item in collection)
                {
                    item.Source = image2;
                }
            };
            timer.Start();
        }
        private void CycleTShirtmakeAnimation()
        {
            TShirtMake.Content = new Image { Source = tshirt1 };
            var milli = 25;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milli) };
            var timer2 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milli) };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                TShirtMake.Content = new Image { Source = tshirt2 };
                timer2.Start();


            };
            timer.Start();
            timer2.Tick += (sender, args) =>
            {
                timer2.Stop();
                TShirtMake.Content = new Image { Source = tshirt1 };


            };
            timer.Start();
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
       /* private void HandleTShirtMadeAnimation(object sender, int e)
        {
            CycleAnimation(e);
        }*/
        private void HandlePlaceSewMachine(object sender, int e)
        {
            PlaceSewMachine();

        }
        private void HandleSewMachineMake(object sender, int e)
        {
            CycleSewMachineAnimation(e); 
        }
        private void PlaceSewMachine()
        {
            Random random = new Random();
            var x = random.Next(0, (int)(SewMachineCanvas.ActualWidth / 1.4));
            var y = random.Next(0, (int)(SewMachineCanvas.ActualHeight / 1.4));

            Image image = new Image() { Source = LoadBitmap("SewingMachine1.png", 100) };
            image.Width = SewMachineCanvas.ActualWidth / 2;
            image.Height = SewMachineCanvas.ActualHeight / 2;
            Canvas.SetTop(image, y);
            Canvas.SetLeft(image, x);
            SewMachineCanvas.Children.Add(image);
        }

        private void Button_MakeTShirt(object sender, RoutedEventArgs e)
        {
            if (_gameState.PlayerInformation.MakeTShirt(1))
            {
                CycleTShirtmakeAnimation();
                TShirtMake.Content = new Image { Source = tshirt1 };

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
            var cost = upgrade.Cost.ToString("C", CultureInfo.CurrentCulture);
            var amount = upgrade.amount;
            var rate = upgrade.rateOfMake;
            var description = upgrade.Description;
           
            if (amount > SewMachineCanvas.Children.Count)
            {
                var count = amount - SewMachineCanvas.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    PlaceSewMachine();
                }
            }
            SewingMachinePurchase.ToolTip = $"{description}\nCost: {cost}\nAmount: {amount}\nRate: {rate} per sec";
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
            var x = this.Width;
            var y = this.Height;
            if (_gameState.UnlockPurchases[UpgradeTypes.AutoSewingMachine].Unlocked == true)
            {
                SewingMachGrid.Visibility = Visibility.Visible;
            }
            if (UpgradePanel.Children.Count <= 4)
            {
                foreach (UpgradeTypes upgradeType in _gameState.UnlockPurchases.Keys)
                {
                    var upgrade = _gameState.PlayerInformation.Upgrades.Find(upgrade => upgrade.upgradeType == upgradeType);
                    if (_gameState.UnlockPurchases[upgradeType].Unlocked && !_gameState.UnlockPurchases[upgradeType].Purchase && !(upgrade is AutoMaker))
                    {
                        var name = upgrade.Name;
                        if (!CheckButtonExistence(Utility.ReplaceWhitespace(name, "") + "Button"))
                        {
                            Button button = SetUpButton(name,
                            LoadBitmap(upgrade.IconFileName, 800),
                            $"{upgrade.Name} ${upgrade.Cost}\n{upgrade.Description}");
                            button.Click += (object? sender, RoutedEventArgs e) =>
                            {
                                if (_gameState.HandleUpgradePurchase(upgradeType))
                                {
                                    var purchase = _gameState.UnlockPurchases[upgradeType];
                                    purchase.Purchase = true;
                                    _gameState.UnlockPurchases[upgradeType] = purchase;
                                    UpgradePanel.Children.Remove(button);

                                }
                            };

                            UpgradePanel.Children.Add(button);
                            this.Width = x;
                            this.Height = y;


                        }
                    }
                }
            }
            
        }
        
        private Button SetUpButton(string name, BitmapImage image, string tooltip)
        {
            Button button = new Button()
            {
                Name = Utility.ReplaceWhitespace(name, "") + "Button",
                Content = new Image { Source = image },
                ToolTip = tooltip,
                FontSize = 16,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(7),
                Width = Height = 100
            };
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

        private void AutomakerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;
            var canvas = sender as Canvas;
            if (image != null && canvas.CaptureMouse()) 
            {
                mousePosition = e.GetPosition(canvas);
                draggedImage = image;
            }
        }

        private void AutomakerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            if (draggedImage != null)
            {
                canvas.ReleaseMouseCapture();
                draggedImage = null;
            }
        }

        private void AutomakerCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;
            if (draggedImage != null)
            {
                var position = e.GetPosition(canvas);
                var offset = position - mousePosition;
                var X = offset.X; 
                var Y = offset.Y;
               
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);


            }
        }
    }
}