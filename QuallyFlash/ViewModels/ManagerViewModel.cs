using System;
using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class ManagerViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        public ICommand BuyCommand { get; set; }
        public ICommand SellCommand { get; set; }
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
        public string Forehead { get; set; }
        public string Bandages { get; set; }
        public string RampClassic { get; set; }
        public string RampWestern { get; set; }
        #endregion

        public ManagerViewModel(Account account)
        {
            BackCommand = new RelayCommand(() => Back());
            BuyCommand = new RelayParamCommand((product) => Buy(product as Product));
            SellCommand = new RelayParamCommand((product) => Sell(product as Product));
            Account = account;
        }

        private void Back()
        {
            MainHelper.ShowPage(new MainPage());
        }

        private async void Buy(Product product)
        {
            string quantity = GetQuantity(product);
            if (Convert.ToInt32(quantity) > 0)
            {
                await Account.Buy(product, GetQuantity(product));
                await Account.LoadProducts();
            }
            else
            {
                MessageBox.Show(Properties.Resources.ManagerBuyErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Sell(Product product)
        {
            string quantity = GetQuantity(product);
            if (Convert.ToInt32(quantity) > 0)
            {
                await Account.Sell(product, quantity);
                await Account.LoadProducts();
            }
            else
            {
                MessageBox.Show(Properties.Resources.ManagerSellErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BuyEquip(Equipment equipment)
        {
            string quantity = GetQuantity(equipment);
            if (Convert.ToInt32(quantity) > 0)
            {
                await Account.Buy(equipment, "1");
                await Account.LoadProducts();
            }
            else
            {
                MessageBox.Show("Ошибка при покупке!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private string GetQuantity(Equipment equipment)
        {
            string quantity = "0";
            switch (equipment.Type)
            {
                case EquipmentType.Forehead:
                    quantity = Forehead;
                    break;
                case EquipmentType.Bandages:
                    quantity = Bandages;
                    break;
                case EquipmentType.RampClassic:
                    quantity = RampClassic;
                    break;
                case EquipmentType.RampWestern:
                    quantity = RampWestern;
                    break;
            }
            return quantity;
        }
    }
}
