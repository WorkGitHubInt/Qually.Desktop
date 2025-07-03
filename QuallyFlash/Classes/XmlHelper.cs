using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace QuallyFlash
{
    public static class XmlHelper
    {
        private static readonly string accPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Accounts.xml");
        private static readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Settings.xml");

        public static List<Account> LoadAccountsFromFile()
        {
            List<Account> accounts = new List<Account>();
            try
            {
                if (File.Exists(accPath))
                {
                    using (FileStream fs = File.Open(accPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var xs = new XmlSerializer(typeof(List<Account>));
                        accounts = (List<Account>)xs.Deserialize(fs);
                    }
                }
            } catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Произошла ошибка при загрузке аккаунтов! Сохраненные аккаунты будут сброшены! Направьте скриншот администрации! \nMessage: {e.Message} StackTrace: {e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveAccountsToFile(accounts);
            }
            return accounts;
        }

        public static void SaveAccountsToFile(List<Account> accounts)
        {
            using (FileStream fs = File.Create(accPath))
            {
                var xs = new XmlSerializer(typeof(List<Account>));
                xs.Serialize(fs, accounts);
            }
        }

        public static List<Account> LoadAccountsFromFile(string path)
        {
            List<Account> accounts = new List<Account>();
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xs = new XmlSerializer(typeof(List<Account>));
                accounts = (List<Account>)xs.Deserialize(fs);
            }
            return accounts;
        }

        public static void SaveAccountsToFile(List<Account> accounts, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                var xs = new XmlSerializer(typeof(List<Account>));
                xs.Serialize(fs, accounts);
            }
        }

        public static GlobalSettings LoadGlobalSettingsFromFile()
        {
            GlobalSettings settings;
            try
            {
                using (FileStream fs = File.Open(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xs = new XmlSerializer(typeof(GlobalSettings));
                    settings = (GlobalSettings)xs.Deserialize(fs);
                }
            } catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Произошла ошибка при загрузке настроек! Общие настройки будут сброшены! Направьте скриншот администрации! \nMessage: {e.Message} StackTrace: {e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                settings = new GlobalSettings();
            }
            return settings;
        }

        public static void SaveGlobalSettingsToFile(GlobalSettings settings)
        {
            using (FileStream fs = File.Create(settingsPath))
            {
                var xs = new XmlSerializer(typeof(GlobalSettings));
                xs.Serialize(fs, settings);
            }
        }
    }
}
