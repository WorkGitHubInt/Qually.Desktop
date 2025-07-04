﻿using System;
using System.Collections.Generic;

namespace BotQually
{
    public class Product : BaseModel
    {
        private readonly Dictionary<string, string> ids = new Dictionary<string, string>
        {
            {"https://au.howrse.com-Hay", "396"},
            {"https://au.howrse.com-Oat", "26"},
            {"https://au.howrse.com-Wheat", "97"},
            {"https://au.howrse.com-Shit", "69"},
            {"https://au.howrse.com-Leather", "406"},
            {"https://au.howrse.com-Apples", "18"},
            {"https://au.howrse.com-Carrot", "19"},
            {"https://au.howrse.com-Wood", "403"},
            {"https://au.howrse.com-Steel", "404"},
            {"https://au.howrse.com-Sand", "405"},
            {"https://au.howrse.com-Straw", "33"},
            {"https://au.howrse.com-Flax", "32"},
            {"https://au.howrse.com-OR", "81"},

            {"https://www.howrse.co.uk-Hay", "406"},
            {"https://www.howrse.co.uk-Oat", "26"},
            {"https://www.howrse.co.uk-Wheat", "97"},
            {"https://www.howrse.co.uk-Shit", "69"},
            {"https://www.howrse.co.uk-Leather", "416"},
            {"https://www.howrse.co.uk-Apples", "18"},
            {"https://www.howrse.co.uk-Carrot", "19"},
            {"https://www.howrse.co.uk-Wood", "413"},
            {"https://www.howrse.co.uk-Steel", "414"},
            {"https://www.howrse.co.uk-Sand", "415"},
            {"https://www.howrse.co.uk-Straw", "33"},
            {"https://www.howrse.co.uk-Flax", "32"},
            {"https://www.howrse.co.uk-OR", "81"},

            {"https://ar.howrse.com-Hay", "375"},
            {"https://ar.howrse.com-Oat", "26"},
            {"https://ar.howrse.com-Wheat", "97"},
            {"https://ar.howrse.com-Shit", "69"},
            {"https://ar.howrse.com-Leather", "385"},
            {"https://ar.howrse.com-Apples", "18"},
            {"https://ar.howrse.com-Carrot", "19"},
            {"https://ar.howrse.com-Wood", "382"},
            {"https://ar.howrse.com-Steel", "383"},
            {"https://ar.howrse.com-Sand", "384"},
            {"https://ar.howrse.com-Straw", "97"},
            {"https://ar.howrse.com-Flax", "32"},
            {"https://ar.howrse.com-OR", "81"},

            {"https://www.howrse.bg-Hay", "396"},
            {"https://www.howrse.bg-Oat", "26"},
            {"https://www.howrse.bg-Wheat", "97"},
            {"https://www.howrse.bg-Shit", "69"},
            {"https://www.howrse.bg-Leather", "406"},
            {"https://www.howrse.bg-Apples", "18"},
            {"https://www.howrse.bg-Carrot", "19"},
            {"https://www.howrse.bg-Wood", "403"},
            {"https://www.howrse.bg-Steel", "404"},
            {"https://www.howrse.bg-Sand", "405"},
            {"https://www.howrse.bg-Straw", "33"},
            {"https://www.howrse.bg-Flax", "32"},
            {"https://www.howrse.bg-OR", "81"},

            {"https://www.howrse.com-Hay", "396"},
            {"https://www.howrse.com-Oat", "26"},
            {"https://www.howrse.com-Wheat", "97"},
            {"https://www.howrse.com-Shit", "69"},
            {"https://www.howrse.com-Leather", "406"},
            {"https://www.howrse.com-Apples", "18"},
            {"https://www.howrse.com-Carrot", "19"},
            {"https://www.howrse.com-Wood", "403"},
            {"https://www.howrse.com-Steel", "404"},
            {"https://www.howrse.com-Sand", "405"},
            {"https://www.howrse.com-Straw", "33"},
            {"https://www.howrse.com-Flax", "32"},
            {"https://www.howrse.com-OR", "81"},

            {"https://www.caballow.com-Hay", "387"},
            {"https://www.caballow.com-Oat", "26"},
            {"https://www.caballow.com-Wheat", "90"},
            {"https://www.caballow.com-Shit", "64"},
            {"https://www.caballow.com-Leather", "397"},
            {"https://www.caballow.com-Apples", "18"},
            {"https://www.caballow.com-Carrot", "19"},
            {"https://www.caballow.com-Wood", "394"},
            {"https://www.caballow.com-Steel", "395"},
            {"https://www.caballow.com-Sand", "396"},
            {"https://www.caballow.com-Straw", "32"},
            {"https://www.caballow.com-Flax", "31"},
            {"https://www.caballow.com-OR", "76"},

            {"https://ca.howrse.com-Hay", "396"},
            {"https://ca.howrse.com-Oat", "26"},
            {"https://ca.howrse.com-Wheat", "97"},
            {"https://ca.howrse.com-Shit", "69"},
            {"https://ca.howrse.com-Leather", "406"},
            {"https://ca.howrse.com-Apples", "18"},
            {"https://ca.howrse.com-Carrot", "19"},
            {"https://ca.howrse.com-Wood", "403"},
            {"https://ca.howrse.com-Steel", "404"},
            {"https://ca.howrse.com-Sand", "405"},
            {"https://ca.howrse.com-Straw", "33"},
            {"https://ca.howrse.com-Flax", "32"},
            {"https://ca.howrse.com-OR", "81"},

            {"https://www.howrse.de-Hay", "389"},
            {"https://www.howrse.de-Oat", "26"},
            {"https://www.howrse.de-Wheat", "97"},
            {"https://www.howrse.de-Shit", "69"},
            {"https://www.howrse.de-Leather", "399"},
            {"https://www.howrse.de-Apples", "18"},
            {"https://www.howrse.de-Carrot", "19"},
            {"https://www.howrse.de-Wood", "396"},
            {"https://www.howrse.de-Steel", "397"},
            {"https://www.howrse.de-Sand", "398"},
            {"https://www.howrse.de-Straw", "33"},
            {"https://www.howrse.de-Flax", "32"},
            {"https://www.howrse.de-OR", "81"},

            {"https://www.howrse.no-Hay", "396"},
            {"https://www.howrse.no-Oat", "26"},
            {"https://www.howrse.no-Wheat", "97"},
            {"https://www.howrse.no-Shit", "69"},
            {"https://www.howrse.no-Leather", "406"},
            {"https://www.howrse.no-Apples", "18"},
            {"https://www.howrse.no-Carrot", "19"},
            {"https://www.howrse.no-Wood", "403"},
            {"https://www.howrse.no-Steel", "404"},
            {"https://www.howrse.no-Sand", "405"},
            {"https://www.howrse.no-Straw", "33"},
            {"https://www.howrse.no-Flax", "32"},
            {"https://www.howrse.no-OR", "81"},

            {"https://www.howrse.pl-Hay", "396"},
            {"https://www.howrse.pl-Oat", "26"},
            {"https://www.howrse.pl-Wheat", "97"},
            {"https://www.howrse.pl-Shit", "69"},
            {"https://www.howrse.pl-Leather", "406"},
            {"https://www.howrse.pl-Apples", "18"},
            {"https://www.howrse.pl-Carrot", "19"},
            {"https://www.howrse.pl-Wood", "403"},
            {"https://www.howrse.pl-Steel", "404"},
            {"https://www.howrse.pl-Sand", "405"},
            {"https://www.howrse.pl-Straw", "33"},
            {"https://www.howrse.pl-Flax", "32"},
            {"https://www.howrse.pl-OR", "81"},

            {"https://www.lowadi.com-Hay", "446"},
            {"https://www.lowadi.com-Oat", "109"},
            {"https://www.lowadi.com-Wheat", "169"},
            {"https://www.lowadi.com-Shit", "147"},
            {"https://www.lowadi.com-Leather", "456"},
            {"https://www.lowadi.com-Apples", "101"},
            {"https://www.lowadi.com-Carrot", "102"},
            {"https://www.lowadi.com-Wood", "453"},
            {"https://www.lowadi.com-Steel", "454"},
            {"https://www.lowadi.com-Sand", "455"},
            {"https://www.lowadi.com-Straw", "115"},
            {"https://www.lowadi.com-Flax", "114"},
            {"https://www.lowadi.com-OR", "76"},

            {"https://www.howrse.ro-Hay", "396"},
            {"https://www.howrse.ro-Oat", "26"},
            {"https://www.howrse.ro-Wheat", "97"},
            {"https://www.howrse.ro-Shit", "69"},
            {"https://www.howrse.ro-Leather", "406"},
            {"https://www.howrse.ro-Apples", "18"},
            {"https://www.howrse.ro-Carrot", "19"},
            {"https://www.howrse.ro-Wood", "403"},
            {"https://www.howrse.ro-Steel", "404"},
            {"https://www.howrse.ro-Sand", "405"},
            {"https://www.howrse.ro-Straw", "33"},
            {"https://www.howrse.ro-Flax", "32"},
            {"https://www.howrse.ro-OR", "81"},

            {"https://us.howrse.com-Hay", "396"},
            {"https://us.howrse.com-Oat", "26"},
            {"https://us.howrse.com-Wheat", "97"},
            {"https://us.howrse.com-Shit", "69"},
            {"https://us.howrse.com-Leather", "406"},
            {"https://us.howrse.com-Apples", "18"},
            {"https://us.howrse.com-Carrot", "19"},
            {"https://us.howrse.com-Wood", "403"},
            {"https://us.howrse.com-Steel", "404"},
            {"https://us.howrse.com-Sand", "405"},
            {"https://us.howrse.com-Straw", "33"},
            {"https://us.howrse.com-Flax", "32"},
            {"https://us.howrse.com-OR", "81"},

            {"https://ouranos.equideow.com-Hay", "389"},
            {"https://ouranos.equideow.com-Oat", "26"},
            {"https://ouranos.equideow.com-Wheat", "97"},
            {"https://ouranos.equideow.com-Shit", "69"},
            {"https://ouranos.equideow.com-Leather", "406"},
            {"https://ouranos.equideow.com-Apples", "18"},
            {"https://ouranos.equideow.com-Carrot", "19"},
            {"https://ouranos.equideow.com-Wood", "403"},
            {"https://ouranos.equideow.com-Steel", "404"},
            {"https://ouranos.equideow.com-Sand", "405"},
            {"https://ouranos.equideow.com-Straw", "33"},
            {"https://ouranos.equideow.com-Flax", "32"},
            {"https://ouranos.equideow.com-OR", "81"},

            {"https://gaia.equideow.com-Hay", "390"},
            {"https://gaia.equideow.com-Oat", "26"},
            {"https://gaia.equideow.com-Wheat", "97"},
            {"https://gaia.equideow.com-Shit", "69"},
            {"https://gaia.equideow.com-Leather", "400"},
            {"https://gaia.equideow.com-Apples", "18"},
            {"https://gaia.equideow.com-Carrot", "19"},
            {"https://gaia.equideow.com-Wood", "403"},
            {"https://gaia.equideow.com-Steel", "404"},
            {"https://gaia.equideow.com-Sand", "405"},
            {"https://gaia.equideow.com-Straw", "33"},
            {"https://gaia.equideow.com-Flax", "32"},
            {"https://gaia.equideow.com-OR", "81"},

            {"https://www.howrse.se-Hay", "396"},
            {"https://www.howrse.se-Oat", "26"},
            {"https://www.howrse.se-Wheat", "97"},
            {"https://www.howrse.se-Shit", "69"},
            {"https://www.howrse.se-Leather", "406"},
            {"https://www.howrse.se-Apples", "18"},
            {"https://www.howrse.se-Carrot", "19"},
            {"https://www.howrse.se-Wood", "403"},
            {"https://www.howrse.se-Steel", "404"},
            {"https://www.howrse.se-Sand", "405"},
            {"https://www.howrse.se-Straw", "33"},
            {"https://www.howrse.se-Flax", "32"},
            {"https://www.howrse.se-OR", "81"},

        };

        public ProductType Type { get; set; }
        public string Id { get; set; }
        public int Amount { get; set; } = 0;
        public int SellPrice { get; private set; }

        public Product(string baseAddress, ProductType type, int sellPrice)
        {
            Type = type;
            SellPrice = sellPrice;
            Id = ids[$"{baseAddress}-{Enum.GetName(typeof(ProductType), type)}"];
        }
    }
}
