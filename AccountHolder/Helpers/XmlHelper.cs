using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AccountHolder
{
    public static class XmlHelper
    {
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
