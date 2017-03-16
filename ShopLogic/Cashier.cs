using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ShopThreads.ShopLogic
{
    class Cashier
    {
        private EventWaitHandle cashierWaitHandler;

        private Thread thread;

        private List<Customer> customers;

        private Queue<Customer> waitingCustomers;

        private Random rnd;
         
        private int evaitingQuantity;

        private int servedQuantity;

        public int EvaitingQuantity { get
            {
                return evaitingQuantity;
            }
        }

        public int ServedQuantity
        {
            get
            {
                return servedQuantity;
            }
        }

        public Cashier(string cashierThreadName, Random rand)
        {
            customers = new List<Customer>();
            waitingCustomers = new Queue<Customer>();
            rnd = rand;
            evaitingQuantity = 0;
           
            cashierWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);
            thread = new Thread(Work);
            thread.Name = cashierThreadName;
            thread.Start();
        }

        private void Work()
        {
            while (true)
            {
                cashierWaitHandler.WaitOne();

                while (waitingCustomers.Count > 0)
                {
                    Customer customer = waitingCustomers.Dequeue();
                    //taking customer's waitHandler and sending it a signal
                    customer.customerWaitHandler.Set();
                    //waiting
                    Thread.Sleep(1000 * rnd.Next(3,8));
                    //deleting customer from list
                    customers.Remove(customer);
                }
            }
        }

        public void AddCustomer(string customerThreadName, int customerSleepTime)
        {
            Customer customer = new Customer(
                ()=>
                {
                    Interlocked.Increment(ref evaitingQuantity);//thread-safe increment
                },
                () => {
                    Interlocked.Decrement(ref evaitingQuantity);//thread-safe decrement
                    Interlocked.Increment(ref servedQuantity);//thread-safe increment
                }, 
                (cust) => 
                {
                    //locking just for the sake of safety
                    object lockItem = new object();
                    lock (lockItem)
                    {
                        waitingCustomers.Enqueue(cust);
                    }
                },
                customerSleepTime, customerThreadName, cashierWaitHandler);
            customers.Add(customer);
        }
    }
}
