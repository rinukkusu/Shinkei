using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Exceptions
{
    interface IReportable
    {
        String GetReportableMessage();
    }
}
