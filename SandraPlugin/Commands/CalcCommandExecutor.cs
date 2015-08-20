using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;
using WolframAlphaNET.Objects;

namespace SandraPlugin.Commands
{
    public class CalcCommandExecutor : ICommandExecutor
    {
        private SandraPlugin plugin;
        public CalcCommandExecutor(SandraPlugin plugin)
        {
            this.plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (plugin.WolframAlphaClient == null)
            {
                executor.SendMessage("Wolfram has not been configured");
                return true;
            }

            String s = "";
            foreach (var arg in data.Arguments)
            {
                if (s.Equals(""))
                {
                    s = arg;
                    continue;
                }
                s += " " + arg;
            }
            QueryResult results = plugin.WolframAlphaClient.Query(s);
            if (results != null)
            {
                int limit = 3;
                int i = 1;
                foreach (Pod pod in results.Pods)
                {
                    if (pod.SubPods != null)
                    {
                        foreach (SubPod subPod in pod.SubPods)
                        {
                            String podTitle = String.IsNullOrWhiteSpace(pod.Title) ? "" : pod.Title;
                            if (i == 1)
                            {
                                podTitle = "Input";
                            }
                            String title = String.IsNullOrWhiteSpace(subPod.Title) ? podTitle : subPod.Title;
                            String plainText = String.IsNullOrWhiteSpace(subPod.Plaintext) ? "" : subPod.Plaintext;
                            bool sendSeperator = !String.IsNullOrWhiteSpace(title) &&
                                        !String.IsNullOrWhiteSpace(subPod.Plaintext);
                            String sep = sendSeperator ? ": " : "";
                            data.SendResponse(title + sep + plainText);
                        }
                    }
                    i++;
                    if (i > limit)
                    {
                        break;
                    }
                }

                if (results.Error != null && !String.IsNullOrWhiteSpace(results.Error.Message))
                {
                    data.SendResponse(ColorCode.RED + "Error: " + results.Error.Message);
                } else if (results.Pods.Count == 0)
                {
                    data.SendResponse(ColorCode.RED + "Error: empty response");
                }
                return true;
            }

            data.SendResponse("Error: No result received!");
            return true;
        }
    }
}