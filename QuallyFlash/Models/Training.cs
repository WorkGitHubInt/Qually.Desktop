using AngleSharp.Dom;
using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace QuallyFlash
{
    public class Training : BaseModel, ICloneable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        [XmlIgnore]
        public bool IsDone { get; set; } = false;

        public Training(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Training()
        {
        }

        public void LoadTraining(string script, IHtmlCollection<IElement> a, int lastIndex, Horse horse)
        {
            if (horse.Age >= 18)
            {
                switch (Value)
                {
                    case "forest": LoadSkill(script, "b3"); break;
                    case "mountain": LoadSkill(script, "b1"); break;
                }
            }
            if (horse.Age >= 24)
            {
                switch (Value)
                {
                    case "vitality": LoadSkill(script, "e1"); break;
                    case "speed": LoadSkill(script, "e2"); break;
                    case "dressage": LoadSkill(script, "e3"); break;
                    case "galop": LoadSkill(script, "e4"); break;
                    case "lynx": LoadSkill(script, "e5"); break;
                    case "jump": LoadSkill(script, "e6"); break;
                }
            }
            if (horse.Age >= 36 && horse.IsSpecialized && horse.IsEquiped)
            {
                switch (Value)
                {
                    case "lynxcompet": LoadCompet(a[0]); break;
                    case "galopcompet": LoadCompet(a[1]); break;
                    case "dressagecompet": LoadCompet(a[2]); break;
                    case "cross": LoadCompet(a[3]); break;
                    case "concur": LoadCompet(a[lastIndex]); break;
                    case "barell": LoadCompet(a[0]); break;
                    case "cutting": LoadCompet(a[1]); break;
                    case "trail": LoadCompet(a[2]); break;
                    case "raining": LoadCompet(a[3]); break;
                    case "plege": LoadCompet(a[lastIndex]); break;
                }
            }
        }

        public void ParseSkill(double b3, double b1, double e1, double e2, double e3, double e4, double e5, double e6)
        {
            switch (Value)
            {
                case "forest": IsDone = b3 >= 100; break;
                case "mountain": IsDone = b1 >= 100; break;
                case "vitality": IsDone = e1 >= 100; break;
                case "speed": IsDone = e2 >= 100; break;
                case "dressage": IsDone = e3 >= 100; break;
                case "galop": IsDone = e4 >= 100; break;
                case "lynx": IsDone = e5 >= 100; break;
                case "jump": IsDone = e6 >= 100; break;
            }
        }

        private void LoadSkill(string script, string varName)
        {
            double num = Convert.ToDouble(Regex.Match(script, $"var {varName} = (.*?);").Groups[1].Value);
            IsDone = num >= 100;
        }

        private void LoadCompet(IElement a)
        {
            string html = a.GetAttribute("_tooltip");
            var document = Parser.ParseDocument(html);
            var strong = document.QuerySelectorAll("strong");
            IsDone = !(strong.Length > 0);
        }

        public object Clone()
        {
            return new Training(Name, Value)
            {
                IsDone = IsDone
            };
        }
    }
}
