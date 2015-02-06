using System;
using System.Threading;
using Shinkei.API.Events;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;

namespace Shinkei.IRC
{
    public class Channel : IListener
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

            EventManager.GetInstance().RegisterEvents(this, ShinkeiPlugin.GetInstance());
        }

        [Events.EventHandler(Priority = EventPriority.MONITOR)]
        private void OnIrcJoin(IrcJoinEvent evnt)
        {
            Console.WriteLine("Channel.OnIrcJoin");

            EntUser msgUser = (EntUser)evnt.Sender;
            EntChannel msgChannel = (EntChannel)evnt.Recipient;

            if (msgChannel.Name == Name)
            {
                if (msgUser.Nickname == evnt.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = true;
                }
            }
        }

        [Events.EventHandler(Priority = EventPriority.MONITOR)]
        private void OnIrcKick(IrcKickEvent evnt)
        {
            Console.WriteLine("Channel.OnIrcKick");

            //EntUser msgKicker = (EntUser)data.Sender;
            EntUser msgKickedOne = (EntUser)evnt.Recipient;
            EntChannel msgChannel = evnt.Channel;

            if (msgChannel.Name == Name)
            {
                if (msgKickedOne.Nickname == evnt.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = false;
                }
            }
        }

        [Events.EventHandler(Priority = EventPriority.MONITOR)]
        private void OnIrcPart(IrcPartEvent evnt)
        {
            Console.WriteLine("Channel.OnIrcPart");

            EntUser msgUser = (EntUser)evnt.Sender;
            EntChannel msgChannel = (EntChannel)evnt.Recipient;

            if (msgChannel.Name == Name)
            {
                if (msgUser.Nickname == evnt.ServerInstance.LocalSettings.Nickname)
                {
                    _inChannel = false;
                }
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
