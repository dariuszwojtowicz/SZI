namespace Kelner.Model
{
    using Kelner.ViewModel;

    /// <summary>
    /// Klasa reprezentująca obiekt Kelnera, która będzie zbierać informacje o tym co się dzieje w restauracji
    /// </summary>
    public class Waiter : ViewModelBase
    {
        private Kitchen kitchen;

        private int mealsOnKitchen;

        private int orders;

        public Waiter()
        {
            this.MealsOnKitchen = 0;
            this.kitchen = new Kitchen();
            this.kitchen.NewMealEvent += this.OnNewMealFromKitchen;
        }

        public void StartWork()
        {
            this.kitchen.Start();
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

        private void OnNewMealFromKitchen(object sender, NewMealEventArgs eventArgs)
        {
            this.MealsOnKitchen += eventArgs.MealCount;
        }
    }
}
