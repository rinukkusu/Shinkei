namespace Shinkei.IRC.Entities
{
    public abstract class ServerEntity : IEntity
    {

        public abstract string GetName();

        public virtual void SendMessage(string s)
        {
            Server.PrivateMessage(this, s);
        }

        private readonly Server _server;
        public Server Server
        {
            get { return _server; }
        }

        protected ServerEntity(Server server)
        {
            _server = server;
        }
    }
}