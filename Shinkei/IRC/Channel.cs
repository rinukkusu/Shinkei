using Shinkei.IRC.Entities;
using System;

namespace Shinkei.IRC
{
    public class Channel
    {
        Server _server;

        private bool _inChannel;
        public bool InChannel
        {
            get
            {
                return _inChannel;
            }
        }

        private string _name;
        public string Name
        { 
            get
            {
                return _name;
            }
        }

        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
        }

        public Channel(Server server, string name, string key = "")
        {
            this._server = server;
            this._name = name;
            this._key = key;

            Eventsink.GetInstance().OnIrcJoin += new Eventsink.IrcJoinDelegate(OnIrcJoin);
            Eventsink.GetInstance().OnIrcKick += new Eventsink.IrcKickDelegate(OnIrcKick);
            Eventsink.GetInstance().OnIrcPart += new Eventsink.IrcPartDelegate(OnIrcPart);
        }

        private void OnIrcJoin(Messages.JoinMessage data)
        {
            Console.WriteLine("Channel.OnIrcJoin");

            EntUser msgUser = (EntUser)data.Sender;
            EntChannel msgChannel = (EntChannel)data.Recipient;

            if (msgChannel.Name == this.Name)
            {
                if (msgUser.Nickname == data.ServerInstance.LocalSettings.Nickname) {
                    _inChannel = true;
                }

                // dispatch queued event
                Eventsink.GetInstance().OnIrcQueuedJoin(data);
            }
        }

        private void OnIrcKick(Messages.KickMessage data)
        {
            Console.WriteLine("Channel.OnIrcKick");

            EntUser msgKicker = (EntUser)data.Sender;
            EntUser msgKickedOne = (EntUser)data.Recipient;
            EntChannel msgChannel = (EntChannel)data.Channel;

            if (msgChannel.Name == this.Name)
            {
                if (msgKickedOne.Nickname == data.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = false;
                }

                // dispatch queued event
                Eventsink.GetInstance().OnIrcQueuedKick(data);
            }
        }

        private void OnIrcPart(Messages.PartMessage data)
        {
            Console.WriteLine("Channel.OnIrcPart");

            EntUser msgUser = (EntUser)data.Sender;
            EntChannel msgChannel = (EntChannel)data.Recipient;

            if (msgChannel.Name == this.Name)
            {
                if (msgUser.Nickname == data.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = false;
                }

                // dispatch queued event
                Eventsink.GetInstance().OnIrcQueuedPart(data);
            }
        }

        public bool Join(string key = null)
        {
            if (!_inChannel)
            {
                if (key != null)
                {
                    _key = key;
                }

                _server.WriteLine("JOIN " + _name + " " + _key);

                int interval = 250;
                int counter = (1000 / interval) * 5; // wait for 5 seconds

                while ((counter > 0) && (!_inChannel))
                {
                    System.Threading.Thread.Sleep(interval);
                    counter--;
                }
            }

            return _inChannel;
        }

        public bool Part()
        {
            if (_inChannel)
            {
                _server.WriteLine("PART " + _name);

                int interval = 250;
                int counter = (1000 / interval) * 5; // wait for 5 seconds

                while ((counter > 0) && (_inChannel))
                {
                    System.Threading.Thread.Sleep(interval);
                    counter--;
                }
            }

            return !(_inChannel);
        }
    }
}
