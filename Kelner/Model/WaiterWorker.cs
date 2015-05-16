namespace Kelner.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Kelner.Algorithm;
    using Kelner.ViewModel;

    /// <summary>
    /// Klasa reprezentująca obiekt Kelnera, która będzie zbierać informacje o tym co się dzieje w restauracji
    /// </summary>
    public partial class WaiterWorker : ViewModelBase
    {
        static int readerTimeouts = 0;
        static int writerTimeouts = 0;
        static int reads = 0;
        static int writes = 0;

        private TreeNode clientsQueueTreeRoot;

        private KitchenWorker kitchenWorker;

        private ClientsQueueWorker clientsQueueWorker;

        private TableWorker tableWorker;

        private int mealsOnKitchen;

        private int doneOrdersOnKitchen;

        private int clientsQueueCount;

        private List<Order> writtenOrders;

        private List<Order> doneOrdersOnHands;

        private int ordersOnTables;

        private static ReaderWriterLock rwl;

        public WaiterWorker()
        {
            rwl = new ReaderWriterLock();
            this.WrittenOrders = new List<Order>();
            this.DoneOrdersOnHands = new List<Order>();

            this.kitchenWorker = new KitchenWorker();
            this.kitchenWorker.NewMealEvent += this.OnNewMealFromKitchenWorker;

            this.clientsQueueWorker = new ClientsQueueWorker();
            this.clientsQueueWorker.NewClientEvent += this.OnNewClientCome;

            this.tableWorker = new TableWorker();
            this.tableWorker.ClientOutEvent += this.OnClientOut;
            this.tableWorker.NewTableOrderEvent += this.OnNewTableOrder;

            this.RestaurantSections = new Section[10][];

            for (int i = 0; i < 10; i++)
            {
                this.RestaurantSections[i] = new Section[10];
            }

            this.State = new State
            {
                X = 5,
                Y = 5,
                Direction = Direction.South
            };

            this.CreateSections();


            DecisionTreeImplementation sam = new DecisionTreeImplementation();
            this.clientsQueueTreeRoot = sam.GetTree("C:\\Users\\Maciej\\Desktop\\client.txt");
        }

        public Section[][] RestaurantSections { get; set; }

        public State State { get; set; }

        public List<WaiterAction> GoToPoint(int x, int y)
        {
            var targetState = new State { X = x, Y = y, Direction = Direction.South };

            var walkAlgorithm = new WalkingAStar();
            var waiterActions = walkAlgorithm.GetPath(this.State, targetState, this.RestaurantSections);

            return waiterActions;
        }

        public void CreateSections()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Section section = null;

                    if (i == 5 && j == 6)
                    {
                        section = new Chair { X = i * 25, Y = j * 25 };
                    }
                    if (i == 1 && j == 8)
                    {
                        section = new Chair { X = i * 25, Y = j * 25 };
                    }
                    if (i == 8 && j == 9)
                    {
                        section = new Chair { X = i * 25, Y = j * 25 };
                    }
                    if (i == 2 && j == 2)
                    {
                        section = new Chair { X = i * 25, Y = j * 25 };
                    }

                    if (i == 3 && j == 8)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i * j == 81)
                    {
                        section = new Kitchen { X = i * 25, Y = j * 25 };
                    }

                    if (section == null)
                    {
                        section = new Floor { X = i * 25, Y = j * 25 };
                    }

                    this.RestaurantSections[i][j] = section;
                }
            }

            this.RestaurantSections[this.State.X][this.State.Y] = new Waiter
            {
                X = this.State.X * 25,
                Y = this.State.Y * 25
            };
        }

        public void StartWork()
        {
            this.WrittenOrders = new List<Order>();
            this.DoneOrdersOnHands = new List<Order>();
            this.kitchenWorker.Start();
            this.clientsQueueWorker.Start();
            this.tableWorker.Start();
        }

        public void StopWork()
        {
            this.kitchenWorker.Stop();
            this.clientsQueueWorker.Stop();
            this.tableWorker.Stop();
            this.DoneOrdersOnKitchen = 0;
            this.OrdersIsProgressOnKitchen = 0;
            this.ClientsQueueCount = 0;
        }

        /// <summary>
        /// Przekazanie zebranych zamówień do kuchnii
        /// </summary>
        public void GiveOrdersToKitchen()
        {
            this.OrdersIsProgressOnKitchen = this.kitchenWorker.AddNewOrders(this.WrittenOrders);
            this.WrittenOrders = new List<Order>();
        }

        /// <summary>
        /// Odebranie zamówienia z kuchnii na rece kelnera
        /// </summary>
        public void TakeOrderFromKitchen()
        {
            var doneOrder = this.kitchenWorker.DoneOrders.FirstOrDefault();
            if (doneOrder != null)
            {
                this.DoneOrdersOnHands.Add(doneOrder);
                this.kitchenWorker.DoneOrders.Remove(doneOrder);
                this.DoneOrdersOnKitchen--;
            }
        }

        /// <summary>
        /// Wydaj wszystkie zamówienia na stoliki
        /// </summary>
        public void GiveOrdersToClients()
        {
            this.DoneOrdersOnHands = new List<Order>();
            this.tableWorker.Order.OrderState = Order.State.Done;
        }

        public void GetClientFromQueue()
        {
            var client = this.clientsQueueWorker.WaitingClients.FirstOrDefault();
            if (client != null)
            {
                this.clientsQueueWorker.WaitingClients.Remove(client);
                this.ClientsQueueCount--;
                this.tableWorker.IsFree = false;
                this.RefreshInformations();
            }
        }

        public void CleanTable()
        {
            this.tableWorker.IsDirty = false;
            this.RefreshInformations();
        }

        /// <summary>
        /// Zamówienia od klientów zapisane na kartkę
        /// </summary>
        public List<Order> WrittenOrders
        {
            get
            {
                return this.writtenOrders;
            }

            set
            {
                this.writtenOrders = value;
                this.RaisePropertyChanged("WrittenOrders");
                this.RaisePropertyChanged("InformationAboutWaiter");
            }
        }

        /// <summary>
        /// Zrobione zamówienia odebrane z kuchni i niesione na stolik
        /// </summary>
        public List<Order> DoneOrdersOnHands
        {
            get
            {
                return this.doneOrdersOnHands;
            }

            set
            {
                this.doneOrdersOnHands = value;
                this.RaisePropertyChanged("DoneOrdersOnHands");
                this.RaisePropertyChanged("InformationAboutWaiter");
            }
        }

        public int OrdersIsProgressOnKitchen
        {
            get
            {
                return this.mealsOnKitchen;
            }

            set
            {
                this.mealsOnKitchen = value;
                this.RaisePropertyChanged("OrdersIsProgressOnKitchen");
                this.RaisePropertyChanged("InformationAboutKitchen");
            }
        }

        public int DoneOrdersOnKitchen
        {
            get
            {
                return this.doneOrdersOnKitchen;
            }

            set
            {
                this.doneOrdersOnKitchen = value;
                this.RaisePropertyChanged("DoneOrdersOnKitchen");
                this.RaisePropertyChanged("InformationAboutKitchen");
            }
        }

        public string InformationAboutKitchen
        {
            get
            {
                return string.Format("Przyjęte zamówienia: {0}\nWydane zamówienia: {1}", this.OrdersIsProgressOnKitchen, this.DoneOrdersOnKitchen);
            }
        }

        public string InformationAboutWaiter
        {
            get
            {
                if (this.DoneOrdersOnHands != null && this.WrittenOrders != null)
                {
                    return string.Format(
                        "Przyjęte zamówienia: {0}\nZamówienia na rękach: {1}",
                        this.WrittenOrders.Count,
                        this.DoneOrdersOnHands.Count);
                }

                return string.Empty;
            }
        }

        public int OrdersOnTables
        {
            get
            {
                return this.ordersOnTables;
            }

            set
            {
                this.ordersOnTables = value;
                this.RaisePropertyChanged("OrdersOnTables");
                this.RaisePropertyChanged("InformationAboutTable");
            }
        }

        public string InformationAboutTable
        {
            get
            {
                return string.Format(
                    "Zamówienie: {0}\nWolny: {1}\nBrudny: {2}",
                    this.OrdersOnTables,
                    this.tableWorker.IsFree,
                    this.tableWorker.IsDirty);
            }
        }

        public void RefreshInformations()
        {
            this.RaisePropertyChanged("InformationAboutWaiter");
            this.RaisePropertyChanged("InformationAboutKitchen");
            this.RaisePropertyChanged("InformationAboutClients");
            this.RaisePropertyChanged("InformationAboutTable");
        }


        public int ClientsQueueCount
        {
            get
            {
                return this.clientsQueueCount;
            }

            set
            {
                this.clientsQueueCount = value;
                this.RaisePropertyChanged("ClientsQueue");
                this.RaisePropertyChanged("InformationAboutClients");
            }
        }

        public string InformationAboutClients
        {
            get
            {
                return string.Format("Klienci w kolejce: {0}", this.ClientsQueueCount);
            }
        }

        private void OnNewMealFromKitchenWorker(object sender, NewMealEventArgs eventArgs)
        {
            try
            {
                rwl.AcquireReaderLock(100);
                try
                {

                    this.DoneOrdersOnKitchen = eventArgs.DoneMealCount;
                    this.OrdersIsProgressOnKitchen = eventArgs.OrdersInProgressCount;
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (Exception)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
            this.RefreshInformations();
        }

        private void OnNewClientCome(object sender, NewClientEventArgs eventArgs)
        {
            try
            {
                rwl.AcquireReaderLock(100);
                try
                {
                    this.ClientsQueueCount = eventArgs.ClientCount;
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (Exception)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
            
            this.RefreshInformations();

            if (this.NewClientDecision())
            {
                var waiterActions = this.GoToPoint(9, 8);
                GetClientEventArgs e = new GetClientEventArgs(waiterActions);
                this.OnGetClient(e);
                Debug.WriteLine("Przyjąłem klienta");
            }
            else
            {
                Debug.WriteLine("Nie przyjąłem klienta");
            }
        }

        private bool NewClientDecision()
        {
            var currentTreeNode = this.clientsQueueTreeRoot;
            var parameter = -1;
           
            while (true)
            {
#region checkvalue
                if (currentTreeNode.Attribute == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(currentTreeNode.Attribute.ToString()))
                {
                    return false;
                }
#endregion checkvalue
                if (currentTreeNode.Attribute.ToString().Trim().ToLower() == "false")
                {
                    return false;
                }

                if (currentTreeNode.Attribute.ToString().Trim().ToLower() == "true")
                {
                    return true;
                }

                switch (currentTreeNode.Attribute.ToString())
                {
                    case "queue":
                        parameter = this.ClientsQueueCount;
                        break;
                    case "waiting_time":
                        parameter = this.WaitingTime();
                        break;
                    case "order":
                        parameter = this.OrdersOnTables;
                        break;
                    case "meals":
                        parameter = this.DoneOrdersOnKitchen;
                        break;
                    case "dirty":
                        parameter = this.CheckDirtyTables();
                        break;
                    case "free":
                        parameter = this.CheckFreeTables();
                        break;
                }

                var value = int.Parse(currentTreeNode.Attribute.PossibleValues[0]);
                var difference = System.Math.Abs(parameter - value);
                var index = 0;
                for (int i = 1; i < currentTreeNode.Attribute.PossibleValues.Count; i++)
                {
                    value = int.Parse(currentTreeNode.Attribute.PossibleValues[i]);
                    var newDifference = System.Math.Abs(parameter - value);

                    if (newDifference < difference)
                    {
                        difference = newDifference;
                        index++;
                        continue;
                    }

                    break;
                }

                currentTreeNode = currentTreeNode._children[index];
                
            }

            return false;
        }

        private void OnClientOut(object sender, ClientOutEventArgs e)
        {
            try
            {
                rwl.AcquireReaderLock(100);
                try
                {
                    this.OrdersOnTables--;
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (Exception)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
            
            this.RefreshInformations();
        }

        private void OnNewTableOrder(object sender, NewTableOrderEventArgs e)
        {
            try
            {
                rwl.AcquireReaderLock(100);
                try
                {
                    this.OrdersOnTables++;
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (Exception)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        public event GetClientEventHandler GetClientEvent;

        protected virtual void OnGetClient(GetClientEventArgs e)
        {
            this.GetClientEvent(this, e);
        }

        public delegate void GetClientEventHandler(object sender, GetClientEventArgs e);

        public class GetClientEventArgs
        {
            public GetClientEventArgs(List<WaiterAction> waiterActions)
            {
                this.WaiterActions = waiterActions;
            }
            
            public List<WaiterAction> WaiterActions { get; set; }
        }
    }
}
