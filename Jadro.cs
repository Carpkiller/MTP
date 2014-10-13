using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTP
{
    public class Jadro
    {
        private string[] slovnik;
        private HashSet<string> pomSlovnik; 
        private HashSet<string> trojPismenneSlova;
        public string[] input { get; set; }
        public Jadro()
        {
            slovnik = File.ReadAllLines("slovnik.txt");
            pomSlovnik = new HashSet<string>();
            foreach (var slovo in slovnik)
            {
                pomSlovnik.Add(slovo.ToUpper());
            }
            trojPismenneSlova = ParsujSlova();
        }

        private HashSet<string> ParsujSlova()
        {
            var outputList = new HashSet<string>();
            foreach (var slovo in slovnik)
            {
                var pomList = RozdelSlovo(slovo, 3);
                foreach (var pomSlovo in pomList)
                {
                    if (!outputList.Contains(pomSlovo))
                    {
                        outputList.Add(pomSlovo);
                    }
                }
            }

            return outputList;
        }

        private string HexToChar(int hex)
        {
            return Char.ConvertFromUtf32(hex);
        }

        private string HexToString(string hex)
        {
            var output = "";

            for (int i = 0; i < hex.Length/2; i++)
            {
                output += HexToChar(Convert.ToInt32(hex.Substring(i*2, 2),16));
            }
            return output;
        }

        public string StringToHex(string text)
        {
            var output = "";
            foreach (char letter in text)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = String.Format("{0:X}", value);
                output += hexOutput;
            }
            return output;
        }

        private string Xor(string value1, string value2)
        {
            var kratsi = value1.Length > value2.Length ? value2 : value1;
            var dlhsi = kratsi == value1 ? value2 : value1;
            var output = "";
            for (int i = 0; i < kratsi.Length/2;i++)
            {
                var value = (Convert.ToInt32(kratsi.Substring(i * 2, 2),16) ^ Convert.ToInt32(dlhsi.Substring(i * 2, 2),16)).ToString("X");
                output += value.Length == 1 ? value.Insert(1, "0") : value;
            }
            return output;
        }

        private void NacitajTexty(string text)
        {
            text = text.Replace("\r", null);
            input = text.Split('\n').Where(x => x.Length > 1).ToArray();
        }

        public void PoctajSifru(string text)
        {
            NacitajTexty(text);

            var listKlucov = new List<string>(){""};
            var listpomKlucov = new List<string>();
            var najkratsi = input.Min(x => x.Length);

            for (int i = 0; i < najkratsi/2; i++)
            {
                for (int j = 0x00; j <= 0x0f; j++)
                {
                    for (int jj = 0x00; jj <= 0x0f; jj++)
                    {
                        var pripona = j.ToString("X") + jj.ToString("X");
                        var max = 0;
                        for (int k = 0; k < listKlucov.Count; k++)
                        {
                            var list = new List<string>();
                            var tet = input[0].Substring(i*2, 2);
                            var xor = Xor(tet, pripona);
                            var odpoved = HexToChar(Convert.ToInt32(xor, 16));
                            list.Add(Xor(input[0].Substring(0,i*2+2),listKlucov[k] + pripona));
                            if (Regex.IsMatch(odpoved, @"^[a-zA-Z '.,()-@!%;:]+$"))
                            {
                                var pom = 1;
                                for (int ii = 1; ii < input.Length; ii++)
                                {
                                    tet = input[ii].Substring(i*2, 2);
                                    xor = Xor(tet, pripona);
                                    odpoved = HexToChar(Convert.ToInt32(xor, 16));
                                    if (Regex.IsMatch(odpoved, @"^[a-zA-Z '.,()-@!%;:]+$"))
                                    {
                                        pom++;
                                        list.Add(Xor(input[ii].Substring(0,i*2+2),listKlucov[k] + pripona));
                                    }
                                }
                                if (pom > max)
                                {
                                    max = pom;
                                }
                                if (pom >= input.Length - 1)
                                {
                                    if (i > 1 && NeobsahujeSprostosti(listKlucov[k] + pripona))
                                    {
                                        listpomKlucov.Add(listKlucov[k] + pripona);
                                        if (i == 25)
                                        {
                                            //Console.WriteLine(listKlucov[k] + pripona + "  " + VypisList(list));
                                        }
                                    }
                                    else if (i <= 1 && PrvyZnak(list))
                                    {
                                        listpomKlucov.Add(listKlucov[k] + pripona);
                                        //Console.WriteLine(listKlucov[k] + pripona + "  " + VypisList(list));
                                    }
                                    
                                }

                                if (listKlucov[k] + pripona == "6619698EC3D7")
                                {
                                    VypisList(list);
                                }
                            }
                        }
                        
                    }
                }
                Console.WriteLine(i + " - " + listpomKlucov.Count);
                listKlucov.Clear();
                if (listpomKlucov.Any(x => x.StartsWith("66396E89C9DBD8CC")))
                {
                    //Console.WriteLine("    "+i + "  -  "+ listpomKlucov.Where(x => x.StartsWith("66396E89C9DBD8CC")).Select(x =>x).Count());
                }
                if (listpomKlucov.Count > 500)
                {
                    listpomKlucov = SkontrolujSlova(listpomKlucov);
                }
                listKlucov.AddRange(listpomKlucov);
                listpomKlucov.Clear();

                
            }

            for (int i = 0; i < listKlucov.Count; i++)
            {
                //Console.WriteLine(listKlucov[i] + "  " + VypisPodlaHesla(listKlucov[i]));
            }
        }

        private string VypisPodlaHesla(string heslo)
        {
            var list = new List<string>();
            var output = heslo + " : \n";
            for (int i = 0; i < input.Length; i++)
            {
                var tet = input[i].Substring(0, heslo.Length);
                var xor = Xor(tet, heslo);
                var odpoved = HexToString(xor);

                list.Add(odpoved+"\n");
            }

            foreach (var item in list)
            {
                output += item;
            }

            return output;
        }

        private List<string> SkontrolujSlova(List<string> listpomKlucov)
        {
            var output = new List<string>();
            var pomList = new List<Hodnotenie>();

            foreach (var pomKluc in listpomKlucov)
            {
                if (pomKluc.StartsWith("66396E89C9DBD8CC"))
                {
                    //Console.WriteLine(pomKluc);
                }
                int hodn = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    var tet = input[i].Substring(0, pomKluc.Length);
                    var xor = Xor(tet, pomKluc);
                    var odpoved = HexToString(xor);

                    var slova = odpoved.Split(' ').ToList();
                    for (int j = 0; j < slova.Count; j++)
                    {
                        slova[j] = slova[j].ToUpper();
                    }

                    foreach (var slovo in slova)
                    {
                        if (pomSlovnik.Contains(slovo))
                        {
                            hodn++;
                        }
                    }
                    
                }
                if (pomKluc.StartsWith("66396E89C9DBD8"))
                {
                    //Console.WriteLine(hodn +"  - "+pomKluc);
                }
                pomList.Add(new Hodnotenie(pomKluc,hodn));
            }

            var epomlist = pomList.OrderByDescending(x => x.hodnota).ToList();
            Console.WriteLine(VypisPodlaHesla(epomlist.First().kluc));
            output = pomList.OrderByDescending(x => x.hodnota).Select(x => x.kluc).Take(100).ToList();
            return output;
        }

        private bool NeobsahujeSprostosti(string kluc)
        {
            List<string> pomList = new List<string>();
            List<string> NevhodneSlova = new List<string>()
            {
                "  ",
                "''",
                ".,",
                ",.",
                "--",
                "..",
                ",,",
                "g-"
            };

            for (int i = 0; i < input.Length; i++)
            {
                var tet = input[i].Substring(0, kluc.Length);
                var xor = Xor(tet, kluc);
                var odpoved = HexToString(xor);


                if (odpoved.Length > 2)
                {
                    pomList = RozdelSlovo(odpoved,2);
                }

                if (NevhodneSlova.Any(pomList.Contains))
                {
                    //Console.WriteLine("Kluc zmazany");
                    return false;
                }
                if (odpoved.Length > 3)
                {
                    pomList = RozdelSlovo(odpoved, 3);
                    if (!trojPismenneSlova.Any(pomList.Contains))
                    {
                       // Console.WriteLine("Kluc zmazany");
                        return false;
                    }
                    if (ObsahujeDveVelke(odpoved))
                    {
                        //Console.WriteLine("Kluc zmazany");
                        return false;
                    }
                }

                return true;
            }

            return true;
        }

        private bool ObsahujeDveVelke(string odpoved)
        {
            for (int i = 0; i < odpoved.Length-1; i++)
            {
                if (Regex.IsMatch(odpoved[i].ToString(), @"^[A-Z]+$") && Regex.IsMatch(odpoved[i+1].ToString(), @"^[A-Z]+$"))
                {
                    return true;
                }
                if (Regex.IsMatch(odpoved[i].ToString(), @"^[a-z]+$") && Regex.IsMatch(odpoved[i + 1].ToString(), @"^[A-Z]+$"))
                {
                    return true;
                }
                if (Regex.IsMatch(odpoved[i].ToString(), @"^[A-Z]+$") && odpoved[i + 1].ToString() == "'")
                {
                    return true;
                }
                if (Regex.IsMatch(odpoved[i].ToString(), @"^[']+$") && Regex.IsMatch(odpoved[i + 1].ToString(), @"^[A-Z]+$"))
                {
                    return true;
                }
            }
            return false;
        }


        private List<string> RozdelSlovo(string odpoved, int pocet)
        {
            var output = new List<string>();
            for (int i = 0; i < odpoved.Length-(pocet-1); i++)
            {
                output.Add(odpoved.Substring(i,pocet));
            }

            return output;
        }

        private bool PrvyZnak(List<string> list)
        {
            foreach (var item in list)
            {
                if (!Regex.IsMatch(HexToString(item)[0].ToString(), @"^[A-Z ]+$"))
                {
                    return false;
                }
            }

            return true;
        }

        private string VypisList(List<string> list)
        {
            var output = "";
            foreach (var item in list)
            {
                var text = HexToString(item);
                output +=  text + " ";
            }

            return output;
        }
    }
}
