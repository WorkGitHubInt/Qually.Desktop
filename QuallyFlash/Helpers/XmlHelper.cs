using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace QuallyFlash
{
    public static class XmlHelper
    {
        private static readonly string accPath = AppDomain.CurrentDomain.BaseDirectory + "settings/Accounts.xml";

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
