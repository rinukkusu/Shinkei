using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;

namespace Shinkei
{
    public partial class Program
    {
        public void MainUnix()
        {      
            //Signs to listen
            UnixSignal[] signals = {
                new UnixSignal(Signum.SIGINT),
                new UnixSignal(Signum.SIGTERM),
                new UnixSignal(Signum.SIGQUIT),
                new UnixSignal(Signum.SIGABRT)
            };

            Thread signalthread = new Thread(delegate ()
            {
                int index = UnixSignal.WaitAny(signals);
                Signum signal = signals[index].Signum;
                Stop("Received Unix Signal: " + signal.GetType().Name.ToUpper());
            });
            signalthread.Start();
        }
    }
}
