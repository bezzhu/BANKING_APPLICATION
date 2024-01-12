using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace BANKING_APPLICATION
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Validate validate;
        private static List<User> userList;

        static void Main()
        {
            LoadUserListFromJsonFile();
            validate = new Validate(){ UserList = userList };
            try
            {
                ApplicationStart();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while initializing the application.");
            }
        }

        private static void ApplicationStart()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Enter Card Details :");

            Console.WriteLine("1.Card number");
            var cardNumber = Console.ReadLine();

            Console.WriteLine("2.CVC");
            var cvc = Console.ReadLine();

            Console.WriteLine("3.Expiration Date");
            var expirationDate = Console.ReadLine();

            if (!validate.CardValidate(cardNumber, cvc, expirationDate))
            {
                Console.WriteLine("Please Provide Correct Data");
                ApplicationStart();
            }
            else
            {

                Console.WriteLine("Enter Pin");

                if (!validate.PinCodeValidate(Console.ReadLine()))
                {
                    Console.WriteLine("Please Provide Correct Pin");
                    ApplicationStart();
                }
            }
            MenuOptionPage();
        }
        private static void MenuOptionPage()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Hello {validate.User.FirstName} {validate.User.LastName}:");
            foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            var userInput = Console.ReadLine();
            Enum.TryParse(userInput, ignoreCase: true, out MenuOption selectedOption);


            switch (selectedOption)
            {
                case MenuOption.CheckDeposit:
                    CheckDeposit();
                    break;

                case MenuOption.GetAmount:
                    GetAmount();
                    break;

                case MenuOption.GetLast5Transactions:
                    GetLast_5_Transaction();
                    break;

                case MenuOption.AddAmount:
                    AddAmount();
                    break;

                case MenuOption.ChangePIN:
                    ChangePIN();
                    break;

                case MenuOption.ChangeAmount:

                    ChangeAmount();
                    break;

                default:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
            ApplicationStart();
        }
        private static void LoadUserListFromJsonFile()
        {
            try
            {
                string jsonFilePath = @"C:\Users\LENOVO\Desktop\BANKING_APPLICATION\BANKING_APPLICATION\users.json";

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    userList = JsonConvert.DeserializeObject<List<User>>(jsonContent);
                }
                else
                {
                    userList = new List<User>();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while DeserializeObject");
            }
           
        }
        private static void UpdateJsonFile(List<User> userList)
        {
            try
            {
                string jsonFilePath = @"C:\Users\LENOVO\Desktop\BANKING_APPLICATION\BANKING_APPLICATION\users.json";
                string updatedJsonContent = JsonConvert.SerializeObject(userList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while SerializeObject");

            }
        }
       
        public enum MenuOption
        {
            CheckDeposit = 1,
            GetAmount,
            GetLast5Transactions,
            AddAmount,
            ChangePIN,
            ChangeAmount
        }
        public enum ValutaOption
        {
            GEL = 1,
            EUR,
            USD,
        }
       
        private static TransactionHistory LastTransaction()
        {
            var transactionHistory = validate.User.TransactionHistory;

            TransactionHistory lastTransaction;

            if (transactionHistory.Count == 0)
            {
                lastTransaction = new TransactionHistory()
                {
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = "",
                    AmountGEL = 0,
                    AmountUSD = 0,
                    AmountEUR = 0
                };
            }else
            {
                lastTransaction = transactionHistory.Last();
            }
            
            return lastTransaction;
        }
        private static void CheckDeposit()
        {
            var lastTransaction = LastTransaction();
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Amount is : ");
            Console.WriteLine($"GEL : {lastTransaction.AmountGEL}");
            Console.WriteLine($"EUR : {lastTransaction.AmountEUR}");
            Console.WriteLine($"USD : {lastTransaction.AmountUSD}");

            var newTransaction = new TransactionHistory()
            {
                TransactionDate = DateTime.Now,
                TransactionType = "CheckDeposit",
                AmountGEL = lastTransaction.AmountGEL,
                AmountEUR = lastTransaction.AmountEUR,
                AmountUSD = lastTransaction.AmountUSD,
            };
            
            validate.User.TransactionHistory.Add(newTransaction);
            UpdateJsonFile(validate.UserList);
        }
        private static void ChangePIN()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Enter new pinCode");
            validate.User.PinCode = Console.ReadLine();
            UpdateJsonFile(validate.UserList);
            Console.WriteLine("Your pinCode has been changed");
        }
        private static void AddAmount()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Choose Valuta: ");

            foreach (ValutaOption option in Enum.GetValues(typeof(ValutaOption)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            var valuta = Console.ReadLine();

            if (!Enum.TryParse(valuta, ignoreCase: true, out ValutaOption selectedOption))
            {
                Console.WriteLine("Invalid Valuta option.");
                return;
            }

            Console.WriteLine("Enter Amount");

            if (!int.TryParse(Console.ReadLine(), out int amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }
            var lastTransaction = LastTransaction();

            var newTransaction = new TransactionHistory()
            {
                TransactionDate = DateTime.Now,
                TransactionType = "AddAmount",
                AmountGEL = 0,
                AmountEUR = 0,
                AmountUSD = 0,
            };
            switch (selectedOption)
            {
                case ValutaOption.GEL:
                    newTransaction.AmountGEL = lastTransaction.AmountGEL + amount;
                    break;
                case ValutaOption.EUR:
                    newTransaction.AmountEUR = lastTransaction.AmountEUR + amount;
                    break;
                case ValutaOption.USD:
                    newTransaction.AmountUSD = lastTransaction.AmountUSD + amount;
                    break;
                default:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
 
            validate.User.TransactionHistory.Add(newTransaction);
            UpdateJsonFile(validate.UserList);
            Console.WriteLine("Your amount has been added");
        }
        private static void GetLast_5_Transaction()
        {
            var transactionHistory = validate.User.TransactionHistory;

            if (transactionHistory.Count > 0)
            {
                int startIndex = Math.Max(0, transactionHistory.Count - 5);
                int endIndex = transactionHistory.Count;

                Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("Last 5 Transactions:");

                for (int i = startIndex; i < endIndex; i++)
                {
                    var transaction = transactionHistory[i];
                    Console.WriteLine($"Transaction Date: {transaction.TransactionDate}");
                    Console.WriteLine($"TransactionType: {transaction.TransactionType}");
                    Console.WriteLine($"AmountGEL: {transaction.AmountGEL}");
                    Console.WriteLine($"AmountEUR: {transaction.AmountEUR}");
                    Console.WriteLine($"AmountUSD: {transaction.AmountUSD}");
                    Console.WriteLine();

                }
            }
            else
            {
                Console.WriteLine("There are no transactions in the history.");
            }

        }
        private static void ChangeAmount()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Choose Valuta: ");

            foreach (ValutaOption option in Enum.GetValues(typeof(ValutaOption)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            var valuta = Console.ReadLine();

            if (!Enum.TryParse(valuta, ignoreCase: true, out ValutaOption selectedOption))
            {
                Console.WriteLine("Invalid Valuta option.");
                return;
            }

            Console.WriteLine("Enter Amount");

            if (!int.TryParse(Console.ReadLine(), out int amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }
            var lastTransaction = LastTransaction();
            var newTransaction = new TransactionHistory()
            {
                TransactionDate = DateTime.Now,
                TransactionType = "ChangeAmount",
                AmountGEL = 0,
                AmountEUR = 0,
                AmountUSD = 0,
            };
            switch (selectedOption)
            {
                case ValutaOption.GEL:
                    if (lastTransaction.AmountGEL >= amount)
                    {
                        newTransaction.AmountGEL = lastTransaction.AmountGEL - amount;
                    }
                    else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                case ValutaOption.EUR:
                    if (lastTransaction.AmountEUR >= amount)
                    {
                        newTransaction.AmountEUR = lastTransaction.AmountEUR - amount;
                    }
                    else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                case ValutaOption.USD:
                    if (lastTransaction.AmountUSD >= amount)
                    {
                        newTransaction.AmountUSD = lastTransaction.AmountUSD - amount;
                    }
                    else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
            Console.WriteLine("Choose Valuta: ");

            foreach (ValutaOption option in Enum.GetValues(typeof(ValutaOption)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            var valuta2 = Console.ReadLine();

            if (!Enum.TryParse(valuta2, ignoreCase: true, out ValutaOption selectedOption2))
            {
                Console.WriteLine("Invalid Valuta option.");
                return;
            }

            switch (selectedOption2)
            {
                case ValutaOption.GEL:
                    newTransaction.AmountGEL = lastTransaction.AmountGEL + amount;
                    break;
                case ValutaOption.EUR:
                    newTransaction.AmountEUR = lastTransaction.AmountEUR + amount;
                    break;
                case ValutaOption.USD:
                    newTransaction.AmountUSD = lastTransaction.AmountUSD + amount;
                    break;
                default:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
  
            validate.User.TransactionHistory.Add(newTransaction);
            UpdateJsonFile(validate.UserList);
            Console.WriteLine("Your amount has been changed");
        }
        private static void GetAmount()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Choose Valuta: ");

            foreach (ValutaOption option in Enum.GetValues(typeof(ValutaOption)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            var valuta = Console.ReadLine();

            if (!Enum.TryParse(valuta, ignoreCase: true, out ValutaOption selectedOption))
            {
                Console.WriteLine("Invalid Valuta option.");
                return;
            }

            Console.WriteLine("Enter Amount");

            if (!int.TryParse(Console.ReadLine(), out int amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }
            var lastTransaction = LastTransaction();
            var newTransaction = new TransactionHistory()
            {
                TransactionDate = DateTime.Now,
                TransactionType = "GetAmount",
                AmountGEL = 0,
                AmountEUR = 0,
                AmountUSD = 0,
            };
            switch (selectedOption)
            {
                case ValutaOption.GEL:
                    if (lastTransaction.AmountGEL >= amount)
                    {
                        newTransaction.AmountGEL = lastTransaction.AmountGEL - amount;
                    }else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                case ValutaOption.EUR:
                    if (lastTransaction.AmountEUR >= amount)
                    {
                        newTransaction.AmountEUR = lastTransaction.AmountEUR - amount;
                    }
                    else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                case ValutaOption.USD:
                    if (lastTransaction.AmountUSD >= amount)
                    {
                        newTransaction.AmountUSD = lastTransaction.AmountUSD - amount;
                    }
                    else
                    {
                        Console.WriteLine("There is not enough money.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
        
            validate.User.TransactionHistory.Add(newTransaction);
            UpdateJsonFile(validate.UserList);
            Console.WriteLine("Withdrawal completed successfully");
        }
        
    }
}
