namespace Kelner.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Media.Animation;
    using System.Windows.Media.TextFormatting;

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
            this.SayHelloCommand = new RelayCommand(this.SayHello);
            this.StartWaiterWorkCommand = new RelayCommand(this.StartWork);
            this.StopWaiterWorkCommand = new RelayCommand(this.StopWork);
            this.AddOrderCommand = new RelayCommand(this.AddOrder);

            this.CreateSections();
        }

        public RelayCommand SayHelloCommand { get; set; }
        public RelayCommand StartWaiterWorkCommand { get; set; }
        public RelayCommand StopWaiterWorkCommand { get; set; }
        public RelayCommand AddOrderCommand { get; set; }

        public ObservableCollection<Section> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                this.sections = value;
                
            }
        }

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

        private void SayHello()
        {
            this.WaiterName = "Siema, jestem Zenek Bębenek";
            this.CreateSections();
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

        private void CreateSections()
        {
            this.Sections = new ObservableCollection<Section>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var section = new Section { X = i * 25, Y = j * 25 };
                    this.Sections.Add(section);
                }
            }
        }
    }
}
