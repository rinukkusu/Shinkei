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
            int p = (int) Environment.OSVersion.Platform;
            return p == (int) PlatformID.Win32NT || p == (int) PlatformID.Win32S || p == (int) PlatformID.Win32Windows;
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
                Console.WriteLine("Couldn't load plugins: " + e.Message);

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
    }
}
