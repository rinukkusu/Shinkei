using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC
{
    public class User : IEntity
    {
        string Name;
        string Login;
        string Hostmask;

        public string GetName()
        {
            return Name;
        }
    }
}
