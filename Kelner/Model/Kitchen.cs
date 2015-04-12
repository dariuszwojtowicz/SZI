namespace Kelner.Model
{
    using System.Threading;

    public class Kitchen
    {
        private Thread thread;

        public Kitchen()
        {
            this.thread = new Thread(this.WorkerFunc);
        }

        public void Start()
        {
            this.thread.Start();
        }

        public event NewMealEventHandler NewMealEvent;

        protected virtual void OnNewMealAppeared(NewMealEventArgs e)
        {
            this.NewMealEvent(this, e);
        }

        private void WorkerFunc()
        {
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(1000);
                NewMealEventArgs e = new NewMealEventArgs(i);
                this.OnNewMealAppeared(e);
            }
        }
    }

    public delegate void NewMealEventHandler(object sender, NewMealEventArgs e);

    public class NewMealEventArgs
    {
        public int MealCount { get; set; }

        public NewMealEventArgs(int i)
        {
            this.MealCount = i;
        }
    }
}

