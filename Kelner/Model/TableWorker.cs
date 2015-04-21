namespace Kelner.Model
{
    using System.Collections.Generic;

    public class TableWorker
    {
        public List<TablePart> TableParts { get; set; }

        public List<Chair> Chairs { get; set; }

        public bool IsDirty { get; set; }

        public int Number { get; set; }

        public Order Order { get; set; }

        public void NewOrder()
        {
            this.Order = new Order
            {
                TableNumber = this.Number,
                OrderState = Order.State.New
            };
        }

        //todo: metoda zwracająca róg stołu do któergo ma podejść kelner
    }
}
