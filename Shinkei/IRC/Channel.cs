using Shinkei.IRC.Entities;
using System;

namespace Shinkei.IRC
{
    public class Channel
    {
        Server _Server;

        private bool _InChannel;
        public bool InChannel
        {
            get
            {
                return _InChannel;
            }
        }

        private string _Name;
        public string Name
        { 
            get
            {
                return _Name;
            }
        }

        private string _Key;
        public string Key
        {
            get
            {
                return _Key;
            }
        }

        public Channel(Server Server, string Name, string Key = "")
        {
            this._Server = Server;
            this._Name = Name;
            this._Key = Key;

            Eventsink.GetInstance().OnIrcJoin += new Eventsink.IrcJoinDelegate(OnIrcJoin);
            Eventsink.GetInstance().OnIrcKick += new Eventsink.IrcKickDelegate(OnIrcKick);
            Eventsink.GetInstance().OnIrcPart += new Eventsink.IrcPartDelegate(OnIrcPart);
        }

        private void OnIrcJoin(Messages.JoinMessage data)
        {
            Console.WriteLine("Channel.OnIrcJoin");

            EntUser MsgUser = (EntUser)data.Sender;
            EntChannel MsgChannel = (EntChannel)data.Recipient;

            if (MsgChannel.Name == this.Name)
            {
                if (MsgUser.Nickname == data.ServerInstance.localSettings.Nickname) {
                    _InChannel = true;
                }
            }

            Eventsink.GetInstance().OnIrcQueuedJoin(data);
        }

        private void OnIrcKick(Messages.KickMessage data)
        {
            Console.WriteLine("Channel.OnIrcKick");

            EntUser MsgKicker = (EntUser)data.Sender;
            EntUser MsgKickedOne = (EntUser)data.Recipient;
            EntChannel MsgChannel = (EntChannel)data.Channel;

            if (MsgChannel.Name == this.Name)
            {
                if (MsgKickedOne.Nickname == data.ServerInstance.localSettings.Nickname)
                {
                    _InChannel = false;
                }
            }

            Eventsink.GetInstance().OnIrcQueuedKick(data);
        }

        private void OnIrcPart(Messages.PartMessage data)
        {
            Console.WriteLine("Channel.OnIrcPart");

            EntUser MsgUser = (EntUser)data.Sender;
            EntChannel MsgChannel = (EntChannel)data.Recipient;

            if (MsgChannel.Name == this.Name)
            {
                if (MsgUser.Nickname == data.ServerInstance.localSettings.Nickname)
                {
                    _InChannel = false;
                }
            }

            Eventsink.GetInstance().OnIrcQueuedPart(data);
        }

        public bool Join(string Key = null)
        {
            if (!_InChannel)
            {
                if (Key != null)
                {
                    _Key = Key;
                }

                _Server.WriteLine("JOIN " + _Name + " " + _Key);

                int Interval = 250;
                int Counter = (1000 / Interval) * 5; // wait for 5 seconds

                while ((Counter > 0) && (!_InChannel))
                {
                    System.Threading.Thread.Sleep(Interval);
                    Counter--;
                }
            }

            return _InChannel;
        }
    }
}
