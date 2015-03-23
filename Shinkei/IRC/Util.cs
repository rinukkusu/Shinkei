using System;
using System.Collections.Generic;
using Shinkei.IRC.Exceptions;

namespace Shinkei.IRC
{
    class Util
    {
        public static Boolean IsReportable(Exception e)
        {
#if DEBUG
            return true;
#else
            return e is IReportable;
#endif
        }

        public static List<string> ParseArguments(string argumentString)
        {
            List<string> arguments = new List<string>();
            argumentString = argumentString.Trim();

            string singleArgument = "";
            bool bInQuotes = false;
            foreach (char c in argumentString)
            {
                if (c == '\'')
                {
                    bInQuotes = !bInQuotes;
                }

                if (c == ' ')
                {
                    if (bInQuotes)
                    {
                        singleArgument += c.ToString();
                    }
                    else
                    {
                        arguments.Add(singleArgument);
                        singleArgument = "";
                    }
                }
                else
                {
                    singleArgument += c.ToString();
                }
            }

            if (singleArgument.Length > 0)
            {
                arguments.Add(singleArgument);
            }

            return arguments;
        }
    }
}
