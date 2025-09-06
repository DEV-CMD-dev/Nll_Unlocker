using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nll_Unlocker.Classes
{
    internal class MyProcess
    {
        public string ProcessName { get; set; }
        public string CmdLine { get; set; }
        public int Id { get; set; }
        public bool IsCritical { get; set; } = false;
    }
}
