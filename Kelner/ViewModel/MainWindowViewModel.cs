namespace Kelner.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Kelner.Algorithm;
    using Kelner.Model;

    public class MainWindowViewModel : ViewModelBase
    {
        private string waiterName;

        private string firstSectionData;

        private WaiterWorker waiter;

        private ObservableCollection<Section> sections;

        public MainWindowViewModel()
        {
            this.Waiter = new WaiterWorker();
            this.StartWaiterWorkCommand = new RelayCommand(this.StartWork);
            this.StopWaiterWorkCommand = new RelayCommand(this.StopWork);
            this.WriteNewOrderCommand = new RelayCommand(this.WriteNewOrder);
            this.GiveWrittenOrdersToKitchenCommand = new RelayCommand(this.GiveNewOrdersToKitchen);
            this.TakeOrderFromKitchenCommand = new RelayCommand(this.TakeOrderFromKitchen);
            this.GiveOrdersToClientsCommand = new RelayCommand(this.GiveOrdersToClients);
            this.GetClientFromQueueCommand = new RelayCommand(this.GetClientFromQueue);
            this.CleanTableCommand = new RelayCommand(this.CleanTable);
            this.GoToPointCommand = new RelayCommand(this.GoToPoint);

            this.Waiter.MoveWaiterEvent += this.OnMoveWaiter;

            this.CreateSections();
        }

        public RelayCommand StartWaiterWorkCommand { get; set; }

        public RelayCommand StopWaiterWorkCommand { get; set; }

        public RelayCommand WriteNewOrderCommand { get; set; }

        public RelayCommand GiveWrittenOrdersToKitchenCommand { get; set; }

        public RelayCommand TakeOrderFromKitchenCommand { get; set; }

        public RelayCommand GiveOrdersToClientsCommand { get; set; }

        public RelayCommand GetClientFromQueueCommand { get; set; }

        public RelayCommand CleanTableCommand { get; set; }

        public RelayCommand GoToPointCommand { get; set; }

        public ObservableCollection<Section> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                this.sections = value;
                this.RaisePropertyChanged("Sections");
                
            }
        }

        public WaiterWorker Waiter
        {
            get
            {
                return this.waiter;
            }

            set
            {
                this.waiter = value;
                this.RaisePropertyChanged("Waiter");
            }
        }

        public int TargetX { get; set; }

        public int TargetY { get; set; }

        private void StartWork()
        {
            this.Waiter.StartWork();
        }

        private void StopWork()
        {
            this.Waiter.StopWork();
        }

        /// <summary>
        /// Zapisz nowe zamówienie od klientów
        /// </summary>
        private void WriteNewOrder()
        {
            var newOrder = new Order { OrderState = Order.State.New, TableNumber = 2 };
            this.Waiter.WrittenOrders.Add(newOrder);

            this.Waiter.RefreshInformations();
        }

        private void GiveNewOrdersToKitchen()
        {
            this.Waiter.GiveOrdersToKitchen();
            this.Waiter.RefreshInformations();
        }

        private void TakeOrderFromKitchen()
        {
            this.Waiter.TakeOrderFromKitchen();
            this.Waiter.RefreshInformations();
        }

        private void GiveOrdersToClients()
        {
            this.Waiter.GiveOrdersToClients();
            this.Waiter.RefreshInformations();
        }

        private void GetClientFromQueue()
        {
            this.Waiter.GetClientFromQueue();
        }

        private void CleanTable()
        {
            this.Waiter.CleanTable();
        }

        private void GoToPoint()
        {
            var waiterActions = this.Waiter.GoToPoint(this.TargetX, this.TargetY);

            this.MoveWaiter(waiterActions);
        }

        private void OnMoveWaiter(object sender, WaiterWorker.MoveWaiterEventArgs eventArgs)
        {
            this.MoveWaiter(eventArgs.WaiterActions);
        }

        private void MoveWaiter(List<Node> waiterActions)
        {
            waiterActions.Reverse();
            foreach (var waiterAction in waiterActions)
            {
                this.Waiter.State = new State { X = waiterAction.X, Y = waiterAction.Y, Direction = Direction.East };
                this.Waiter.CreateSections();
                this.CreateSections();
            }
        }

        private void MoveWaiter(List<WaiterAction> waiterActions)
        {
            foreach (var waiterAction in waiterActions)
            {
                switch (waiterAction)
                {
                    case WaiterAction.GoForward:
                        this.WaiterMoveForward();
                        Debug.WriteLine("Krok do przodu");
                        break;
                    case WaiterAction.TurnLeft:
                        this.WaiterTurnLeft();
                        Debug.WriteLine("Obórt w lewo");
                        break;
                    case WaiterAction.TurnRight:
                        this.WaiterTurnRight();
                        Debug.WriteLine("Obrót w prawo");
                        break;
                }
            }

            this.Waiter.CreateSections();
            this.CreateSections();
        }

        private void WaiterMoveForward()
        {
            var newState = new State { Direction = this.Waiter.State.Direction };
            switch (this.Waiter.State.Direction)
            {
                case Direction.South:
                    newState.X = this.Waiter.State.X;
                    newState.Y = this.Waiter.State.Y + 1;
                    break;
                case Direction.West:
                    newState.X = this.Waiter.State.X - 1;
                    newState.Y = this.Waiter.State.Y;
                    break;
                case Direction.North:
                    newState.X = this.Waiter.State.X;
                    newState.Y = this.Waiter.State.Y - 1;
                    break;
                case Direction.East:
                    newState.X = this.Waiter.State.X + 1;
                    newState.Y = this.Waiter.State.Y;
                    break;
            }

            this.Waiter.State = newState;
        }

        private void WaiterTurnLeft()
        {
            var newState = new State { X = this.Waiter.State.X, Y = this.Waiter.State.Y };
            switch (this.Waiter.State.Direction)
            {
                case Direction.South:
                    newState.Direction = Direction.East;
                    break;
                case Direction.East:
                    newState.Direction = Direction.North;
                    break;
                case Direction.North:
                    newState.Direction = Direction.West;
                    break;
                case Direction.West:
                    newState.Direction = Direction.South;
                    break;
            }

            this.Waiter.State = newState;
        }

        private void WaiterTurnRight()
        {
            var newState = new State { X = this.Waiter.State.X, Y = this.Waiter.State.Y };
            switch (this.Waiter.State.Direction)
            {
                case Direction.South:
                    newState.Direction = Direction.West;
                    break;
                case Direction.West:
                    newState.Direction = Direction.North;
                    break;
                case Direction.North:
                    newState.Direction = Direction.East;
                    break;
                case Direction.East:
                    newState.Direction = Direction.South;
                    break;
            }

            this.Waiter.State = newState;
        }

        private void CreateSections()
        {
            this.Sections = new ObservableCollection<Section>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Section section = this.Waiter.RestaurantSections[i][j];

                    this.Sections.Add(section);
                }
            }
        }
    }
}
