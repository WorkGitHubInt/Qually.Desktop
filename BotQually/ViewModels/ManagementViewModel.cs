using System;
using System.Windows;
using System.Windows.Input;

namespace BotQually
{
    public class ManagementViewModel : BaseViewModel
    {
        #region Command
        public ICommand BuyCommand { get; set; }
        public ICommand SellCommand { get; set; }
        public ICommand ReturnCommand { get; set; }
        #endregion

        #region Properties
        public Account Account { get; set; }
        public string Hay { get; set; }
        public string Oat { get; set; }
        public string Wheat { get; set; }
        public string Leather { get; set; }
        public string Shit { get; set; }
        public string Apples { get; set; }
        public string Carrot { get; set; }
        public string Wood { get; set; }
        public string Steel { get; set; }
        public string Sand { get; set; }
        public string Straw { get; set; }
        public string Flax { get; set; }
        #endregion

        public ManagementViewModel(Account account)
        {
            Account = account;
            ReturnCommand = new RelayCommand(() => Return());
            BuyCommand = new RelayParamCommand((product) => Buy(product as Product));
            SellCommand = new RelayParamCommand((product) => Sell(product as Product));
        }

        private async void Buy(Product product)
        {
            string quantity = GetQuantity(product);
            if (Convert.ToInt32(quantity) > 0)
            {
                await Account.Buy(product, GetQuantity(product));
                await Account.LoadProducts();
            } else
            {
                MessageBox.Show(Properties.Resources.ManagmentBuyErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Sell(Product product)
        {
            string quantity = GetQuantity(product);
            if (Convert.ToInt32(quantity) > 0)
            {
                await Account.Sell(product, quantity);
                await Account.LoadProducts();
            } else
            {
                MessageBox.Show(Properties.Resources.ManagmentSellErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string GetQuantity(Product product)
        {
            string quantity = "0";
            switch (product.Type)
            {
                case ProductType.Hay:
                    quantity = Hay;
                    break;
                case ProductType.Oat:
                    quantity = Oat;
                    break;
                case ProductType.Wheat:
                    quantity = Wheat;
                    break;
                case ProductType.Shit:
                    quantity = Shit;
                    break;
                case ProductType.Leather:
                    quantity = Leather;
                    break;
                case ProductType.Apples:
                    quantity = Apples;
                    break;
                case ProductType.Carrot:
                    quantity = Carrot;
                    break;
                case ProductType.Wood:
                    quantity = Wood;
                    break;
                case ProductType.Steel:
                    quantity = Steel;
                    break;
                case ProductType.Sand:
                    quantity = Sand;
                    break;
                case ProductType.Straw:
                    quantity = Straw;
                    break;
                case ProductType.Flax:
                    quantity = Flax;
                    break;
            }
            return quantity;
        }

        private void Return()
        {
            MainHelper.ShowPage(new MainPage());
        }
    }
}
