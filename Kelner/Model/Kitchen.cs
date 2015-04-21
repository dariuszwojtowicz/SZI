namespace Kelner.Model
{
    using System.Threading;

    public class KitchenWorker
    {
        private Thread thread;

        private bool shouldStop;

        public KitchenWorker()
        {
            this.thread = new Thread(this.WorkerFunc);
        }

        public void Start()
        {
            this.thread = new Thread(this.WorkerFunc);
            this.shouldStop = false;
            this.thread.Start();
        }

        public void Stop()
        {
            this.shouldStop = true;
            Thread.Sleep(20);
            this.thread.Join();
        }

        public event NewMealEventHandler NewMealEvent;

        protected virtual void OnNewMealAppeared(NewMealEventArgs e)
        {
            this.NewMealEvent(this, e);
        }

        private void WorkerFunc()
        {
            var i = 0;
            while (!this.shouldStop)
            {
                i++;
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

