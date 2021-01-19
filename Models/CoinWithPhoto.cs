using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace EuroCollection.Models
{
    public class CoinWithPhoto
    {
        public string CoinName { get; set; }

        public int Denomination { get; set; }

        public int Year { get; set; }

        public string Country { get; set; }
        public double Value { get; set; }
        public string Photo { get; set; }
        public string Type { get; set; }
    }
}
