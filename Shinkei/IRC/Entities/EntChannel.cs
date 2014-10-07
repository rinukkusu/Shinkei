using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Entities
{
    public class EntChannel : IEntity
    {
        private string _Name;
        public string Name
        { 
            get
            {
                return _Name;
            }
        }

        public EntChannel (string Name)
        {
            this._Name = Name;
        }
    }
}
