﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei;
using System.ComponentModel.Composition;
using Shinkei.IRC;
using Shinkei.IRC.Messages;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;

[Export(typeof(Shinkei.IPlugin))]
[ExportMetadata("Name", "Rejoin")]
[ExportMetadata("Version", "0.1")]
[ExportMetadata("Author", "rinukkusu")]
public class RejoinPlugin : IPlugin
{
    public static Settings m_Settings = LoadSettings();

    public bool IsEnabled()
    {
        return true;
    }

    public void RegisterEvents(Eventsink Eventdata)
    {
        Eventdata.OnIrcQueuedKick += new Eventsink.IrcQueuedKickDelegate(this.IrcKickHandler);
    }

    public void IrcKickHandler(KickMessage MessageData)
    {
        Console.WriteLine("RejoinPlugin.IrcKickHandler");

        Thread.Sleep(RejoinPlugin.m_Settings.WaitUntilRejoin);
        MessageData.ServerInstance.Channels[MessageData.Channel.Name].Join();
    }

    public static Settings LoadSettings()
    {
        DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Settings));

        string Path = "plugins/RejoinPlugin.json";
        FileStream SettingsFile;
        Settings newSettings;

        if (!File.Exists(Path))
        {
            newSettings = new Settings();
            newSettings.WaitUntilRejoin = 2000;

            SettingsFile = File.Create(Path);
            Serializer.WriteObject(SettingsFile, newSettings);
        }
        else
        {
            SettingsFile = File.Open(Path, FileMode.Open);
            newSettings = (Settings)Serializer.ReadObject(SettingsFile);
        }

        SettingsFile.Close();

        return newSettings;
    }
}

[DataContractAttribute]
public class Settings
{
    [DataMemberAttribute]
    public int WaitUntilRejoin;
}