using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogecoinBlockExplorer;
using System.IO;

namespace CoinFrameTest
{
    class Program
    {
        public static List<string> lines = new List<string>();
        public static decimal value;
        public static string currentaddress;
        public static int addresses_attempted = 0;
        public static int received_amount = 0;
        public static DogecoinRestExplorer explorer = new DogecoinRestExplorer();
        public static decimal lowest_value = 0.00001m;
        public static List<string> history = new List<string>();
        public static List<string> balances = new List<string>();
        public static string checkingfile = "addresses.txt";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the great address and balance checker");
            Console.WriteLine("This program/utility will for you today scan doge addresses on the blockchain");
            Console.WriteLine("By default addresses with a receive history of more than 0.00001 DOGE will be saved in history.txt");
            Console.WriteLine("Respectively, addresses that have a balance will be saved in balances.txt");
            Console.WriteLine("If you want to specify a file other than addresses type the filename.txt, it must be in the same exact location as this executable!");
            Console.WriteLine("For nothing just press enter, default 'addresses.txt' will be used");
            string alternativefile = Console.ReadLine();
            if (alternativefile.Contains(".txt")) 
            { Console.WriteLine("Assuming you wanted to read the file: '" + alternativefile + "', loading it now.");
                checkingfile = alternativefile;
            }

            lines = File.ReadAllLines(checkingfile).ToList<String>();
            Console.WriteLine("File by name of " + checkingfile + "loaded to be read");
            Console.WriteLine("Please note: This will require the format produced by VanityKeyGen, where the formatting appears like so:");
            Console.WriteLine("DOGE Address: DHACKmEgpUByp6tw6ebAm1Sq3KwBkf8T8b");
            Console.WriteLine("Press return/enter one more time to start! ");
            Console.ReadLine();

            Console.Clear();
            Console.CursorVisible = false;

            foreach (string s in lines)
            {
                
                if (s.Contains("DOGE Address:"))
                {
                    string[] results = s.Split(' ');
                    currentaddress = results[2];
                    addresses_attempted += 1;
                    try{


                        var mytask = await explorer.Balance(currentaddress);
                        if (mytask != null)
                        {
                            value = value + mytask.Balance;
                            if (mytask.Balance > lowest_value) { balances.Add(currentaddress); }
                        }
                        else
                        {
                            Console.WriteLine("The task was null cannot continue! We must be at end of file.");
                            Console.WriteLine("Press enter to exit!!, results will be saved to balances.txt and history.txt respectively!");
                            Console.ReadLine();
                            File.WriteAllLines("history.txt", history.ToArray());
                            File.WriteAllLines("balances.txt", balances.ToArray());

                            Environment.Exit(0);
                            
                        }

                        var task2 = await explorer.AmountReceived(currentaddress);
                        if (task2 != null)
                        {
                            decimal received_ = task2.Received;
                            if (received_ > lowest_value)
                            {
                                history.Add(currentaddress);
                                received_amount += 1;
                            }
                        }
                       
                        
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("                                                                                          ");
                        Console.WriteLine("                                                                                          ");
                        Console.WriteLine("                                                                                          ");
                        Console.WriteLine("                                                                                          ");
                        Console.SetCursorPosition(0, 0);


                        Console.WriteLine("Addresses tried: " + addresses_attempted);
                        Console.WriteLine("Last address attempted: " + currentaddress);
                        Console.WriteLine("Total Value in Doge So Far " + value);
                        Console.WriteLine("Addresses with received amounts total: " + received_amount);
                    }
                    catch
                    {
                        Console.WriteLine("Ran out of lines or something went wrong.");
                        Console.WriteLine("Press enter or return key to exit now..");
                        Console.ReadLine();
                    }
                   


                   
                }

                
            }
            Console.WriteLine("Balance was " + value.ToString() + " " + "Press any key to continue");
            Console.ReadLine();
        }
    }
}
