namespace Kelner.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows.Media.Imaging;

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

        private TreeNode ordersQueueTreeRoot;

        private KitchenWorker kitchenWorker;

        private ClientsQueueWorker clientsQueueWorker;

        private TableWorker[] tableWorkers;

        private int mealsOnKitchen;

        private int doneOrdersOnKitchen;

        private int clientsQueueCount;

        private List<Order> writtenOrders;

        private List<Order> doneOrdersOnHands;

        private int ordersOnTables;

        private bool newOrdersToGetFromKitchen;

        private BitmapImage kitchenImage;

        private BitmapImage tableImage1;
        private BitmapImage tableImage2;
        private BitmapImage tableImage3;
        private BitmapImage tableImage4;
        private BitmapImage tableImage5;

        private static ReaderWriterLock rwl;
        private NeuralBackProp neuralBackProp;

        public WaiterWorker()
        {
            neuralBackProp = new NeuralBackProp();

            rwl = new ReaderWriterLock();
            this.WrittenOrders = new List<Order>();
            this.DoneOrdersOnHands = new List<Order>();

            this.NewOrdersToGetFromKitchen = false;

            this.kitchenWorker = new KitchenWorker();
            this.kitchenWorker.NewMealEvent += this.OnNewMealFromKitchenWorker;

            this.clientsQueueWorker = new ClientsQueueWorker();
            this.clientsQueueWorker.NewClientEvent += this.OnNewClientCome;

            this.InitTableWorkers();

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

            this.KitchenImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\_.bmp", UriKind.Relative));
            this.TableImage1 = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));
            this.TableImage2 = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));
            this.TableImage3 = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));
            this.TableImage4 = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));
            this.TableImage5 = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));

            DecisionTreeImplementation sam = new DecisionTreeImplementation();
            this.clientsQueueTreeRoot = sam.GetTree("..\\..\\Data\\client.txt");
            this.ordersQueueTreeRoot = sam.GetTree("..\\..\\Data\\order.txt");
        }

        private void InitTableWorkers()
        {
            this.tableWorkers = new TableWorker[5];
            this.tableWorkers[0] = new TableWorker(2, 2,1);
            this.tableWorkers[1] = new TableWorker(2, 5,2);
            this.tableWorkers[2] = new TableWorker(2, 8,3);
            this.tableWorkers[3] = new TableWorker(7, 3,4);
            this.tableWorkers[4] = new TableWorker(7, 6,5);

            foreach (var tableWorker in this.tableWorkers)
            {
                tableWorker.ClientOutEvent += this.OnClientOut;
                tableWorker.NewTableOrderEvent += this.OnNewTableOrder;
            }
        }

        public Section[][] RestaurantSections { get; set; }

        public State State { get; set; }

        public BitmapImage KitchenImage
        {
            get
            {
                return this.kitchenImage;
            }

            set
            {
                this.kitchenImage = value;
                this.RaisePropertyChanged("KitchenImage");
            }
        }

        public BitmapImage TableImage1
        {
            get
            {
                return this.tableImage1;
            }

            set
            {
                this.tableImage1 = value;
                this.RaisePropertyChanged("TableImage1");
            }
        }

        public BitmapImage TableImage2
        {
            get
            {
                return this.tableImage2;
            }

            set
            {
                this.tableImage2 = value;
                this.RaisePropertyChanged("TableImage2");
            }
        }
        public BitmapImage TableImage3
        {
            get
            {
                return this.tableImage3;
            }

            set
            {
                this.tableImage3 = value;
                this.RaisePropertyChanged("TableImage3");
            }
        }
        public BitmapImage TableImage4
        {
            get
            {
                return this.tableImage4;
            }

            set
            {
                this.tableImage4 = value;
                this.RaisePropertyChanged("TableImage4");
            }
        }
        public BitmapImage TableImage5
        {
            get
            {
                return this.tableImage5;
            }

            set
            {
                this.tableImage5 = value;
                this.RaisePropertyChanged("TableImage5");
            }
        }

        public List<WaiterAction> GoToPointOld(int x, int y)
        {
            var targetState = new State { X = x, Y = y, Direction = Direction.South };

            var walkAlgorithm = new WalkingAStarOld();
            var waiterActions = walkAlgorithm.GetPath(this.State, targetState, this.RestaurantSections);

            return waiterActions;
        }

        public List<Node> GoToPoint(int x, int y)
        {
            var currentNode = new Node { X = this.State.X, Y = this.State.Y };
            var targetNode = new Node { X = x, Y = y};

            var walkAlgorithm = new WalkingAStar();
            var waiterActions = walkAlgorithm.GetPath(currentNode, targetNode, this.RestaurantSections);

            return waiterActions;
        }

        public void CreateSections()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Section section = null;

                    if (i == 2 && j == 2)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i == 2 && j == 5)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i == 2 && j == 8)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i == 7 && j == 3)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i == 7 && j == 6)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }

                    if (i == 9 && j == 0)
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
            foreach (var tableWorker in this.tableWorkers)
            {
                tableWorker.Start();   
            }
        }

        public void StopWork()
        {
            this.kitchenWorker.Stop();
            this.clientsQueueWorker.Stop();
            foreach (var tableWorker in this.tableWorkers)
            {
                tableWorker.Stop();
            }
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
            foreach (var tableWorker in this.tableWorkers)
            {
                if (tableWorker.Order.OrderState == Order.State.New)
                {
                    tableWorker.Order.OrderState = Order.State.Done;
                }
            }
        }

        public void GetClientFromQueue()
        {
            var client = this.clientsQueueWorker.WaitingClients.FirstOrDefault();
            if (client != null)
            {
                this.clientsQueueWorker.WaitingClients.Remove(client);
                this.ClientsQueueCount--;
                foreach (var tableWorker in this.tableWorkers)
                {
                    if (tableWorker.IsFree && !tableWorker.IsDirty)
                    {
                        tableWorker.IsFree = false;

                        BitmapImage newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\Z.bmp", UriKind.Relative));
                        newImage.Freeze();
                        switch (tableWorker.Number)
                        {
                            case 1:
                                this.TableImage1 = newImage;
                                break;
                            case 2:
                                this.TableImage2 = newImage;
                                break;
                            case 3:
                                this.TableImage3 = newImage;
                                break;
                            case 4:
                                this.TableImage4 = newImage;
                                break;
                            case 5:
                                this.TableImage5 = newImage;
                                break;
                            default:
                                break;
                        }

                        break;
                    }
                }

                this.RefreshInformations();
            }
        }

        public void CleanTable()
        {
            foreach (var tableWorker in this.tableWorkers)
            {
                if (tableWorker.IsDirty)
                {
                    tableWorker.IsDirty = false;
                    var actions = this.GoToPoint(tableWorker.X + 1, tableWorker.Y);
                    this.MoveWaiter(actions);

                    BitmapImage newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\W.bmp", UriKind.Relative));
                    newImage.Freeze();
                    switch (tableWorker.Number)
                    {
                        case 1:
                            this.TableImage1 = newImage;
                            break;
                        case 2:
                            this.TableImage2 = newImage;
                            break;
                        case 3:
                            this.TableImage3 = newImage;
                            break;
                        case 4:
                            this.TableImage4 = newImage;
                            break;
                        case 5:
                            this.TableImage5 = newImage;
                            break;
                        default:
                            break;
                    }
                }
                
            }
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

         private bool NewOrdersToGetFromKitchen
        {
            get
            {
                return this.newOrdersToGetFromKitchen;
            }

            set
            {
                this.newOrdersToGetFromKitchen = value;

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

                BitmapImage newImage = null;

                switch (value)
                {
                    case 0:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\_.bmp", UriKind.Relative));
                        break;
                    case 1:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\1.bmp", UriKind.Relative));
                        break;
                    case 2:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\2.bmp", UriKind.Relative));
                        break;
                    case 3:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\3.bmp", UriKind.Relative));
                        break;
                    case 4:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\4.bmp", UriKind.Relative));
                        break;
                    case 5:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\5.bmp", UriKind.Relative));
                        break;
                    default:
                        newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\_.bmp", UriKind.Relative));
                        break;
                }
                newImage.Freeze(); // -> to prevent error: "Must create DependencySource on same Thread as the DependencyObject"
                this.KitchenImage = newImage;

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
                    "Liczba zamówień: {0}\nWolnych: {1}\nBrudnych: {2}",
                    this.OrdersOnTables,
                    this.tableWorkers.Count(t => t.IsFree),
                    this.tableWorkers.Count(t => t.IsDirty));
;
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

                    String readLetter = neuralBackProp.ReadLetter(this.KitchenImage);
                    int number = 0;
                    switch (readLetter)
                    {
                        case "1":
                            number = 1;
                            break;
                        case "2":
                            number = 2;
                            break;
                        case "3":
                            number = 3;
                            break;
                        case "4":
                            number = 4;
                            break;
                        case "5":
                            number = 5;
                            break;
                        default:
                            number = 0;
                            break;
                    }

                    Debug.WriteLine("Widzę cyferkę " + readLetter +", więc na kuchni jest " + number + " zamówienie(a)");

                    if (number > 0)
                    {
                        var waiterActions = this.GoToPoint(9, 1);
                        this.MoveWaiter(waiterActions);

                        this.DoneOrdersOnKitchen = 0;
                        var i = eventArgs.DoneMealCount;
                        foreach (var tableWorker in this.tableWorkers)
                        {
                            if (!tableWorker.IsFree && !tableWorker.IsDirty)
                            {
                                waiterActions = this.GoToPoint(tableWorker.X + 1, tableWorker.Y);
                                this.MoveWaiter(waiterActions);
                                tableWorker.Order.OrderState = Order.State.Done;
                                Debug.WriteLine("Wydaje zamówienie na stolik: " + tableWorker.Number);
                                i--;
                            }

                            if (i <= 0)
                            {
                                break;
                            }
                        }
                    }
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

            while (true)
            {
                Thread.Sleep(1000 * 5);
                if (this.NewClientDecision())
                {
                    var waiterActions = this.GoToPoint(9, 8);
                    this.MoveWaiter(waiterActions);

                    Debug.WriteLine("Przyjąłem klienta");
                    this.GetClientFromQueue();
                    break;
                }
                else
                {
                    Debug.WriteLine("Nie przyjąłem klienta");
                }
            }
        }

        private void MoveWaiter(List<Node> waiterActions)
        {
            foreach (var waiterAction in waiterActions)
            {
                MoveWaiterEventArgs e = new MoveWaiterEventArgs(waiterAction);
                this.OnMoveWaiter(e);
                Thread.Sleep(300);
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
                    return true;
                }

                if (string.IsNullOrEmpty(currentTreeNode.Attribute.ToString()))
                {
                    return true;
                }

                if (!this.tableWorkers.Any(t => t.IsFree && !t.IsDirty))
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
                        parameter = this.WaitingTime(); //Random
                        break;
                    case "order":
                        parameter = this.OrdersOnTables;
                        break;
                    case "meals":
                        parameter = this.DoneOrdersOnKitchen;
                        break;
                    case "dirty":
                        parameter = this.CheckDirtyTables(); //Random
                        break;
                    case "free":
                        parameter = this.tableWorkers.Count(t => t.IsFree);
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

        private bool NewOrderDecision()
        {
            var currentTreeNode = this.ordersQueueTreeRoot;
            var parameter = -1;

            while (true)
            {
                #region checkvalue
                if (currentTreeNode.Attribute == null)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(currentTreeNode.Attribute.ToString()))
                {
                    return true;
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
                        parameter = this.OrdersOnKitchen();
                        break;
                    case "dirty":
                        parameter = this.CheckDirtyTables();
                        break;
                    case "free":
                        parameter = this.tableWorkers.Count(t => t.IsFree);
                        break;
                }

                var value = int.Parse(currentTreeNode.Attribute.PossibleValues[currentTreeNode.Attribute.PossibleValues.Count - 1]);
                var difference = System.Math.Abs(parameter - value);
                var index = currentTreeNode.Attribute.PossibleValues.Count - 1;
                for (int i = currentTreeNode.Attribute.PossibleValues.Count - 2; i >= 0; i--)
                {
                    value = int.Parse(currentTreeNode.Attribute.PossibleValues[i]);
                    var newDifference = System.Math.Abs(parameter - value);

                    if (newDifference < difference)
                    {
                        difference = newDifference;
                        index--;
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
                    var random = new Random();
                    int x = random.Next(2, 3);
                    int dirtyCount = 0;

                    BitmapImage newImage = new BitmapImage(new Uri("..\\..\\Data\\ImageSet\\B.bmp", UriKind.Relative));
                    newImage.Freeze();
                    switch (e.TableNumber)
                    {
                        case 1:
                            this.TableImage1 = newImage;
                            break;
                        case 2:
                            this.TableImage2 = newImage;
                            break;
                        case 3:
                            this.TableImage3 = newImage;
                            break;
                        case 4:
                            this.TableImage4 = newImage;
                            break;
                        case 5:
                            this.TableImage5 = newImage;
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("Sprawdzam stan stolików:");
                    foreach (var tableWorker in tableWorkers)
                    {
                        BitmapImage tableImage;
                        switch (tableWorker.Number)
                        {
                            case 1:
                                tableImage = this.TableImage1;
                                break;
                            case 2:
                                tableImage = this.TableImage2;
                                break;
                            case 3:
                                tableImage = this.TableImage3;
                                break;
                            case 4:
                                tableImage = this.TableImage4;
                                break;
                            case 5:
                                tableImage = this.TableImage5;
                                break;
                            default:
                                tableImage = new BitmapImage();
                                break;
                        }
                        String readLetter = neuralBackProp.ReadLetter(tableImage);
                        switch (readLetter)
                        {
                            case "B":
                                Console.WriteLine("Stolik nr.{0} jest brudny. {1}", tableWorker.Number, readLetter);
                                dirtyCount++;
                                break;
                            case "Z":
                                Console.WriteLine("Stolik nr.{0} jest zajęty. {1}", tableWorker.Number, readLetter);
                                break;
                            case "W":
                                Console.WriteLine("Stolik nr.{0} jest wolny. {1}", tableWorker.Number, readLetter);
                                break;
                            default:
                                break;
                        }
                    }

                    if (dirtyCount > x)
                    {
                        this.CleanTable();
                    }

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

        private void OnNewTableOrder(object sender, NewTableOrderEventArgs args)
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

            while (true)
            {
                Thread.Sleep(1000 * 5);
                if (this.NewOrderDecision())
                {
                    var waiterActions = this.GoToPoint(args.TableX + 1, args.TableY);
                    this.OrdersOnTables--;
                    this.MoveWaiter(waiterActions);
                    Debug.WriteLine("Przyjąłem zamówienie");

                    var newOrder = new Order { OrderState = Order.State.New, TableNumber = args.TableNumber };
                    this.WrittenOrders.Add(newOrder);
                    this.GiveOrdersToKitchen();
                    this.RefreshInformations();
                    break;
                }
                else
                {
                    Debug.WriteLine("Nie przyjąłem zamówienia");
                }
            }
        }

        public event MoveWaiterEventHandler MoveWaiterEvent;

        protected virtual void OnMoveWaiter(MoveWaiterEventArgs e)
        {
            this.MoveWaiterEvent(this, e);
        }

        public delegate void MoveWaiterEventHandler(object sender, MoveWaiterEventArgs e);

        public class MoveWaiterEventArgs
        {
            public MoveWaiterEventArgs(Node waiterAction)
            {
                this.WaiterAction = waiterAction;
            }
            
            public Node WaiterAction { get; set; }
        }
    }
}
