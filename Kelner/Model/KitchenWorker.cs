namespace Kelner.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Klasa reprezentująca kuchnie i to co się w niej dzieje z zamówieniami
    /// </summary>
    public class KitchenWorker
    {
        private Thread thread;

        private bool shouldStop;

        public KitchenWorker()
        {
            this.thread = new Thread(this.KitchenWorkerFunc);
            this.DoneOrders = new List<Order>();
            this.OrdersInProgress = new List<Order>();
        }

        /// <summary>
        /// Lista gotowych posiłków do wydania
        /// </summary>
        public List<Order> DoneOrders { get; set; }

        /// <summary>
        /// Zamówienia złożone przez kelnera w kuchni i właśnie robione
        /// </summary>
        public List<Order> OrdersInProgress { get; set; }

        public void Start()
        {
            this.DoneOrders = new List<Order>();
            this.OrdersInProgress = new List<Order>();
            this.thread = new Thread(this.KitchenWorkerFunc);
            this.shouldStop = false;
            this.thread.Start();
        }

        public void Stop()
        {
            this.shouldStop = true;
            Thread.Sleep(20);
            this.thread.Join();
        }

        public int AddNewOrders(List<Order> newOrders)
        {
            this.OrdersInProgress.AddRange(newOrders);
            return this.OrdersInProgress.Count;
        }

        public event NewMealEventHandler NewMealEvent;

        protected virtual void OnNewMealAppeared(NewMealEventArgs e)
        {
            this.NewMealEvent(this, e);
        }

        /// <summary>
        /// Metoda "pracy" kuchni 
        /// </summary>
        private void KitchenWorkerFunc()
        {
            while (!this.shouldStop)
            {
                try
                {
                    if (this.OrdersInProgress.Any())
                    {
                        Thread.Sleep(1000);
                        var doneOrder = this.OrdersInProgress.FirstOrDefault();

                        if (doneOrder != null)
                        {
                            this.DoneOrders.Add(doneOrder);
                            this.OrdersInProgress.Remove(doneOrder);
                            var doneOrdersCount = this.DoneOrders.Count;
                            var ordersInProgressCount = this.OrdersInProgress.Count;
                            NewMealEventArgs e = new NewMealEventArgs(doneOrdersCount, ordersInProgressCount);
                            this.OnNewMealAppeared(e);
                            this.DoneOrders = new List<Order>();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }

    public delegate void NewMealEventHandler(object sender, NewMealEventArgs e);

    public class NewMealEventArgs
    {
        public NewMealEventArgs(int doneOrdersCount, int ordersInProgressCount)
        {
            this.DoneMealCount = doneOrdersCount;
            this.OrdersInProgressCount = ordersInProgressCount;
        }

        /// <summary>
        /// Licznik wydanych posiłków, który zostanie wzrócony kelnerowi jako argument dla eventu
        /// </summary>
        public int DoneMealCount { get; set; }

        /// <summary>
        /// Licznik posiłków w trakcie robienia
        /// </summary>
        public int OrdersInProgressCount { get; set; }
    }
}

