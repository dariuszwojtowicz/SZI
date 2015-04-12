namespace Kelner.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string waiterName;

        private string firstSectionData;

        private int someNumber;

        public MainWindowViewModel()
        {
            this.someNumber = 1;
            this.SayHelloCommand = new RelayCommand(this.SayHello);
            this.RefreshDataCommand = new RelayCommand(this.RefreshFirstSectionData);
        }

        public RelayCommand SayHelloCommand { get; set; }
        public RelayCommand RefreshDataCommand { get; set; }

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

        public string FirstSectionData
        {
            get
            {
                return string.Format(" - Jakieś tam dane: {0}", this.someNumber);
            }
        }

        private void SayHello()
        {
            this.WaiterName = "Siema, jestem Zenek Bębenek";
        }

        private void RefreshFirstSectionData()
        {
            this.someNumber++;
            this.RaisePropertyChanged("FirstSectionData");
        }
    }
}
