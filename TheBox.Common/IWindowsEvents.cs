using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.Common
{
    public interface IWindowsEvents
    {
        void OnShuttingDown(object sender);
    }
}
