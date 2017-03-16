using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ShopThreads.ShopLogic
{
    class Customer
    {
        public EventWaitHandle customerWaitHandler;

        private EventWaitHandle cashierWaitHandler;

        private Action<Customer> HowToPay;

        private Action HowToStart;

        private Action HowToFinish;

        private Thread thread;

        private int timeTosleep;

        public Customer(Action start, Action end, Action<Customer> whatToDo, int timeTosleep, string threadName, EventWaitHandle handle)
        {
            customerWaitHandler = new EventWaitHandle(false, EventResetMode.ManualReset);

            cashierWaitHandler = handle;

            HowToPay = whatToDo;
            HowToStart = start;
            HowToFinish = end;

            this.timeTosleep = timeTosleep;

            thread = new Thread(Buy);
            thread.Name = threadName;
            thread.Start();
        }

        private void Buy()
        {
            HowToStart();
            Thread.Sleep(timeTosleep);

            Pay();
        }

        private void Pay()
        {
            HowToPay(this);
            cashierWaitHandler.Set();//signals that this customer is getting in line

            customerWaitHandler.WaitOne();//waits in line
            HowToFinish();
        }
    }
}
