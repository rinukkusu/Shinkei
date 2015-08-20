using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shinkei.API;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Events;
using System.Reflection;

namespace Shinkei
{
    partial class Program
    {
        private static Boolean _stop;
        private static Program _instance;
        static void Main()
        {
           _instance = new Program();
        }

        public Program GetInstance()
        {
            return _instance;
        }

        public Program()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

            if (IsWindows())
            {
                MainWindows();
            }
            else if (IsUnix())
            {
                MainUnix();
            }
            Thread @shinkeiThread = new Thread(StartShinkei);
            @shinkeiThread.Start();

            while (!_stop)
            {
                String line = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string command = line.Split(' ')[0];
                    List<string> arguments = line.Split(' ').ToList();
                    arguments.RemoveAt(0);

                    ConsoleCommandEvent evnt = new ConsoleCommandEvent(command, arguments);
                    EventManager.GetInstance().CallEvent(evnt);
                }
            }
        }

        public static bool IsWindows()
        {
            if (IsMono())
            {
                return false;
            }

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
                    break;
                case PlatformID.Unix:
                    //Unix
                    break;
                case PlatformID.MacOSX:
                    //Mac OSX
                    break;
                default:
                    ///????
                    break;
            }

            return false;
        }

        public static bool IsUnix()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public void Stop()
        {
            _stop = true;
            _instance = null;
            OnClose();
        }

        private void OnClose()
        {
            List<Server> servers;
            try
            {
                servers = Server.GetServers();
            }
            catch (Exception)
            {
                return;
            }

            foreach (Server server in servers)
            {
                try
                {
                    server.Quit();
                }
                catch (Exception)
                {

                }
            }
        }

        private void StartShinkei()
        {
            PluginContainer myPluginContainer = PluginContainer.GetInstance();
            try
            {
                myPluginContainer.LoadPlugins();
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't load plugins: " + e);

                if (e is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = e as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    foreach (var loaderException in loaderExceptions)
                    {
                        Console.WriteLine("  >>" + loaderException.Message);
                    }
                }
            }
            foreach (Plugin plugin in myPluginContainer.Plugins)
            {
                plugin.Enable();
            }

            SettingsLoader mySettings = SettingsLoader.GetInstance();
            mySettings.Load();

            mySettings.EnforceSettings();
        }
        private Assembly MyResolveEventHandler (object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.
            Assembly MyAssembly;
            string strTempAssmbPath = "";
            
            //Build the path of the assembly from where it has to be loaded.
            string baseFolder = Assembly.GetExecutingAssembly ().Location;
            baseFolder = baseFolder.Substring (0, baseFolder.LastIndexOf (System.IO.Path.DirectorySeparatorChar));

            //If resolving for a plugin, remove the "plugins" folder, too
            string[] lastFolder = baseFolder.Split (System.IO.Path.DirectorySeparatorChar);
            if (lastFolder [lastFolder.Length - 1] == "plugins") 
            {
                baseFolder = baseFolder.Substring (0, baseFolder.LastIndexOf (System.IO.Path.DirectorySeparatorChar));
            }

            //Now assemble the paths
            try {
                string normalizedName = args.Name.Substring (0, args.Name.IndexOf (","));
                if (normalizedName.ToLower ().Contains ("plugin")) 
                {
                    strTempAssmbPath = System.IO.Path.Combine (baseFolder, "plugins", normalizedName + ".dll");
                } 
                else 
                {
                    strTempAssmbPath = System.IO.Path.Combine (baseFolder, "lib", normalizedName + ".dll");
                }
            }
            catch (Exception exi)
            {
                Console.WriteLine (exi.Message);
            }

            //Load the assembly from the specified path.                    
            MyAssembly = Assembly.LoadFrom(strTempAssmbPath);
            
            //Return the loaded assembly.
            return MyAssembly;
        }

        public static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
