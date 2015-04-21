namespace Kelner.Model
{
    using Kelner.ViewModel;

    /// <summary>
    /// Klasa reprezentująca obiekt Kelnera, która będzie zbierać informacje o tym co się dzieje w restauracji
    /// </summary>
    public class WaiterWorker : ViewModelBase
    {
        // todo: metoda QuoVadis :)

        private KitchenWorker kitchenWorker;

        private ClientsQueue clientsQueue;

        private int mealsOnKitchen;

        private int orders;

        private int clientsQueueCount;

        public WaiterWorker()
        {
            this.MealsOnKitchen = 0;
            this.kitchenWorker = new KitchenWorker();
            this.kitchenWorker.NewMealEvent += this.OnNewMealFromKitchenWorker;

            this.clientsQueue = new ClientsQueue();
            this.clientsQueue.NewClientEvent += this.OnNewClientCome;
        }

        public void StartWork()
        {
            this.kitchenWorker.Start();
            this.clientsQueue.Start();
        }

        public void StopWork()
        {
            this.kitchenWorker.Stop();
            this.clientsQueue.Stop();
            this.Orders = 0;
            this.MealsOnKitchen = 0;
            this.ClientsQueueCount = 0;
        }

        public int MealsOnKitchen
        {
            get
            {
                return this.mealsOnKitchen;
            }

            set
            {
                this.mealsOnKitchen = value;
                this.RaisePropertyChanged("MealsOnKitchen");
                this.RaisePropertyChanged("InformationAboutKitchen");
            }
        }

        public Section[][] RestaurantSections { get; set; }

        public int Orders
        {
            get
            {
                return this.orders;
            }

            set
            {
                this.orders = value;
                this.RaisePropertyChanged("Orders");
                this.RaisePropertyChanged("InformationAboutKitchen");
            }
        }

        public string InformationAboutKitchen
        {
            get
            {
                return string.Format("Wydane dania: {0}\nPrzyjęte zamówienia: {1}", this.MealsOnKitchen, this.Orders);
            }
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

        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public Direction WaiterDirection { get; set; }

        public enum Direction
        {
            North,
            South,
            East,
            West
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
            this.MealsOnKitchen += eventArgs.MealCount;
        }

        private void OnNewClientCome(object sender, NewClientEventArgs eventArgs)
        {
            this.ClientsQueueCount += eventArgs.ClientCount;
        }
    }
}
