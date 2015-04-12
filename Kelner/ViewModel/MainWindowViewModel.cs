namespace Kelner.ViewModel
{
    using Kelner.Model;

    public class MainWindowViewModel : ViewModelBase
    {
        private string waiterName;

        private string firstSectionData;

        private Waiter waiter;

        public MainWindowViewModel()
        {
            this.Waiter = new Waiter();
            this.SayHelloCommand = new RelayCommand(this.SayHello);
            this.StartWaiterWorkCommand = new RelayCommand(this.StartWork);
            this.StopWaiterWorkCommand = new RelayCommand(this.StopWork);
            this.AddOrderCommand = new RelayCommand(this.AddOrder);
        }

        public RelayCommand SayHelloCommand { get; set; }
        public RelayCommand StartWaiterWorkCommand { get; set; }
        public RelayCommand StopWaiterWorkCommand { get; set; }
        public RelayCommand AddOrderCommand { get; set; }

        public string WaiterName
        {
            get
            {
                return this.waiterName;
            }

            set
            {
                this.waiterName = value;
                this.RaisePropertyChanged("WaiterName");
            }
        }

        public Waiter Waiter
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

        private void SayHello()
        {
            this.WaiterName = "Siema, jestem Zenek Bębenek";
        }

        private void StartWork()
        {
            this.Waiter.StartWork();
        }

        private void StopWork()
        {
            this.Waiter.StopWork();
        }

        private void AddOrder()
        {
            this.Waiter.Orders++;
        }
    }
}
