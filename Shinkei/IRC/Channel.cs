using System;
using System.Threading;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace Shinkei.IRC
{
    public class Channel
    {
        readonly Server _server;

        private bool _inChannel;
        public bool InChannel
        {
            get
            {
                return _inChannel;
            }
        }

        private readonly string _name;
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
            _server = server;
            _name = name;
            _key = key;

            Eventsink.GetInstance().OnIrcJoin += OnIrcJoin;
            Eventsink.GetInstance().OnIrcKick += OnIrcKick;
            Eventsink.GetInstance().OnIrcPart += OnIrcPart;
        }

        private void OnIrcJoin(JoinMessage data)
        {
            Console.WriteLine("Channel.OnIrcJoin");

            EntUser msgUser = (EntUser)data.Sender;
            EntChannel msgChannel = (EntChannel)data.Recipient;

            if (msgChannel.Name == Name)
            {
                if (msgUser.Nickname == data.ServerInstance.LocalSettings.Nickname) {
                    _inChannel = true;
                }

                // dispatch queued event
                Eventsink.GetInstance().OnIrcQueuedJoin(data);
            }
        }

        private void OnIrcKick(KickMessage data)
        {
            Console.WriteLine("Channel.OnIrcKick");

            //EntUser msgKicker = (EntUser)data.Sender;
            EntUser msgKickedOne = (EntUser)data.Recipient;
            EntChannel msgChannel = data.Channel;

            if (msgChannel.Name == Name)
            {
                if (msgKickedOne.Nickname == data.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = false;
                }

                // dispatch queued event
                Eventsink.GetInstance().OnIrcQueuedKick(data);
            }
        }

        private void OnIrcPart(PartMessage data)
        {
            Console.WriteLine("Channel.OnIrcPart");

            EntUser msgUser = (EntUser)data.Sender;
            EntChannel msgChannel = (EntChannel)data.Recipient;

            if (msgChannel.Name == Name)
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
                    Thread.Sleep(interval);
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
                    Thread.Sleep(interval);
                    counter--;
                }
            }

            return !(_inChannel);
        }
    }
}
