using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AccountHolder
{
    public static class XmlHelper
    {
        private static readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml");

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

        public static void SaveSettingsToFile(Settings settings)
        {
            using (FileStream fs = File.Create(settingsPath))
            {
                var xs = new XmlSerializer(typeof(Settings));
                xs.Serialize(fs, settings);
            }
        }

        public static Settings LoadSettingsFromFile()
        {
            if (!File.Exists(settingsPath))
            {
                SaveSettingsToFile(new Settings());
            }
            Settings settings;
            using (FileStream fs = File.Open(settingsPath, FileMode.OpenOrCreate))
            {
                var xs = new XmlSerializer(typeof(Settings));
                settings = (Settings)xs.Deserialize(fs);
            }
            return settings;
        }
    }
}
