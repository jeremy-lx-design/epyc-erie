using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epyc_erie
{
    internal class item
    {
        string name;
        int qte;
        double prix;

        public item(string name, int qte, double prix)
        {
            this.name = name;
            this.qte = qte;
            this.prix = prix;
        }

        public string Name { get => name; set => name = value; }
        public int Qte { get => qte; set => qte = value; }
        public double Prix { get => prix; set => prix = value; }
    }
}
