using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ShopThreads.ShopLogic
{
    public class Shop
    {
        private Random rnd;

        private Timer timer;

        private List<Cashier> cashiers;

        private int bigCustomerCounter = 0;

        private int bigCashierCounter = 0;

        public Shop(int cashierQuantity)
        {
            rnd = new Random();
            timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            cashiers = new List<Cashier>();
            for (int i = 0; i < cashierQuantity; i++)
            {
                cashiers.Add(new Cashier("CashierThread"+bigCashierCounter, rnd));
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (rnd.Next(1, 101) < 81)//80% chance a customer will come to the shop
            {
                //give the random casher a new customer
                cashiers[rnd.Next(0, cashiers.Count)].AddCustomer("CustomerThread"+ ++bigCustomerCounter, 1000 * rnd.Next(3, 8));
            }
            ConsolePrint();
        }

        public void ConsolePrint()
        {
            Console.Clear();
            for (int i = 0; i<cashiers.Count; i++)
            {
                Console.WriteLine("In line {0, -10} cashier {1,-15} {2,5} served", cashiers[i].EvaitingQuantity, i, cashiers[i].ServedQuantity);
            }
            Console.WriteLine("Total quantity of ALL current and ex customers {0}",bigCustomerCounter);
        }
    }
}
