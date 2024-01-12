using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BANKING_APPLICATION
{
    internal class Validate
    {
        public List<User> UserList { get; set; }
        public User  User { get; set; }
        public bool CardValidate(string cardNumber, string cvc, string expirationDate)
        {
            var matchingUser = UserList.FirstOrDefault(user =>
                user.CardDetails.CardNumber.Equals(cardNumber) &&
                user.CardDetails.CVC.Equals(cvc) &&
                user.CardDetails.ExpirationDate.Equals(expirationDate));

            if (matchingUser != null)
            {
                User = matchingUser;
                return true;
            }

            return false;
        }
        public bool PinCodeValidate(string pinCode)
        { 
            return User.PinCode.Equals(pinCode);
        }
    }
}
