﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Shinkei
{
    public class JsonHelper
    {
        private const string IndentString = "    ";
        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(IndentString));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(IndentString));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(IndentString));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static T Deserialize<T>(string path)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            FileStream settingsFile = File.Open(path, FileMode.Open);
            T settings = (T)serializer.ReadObject(settingsFile);
            settingsFile.Close();

            return settings;
        }

        public static void Serialize<T>(object _object, string path)
        {
            FileStream newFile = File.Create(path);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(newFile, _object);
            newFile.Close();

            string reformat = File.ReadAllText(path);
            File.WriteAllText(path, FormatJson(reformat));
        }

        public static T DeserializeFromString<T>(string JsonText)
        {
            try
            {
                JsonText = JsonText.Replace("\0", "");
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(T));

                MemoryStream MemStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonText));
                T myObject = (T)Serializer.ReadObject(MemStream);

                return myObject;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string SerializeToString<T>(object _object)
        {
            try
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(T));

                MemoryStream MemStream = new MemoryStream();
                Serializer.WriteObject(MemStream, _object);

                MemStream.Seek(0, SeekOrigin.Begin);

                StreamReader MemReader = new StreamReader(MemStream);
                string JsonText = MemReader.ReadToEnd();

                return JsonText;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }

    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }
}
