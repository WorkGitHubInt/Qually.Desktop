using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BotQually
{
    public static class XmlHelper
    {
        private static readonly string accPath = AppDomain.CurrentDomain.BaseDirectory + "settings/Accounts.xml";
        private static readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "settings/Settings.xml";

        public static void LoadSettingsFromFile()
        {
            if (File.Exists(settingsPath))
            {
                using (FileStream xmlLoad = File.Open(settingsPath, FileMode.Open))
                {
                    var xs = new XmlSerializer(typeof(Settings));
                    IoC.Kernel.Get<MainViewModel>().GlobalSettings.Settings = (Settings)xs.Deserialize(xmlLoad);
                }
            }
        }

        public static void SaveSettingsToFile()
        {
            GlobalSettings globalSettings = IoC.Kernel.Get<MainViewModel>().GlobalSettings;
            Properties.Settings.Default.Sort = globalSettings.Sort;
            Properties.Settings.Default.WorkType = (int)globalSettings.WorkType;
            Properties.Settings.Default.ParallelHorse = globalSettings.ParallelHorse;
            Properties.Settings.Default.RandomPause = globalSettings.RandomPause;
            Properties.Settings.Default.Tray = globalSettings.Tray;
            Properties.Settings.Default.ClientType = (int)globalSettings.ClientType;
            Properties.Settings.Default.Save();
            using (FileStream stream = File.Create(settingsPath))
            {
                var xs = new XmlSerializer(typeof(Settings));
                xs.Serialize(stream, globalSettings.Settings);
            }
        }

        public static List<Account> LoadAccountsFromFile()
        {
            List<Account> accounts = new List<Account>();
            if (File.Exists(accPath))
            {
                using (FileStream fs = File.Open(accPath, FileMode.Open))
                {
                    var xs = new XmlSerializer(typeof(List<Account>));
                    accounts = (List<Account>)xs.Deserialize(fs);
                }
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
            using (FileStream fs = File.Open(path, FileMode.Open))
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
    }
}
