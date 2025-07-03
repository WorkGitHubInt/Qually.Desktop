using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Linq;

namespace QuallyFlash
{
    public class NamesViewModel : BaseViewModel
    {
        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand OpenGuideCommand { get; set; }
        public ICommand AddMaleNameCommand { get; set; }
        public ICommand RemoveMaleNameCommand { get; set; }
        public ICommand ResetMaleNameCommand { get; set; }
        public ICommand AddFemaleNameCommand { get; set; }
        public ICommand RemoveFemaleNameCommand { get; set; }
        public ICommand ResetFemaleNameCommand { get; set; }
        #endregion

        public string Logo { get; set; } = "Qually Flash";

        public ObservableCollection<string> MaleNames { get; set; }
        public ObservableCollection<string> FemaleNames { get; set; }
        public GlobalSettings GlobalSettings { get; set; }
        public string SelectedMaleName { get; set; }
        public string SelectedFemaleName { get; set; }
        public string MaleName { get; set; }
        public string FemaleName { get; set; }

        public NamesViewModel(Window window)
        {
            MinimizeCommand = new RelayCommand(() => Minimize(window));
            MaximizeCommand = new RelayCommand(() => Maximize(window));
            CloseCommand = new RelayCommand(() => window.Close());
            Logo = $"Qually Flash ({MainHelper.GetProgramVersion().Substring(0, MainHelper.GetProgramVersion().Length - 4)})";
            AddMaleNameCommand = new RelayCommand(() => AddMaleName());
            RemoveMaleNameCommand = new RelayCommand(() => RemoveMaleName());
            ResetMaleNameCommand = new RelayCommand(() => ResetMaleName());
            AddFemaleNameCommand = new RelayCommand(() => AddFemaleName());
            RemoveFemaleNameCommand = new RelayCommand(() => RemoveFemaleName());
            ResetFemaleNameCommand = new RelayCommand(() => ResetFemaleName());
            GlobalSettings = GlobalSettings.GetInstance();
            MaleNames = GlobalSettings.MaleNames.Count == 0 ? new ObservableCollection<string>() : GlobalSettings.MaleNames.ToObservableCollection();
            FemaleNames = GlobalSettings.FemaleNames.Count == 0 ? new ObservableCollection<string>() : GlobalSettings.FemaleNames.ToObservableCollection();
        }

        private void Minimize(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        private bool maxCheck = false;
        private double width = 0;
        private double height = 0;
        private double left = 0;
        private double top = 0;
        private void Maximize(Window window)
        {
            if (!maxCheck)
            {
                width = window.Width;
                height = window.Height;
                left = window.Left;
                top = window.Top;
                window.Width = SystemParameters.WorkArea.Width;
                window.Height = SystemParameters.WorkArea.Height;
                window.Left = 0;
                window.Top = 0;
                maxCheck = true;
            }
            else
            {
                window.Width = width;
                window.Height = height;
                window.Left = left;
                window.Top = top;
                maxCheck = false;
            }
        }

        public void AddMaleName()
        {
            if (!string.IsNullOrEmpty(MaleName))
            {
                MaleNames.Add(MaleName);
                SaveMaleList();
                MaleName = string.Empty;
            } else
            {
                MessageBox.Show(Properties.Resources.NamesWindowErrorMessage1, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveMaleName()
        {
            if (SelectedMaleName != null)
            {
                MaleNames.Remove(SelectedMaleName);
                SaveMaleList();
            }
        }

        public void ResetMaleName()
        {
            MaleNames.Clear();
            GlobalSettings.MaleNames = null;
        }

        public void AddFemaleName()
        {
            if (!string.IsNullOrEmpty(FemaleName))
            {
                FemaleNames.Add(FemaleName);
                SaveFemaleList();
                FemaleName = string.Empty;
            } else
            {
                MessageBox.Show(Properties.Resources.NamesWindowErrorMessage1, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveFemaleName()
        {
            if (SelectedFemaleName != null)
            {
                FemaleNames.Remove(SelectedFemaleName);
                SaveFemaleList();
            }
        }

        public void ResetFemaleName()
        {
            FemaleNames.Clear();
            GlobalSettings.FemaleNames = null;
        }

        public void SaveFemaleList()
        {
            GlobalSettings.FemaleNames = FemaleNames.ToList();
        }

        public void SaveMaleList()
        {
            GlobalSettings.MaleNames = MaleNames.ToList();
        }
    }
}
