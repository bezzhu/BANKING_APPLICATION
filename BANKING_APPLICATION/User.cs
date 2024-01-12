using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANKING_APPLICATION
{
    internal class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public CardDetails CardDetails { get; set; }
        public string PinCode { get; set; }
        public List<TransactionHistory> TransactionHistory { get; set; }


    }
}
