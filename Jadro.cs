using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTP
{
    public class Jadro
    {
        private string[] slovnik; 
        public string[] input { get; set; }
        public Jadro()
        {
            slovnik = File.ReadAllLines("slovnik.txt");
        }

        private string HexToString(int hex)
        {
            return hex.ToString("X");
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
            var kratsi = value1.Length > value2.Length ? value1 : value2;
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
        }
    }
}
