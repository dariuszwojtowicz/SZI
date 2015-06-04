namespace Kelner.Model
{

    public class Order
    {
        public int TableNumber { get; set; }

        public State OrderState { get; set; }

        public enum State
        {
            // Klienci chcą złożyć
            New,
            // Gdy kelner zapisał na kartkę ale nie zaniósł do kuchnii
            ToKitchen,
            // Robione na kuchnii
            InProgress,
            //Odebrane z kuchnii ale jeszcze nie zasniesone do stolika
            ToTable,
            // Oddane klientom na stół
            Done
        }
    }
}
