namespace Kelner.Model
{
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Klasa reprezentująca kolejkę przychodzących klientów
    /// </summary>
    public class ClientsQueueWorker
    {
        private Thread thread;

        private bool shouldStop;

        public ClientsQueueWorker()
        {
            this.thread = new Thread(this.ClientsWorkerFunc);
            this.WaitingClients = new List<Client>();
        }

        public void Start()
        {
            this.WaitingClients = new List<Client>();
            this.thread = new Thread(this.ClientsWorkerFunc);
            this.shouldStop = false;
            this.thread.Start();
        }

        public void Stop()
        {
            this.shouldStop = true;
            Thread.Sleep(20);
            this.thread.Join();
        }

        public List<Client> WaitingClients { get; set; }

        public event NewClientEventHandler NewClientEvent;

        protected virtual void OnNewClientAppeared(NewClientEventArgs e)
        {
            this.NewClientEvent(this, e);
        }

        private void ClientsWorkerFunc()
        {
            while (!this.shouldStop)
            {
                var newClient = new Client
                {
                    CanSplit = false,
                    PeopleCount = 2
                };
                this.WaitingClients.Add(newClient);
                NewClientEventArgs e = new NewClientEventArgs(this.WaitingClients.Count);
                this.OnNewClientAppeared(e);

                Thread.Sleep(10000);
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
}
