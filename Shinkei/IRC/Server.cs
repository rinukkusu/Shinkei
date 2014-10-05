using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Shinkei.IRC
{
    class Server
    {
        List<Channel> Channels;
        string Host;
        int Port;
        string NickservPassword;
        Thread TReader;
        TcpClient Socket;

        public Server(string _Host, int _Port)
        {
            Host = _Host;
            Port = _Port;
        }

        private void ReadThread()
        {

        }

        public void Connect()
        {
            Socket = new TcpClient();

            try
            {
                Socket.Connect(Host, Port);
                if (Socket.Connected)
                {
                    TReader = new Thread(ReadThread);
                    TReader.Start();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Socket.Close();
                }
                catch { }
                Disconnect();
            }
        }

        public void Disconnect()
        {
            TReader.Abort();
        }

        public void Reconnect()
        {
            Disconnect();
            Reconnect();
        }

        public void JoinChannel(string _Channel)
        {
            
        }
    }
}
