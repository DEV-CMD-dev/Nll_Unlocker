using Nll_Unlocker.Classes;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nll_Unlocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MyProcess> processes = new List<MyProcess>();

        public MainWindow()
        {
            InitializeComponent();
            LoadProcesses();
        }

        private void LoadProcesses()
        {
            processes.Clear();
            foreach (var p in Process.GetProcesses())
            {
                processes.Add(new MyProcess
                {
                    ProcessName = p.ProcessName,
                    Id = p.Id,
                    CmdLine = ProcessManager.GetCommandLine(p),
                    IsCritical = ProcessManager.IsCritical(p)
                });
            }

            ProcessGrid.ItemsSource = null;
            ProcessGrid.ItemsSource = processes;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProcesses();
        }

        private void EndProcess_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessGrid.SelectedItem is MyProcess selected)
            {
                if (selected.IsCritical)
                {
                    MessageBox.Show("Cannot end a critical process!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var proc = Process.GetProcessById(selected.Id);
                    proc.Kill();
                    MessageBox.Show($"Process {selected.ProcessName} ended.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProcesses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}