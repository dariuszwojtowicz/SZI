namespace Kelner.Model
{
    using System.Threading;

    public class ClientsQueue
    {
        private Thread thread;

        private bool shouldStop;

        public ClientsQueue()
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

        public event NewClientEventHandler NewClientEvent;

        protected virtual void OnNewClientAppeared(NewClientEventArgs e)
        {
            this.NewClientEvent(this, e);
        }

        private void WorkerFunc()
        {
            while (!this.shouldStop)
            {
                Thread.Sleep(1000);
                NewClientEventArgs e = new NewClientEventArgs(2);
                this.OnNewClientAppeared(e);

            }
        }
    }

    public delegate void NewClientEventHandler(object sender, NewClientEventArgs e);

    public class NewClientEventArgs
    {
        public int ClientCount { get; set; }

        public NewClientEventArgs(int i)
        {
            this.ClientCount = i;
        }
    }
    
    //todo generowanie klientów
}
