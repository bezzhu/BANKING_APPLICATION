using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANKING_APPLICATION
{
    internal class TransactionHistory
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public int AmountGEL { get; set; }
        public int AmountEUR { get; set; }
        public int AmountUSD { get; set; }
    }
}
