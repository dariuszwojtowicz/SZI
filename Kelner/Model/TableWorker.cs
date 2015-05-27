namespace Kelner.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class TableWorker
    {
        private Thread thread;

        private bool shouldStop;

        public TableWorker(int x, int y, int tableNumber)
        {
            this.thread = new Thread(this.TableWorkerFunc);
            this.IsFree = true;
            this.IsDirty = false;
            this.Number = tableNumber;

            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        
        public int Y { get; set; }

        public bool IsDirty { get; set; }

        public int Number { get; set; }

        public Order Order { get; set; }

        public bool IsFree { get; set; }

        public void CreateNewOrder()
        {
            this.Order = new Order
            {
                TableNumber = this.Number,
                OrderState = Order.State.New
            };
        }

        public void Start()
        {
            this.thread = new Thread(this.TableWorkerFunc);
            this.shouldStop = false;
            this.thread.Start();
        }

        public void Stop()
        {
            this.shouldStop = true;
            Thread.Sleep(20);
            this.thread.Join();
        }

        public event NewTableOrderEventHandler NewTableOrderEvent;

        protected virtual void OnNewTableOrder(NewTableOrderEventArgs e)
        {
            this.NewTableOrderEvent(this, e);
        }

        public event ClientOutEventHandler ClientOutEvent;

        protected virtual void OnClientOutEvent(ClientOutEventArgs e)
        {
            this.ClientOutEvent(this, e);
        }

        private void TableWorkerFunc()
        {
            while (!this.shouldStop)
            {
                if (!this.IsFree)
                {
                    Thread.Sleep(1000);

                    if (this.Order == null)
                    {
                        Thread.Sleep(1000);
                        this.CreateNewOrder();
                        NewTableOrderEventArgs e = new NewTableOrderEventArgs(this.X, this.Y, this.Number);
                        this.OnNewTableOrder(e);
                    }
                    else
                    {
                        if (this.Order.OrderState == Order.State.Done)
                        {
                            Thread.Sleep(3000);

                            this.IsDirty = true;
                            this.IsFree = true;
                            this.Order = null;
                            ClientOutEventArgs e = new ClientOutEventArgs(this.X, this.Y, this.Number);
                            this.OnClientOutEvent(e);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Zdarzenie nowego zamówienia przez klientów przy stoliku
    /// </summary>
    public delegate void NewTableOrderEventHandler(object sender, NewTableOrderEventArgs e);

    public class NewTableOrderEventArgs
    {
        public int TableX { get; set; }

        public int TableY { get; set; }

        public int TableNumber { get; set; }

        public NewTableOrderEventArgs(int x, int y, int tableNumber)
        {
            this.TableX = x;
            this.TableY = y;
            this.TableNumber = tableNumber;
        }

    }

    /// <summary>
    /// Zdarzenie zjedzenia zamówienia i wyjścia klienta z restauracji
    /// </summary>
    public delegate void ClientOutEventHandler(object sender, ClientOutEventArgs e);

    public class ClientOutEventArgs
    {
        public int TableX { get; set; }

        public int TableY { get; set; }

        public int TableNumber { get; set; }

        public ClientOutEventArgs(int x, int y, int tableNumber)
        {
            this.TableX = x;
            this.TableY = y;
            this.TableNumber = tableNumber;
        }

    }

    //todo: metoda zwracająca róg stołu do któergo ma podejść kelner
}
