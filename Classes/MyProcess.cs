using System.Windows.Media;

namespace Nll_Unlocker.Classes
{
    internal class MyProcess
    {
        public ImageSource Icon { get; set; }
        public string ProcessName { get; set; }
        public string CmdLine { get; set; }
        public int Id { get; set; }
        public bool IsCritical { get; set; } = false;
        public int Risk { get; set; }
    }
}
