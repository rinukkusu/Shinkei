using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC
{
    public class Channel : IEntity
    {
        string Name;

        public string GetName()
        {
            return Name;
        }
    }
}
