using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTP
{
    public class Hodnotenie
    {
        public string kluc { get; set; }
        public int hodnota { get; set; }

        public Hodnotenie(string kluc, int hodnota)
        {
            this.kluc = kluc;
            this.hodnota = hodnota;
        }

    }
}
