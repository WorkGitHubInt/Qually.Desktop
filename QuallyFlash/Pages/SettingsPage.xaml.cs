using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuallyFlash
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool Loading = true;
        private void SchemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as SettingsViewModel;
            if (!Loading)
            {
                vm.Settings.Scheme.Clear();
                vm.Training = new ObservableCollection<Training>()
                {
                    new Training("Лес", "forest"),
                    new Training("Горы", "mountain"),
                    new Training("Выносливость", "vitality"),
                    new Training("Скорость", "speed"),
                    new Training("Выездка", "dressage"),
                    new Training("Галоп", "galop"),
                    new Training("Рысь", "lynx"),
                    new Training("Прыжки", "jump"),
                };
                vm.Classic = new ObservableCollection<Training>()
                {
                    new Training("Рысь(с)", "lynxcompet"),
                    new Training("Галоп(с)", "galopcompet"),
                    new Training("Выездка(с)", "dressagecompet"),
                    new Training("Кросс", "cross"),
                    new Training("Конкур", "concur"),
                };
                vm.Western = new ObservableCollection<Training>()
                {
                    new Training("Бочки", "barell"),
                    new Training("Каттинг", "cutting"),
                    new Training("Трейл", "trail"),
                    new Training("Рейнинг", "raining"),
                    new Training("Плеже", "plege"),
                };
                vm.SpecList = (vm.Settings.Specialization == Specialization.Classic) ? vm.Classic : vm.Western;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loading = false;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var vm = DataContext as SettingsViewModel;
            Training data = (Training)e.Data.GetData(typeof(Training));
            Training target = (Training)((ListBoxItem)(sender)).DataContext;
            int removeIndex = SchemeBox.Items.IndexOf(data);
            int targetIndex = SchemeBox.Items.IndexOf(target);
            if (removeIndex < targetIndex)
            {
                vm.Settings.Scheme.Insert(targetIndex + 1, data);
                vm.Settings.Scheme.RemoveAt(removeIndex);
            }
            else
            {
                int remIdx = removeIndex + 1;
                if (vm.Settings.Scheme.Count + 1 > remIdx)
                {
                    vm.Settings.Scheme.Insert(targetIndex, data);
                    vm.Settings.Scheme.RemoveAt(remIdx);
                }
            }
        }

        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as SettingsViewModel;
            if (sender is ListBoxItem)
            {
                try
                {
                    ListBoxItem draggedItem = sender as ListBoxItem;
                    vm.SelectedSchemeItem = (Training)draggedItem.DataContext;
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                }
                catch
                {

                }
            }
        }
    }
}
