using Nll_Unlocker.Classes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Nll_Unlocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MyProcess> processes = new ObservableCollection<MyProcess>();


        public MainWindow()
        {
            InitializeComponent();
            ProcessGrid.ItemsSource = processes;

            Loaded += async (s, e) => await LoadProcesses();
        }

        private Task LoadProcesses()
        {
            processes.Clear();
            foreach (var p in Process.GetProcesses())
            {
                var proc = new MyProcess
                {
                    ProcessName = p.ProcessName,
                    Id = p.Id,
                    Icon = ProcessManager.GetProcessIcon(p),
                    IsCritical = ProcessManager.IsCritical(p),
                    Risk = 0,
                    CmdLine = "Loading..."
                };

                processes.Add(proc);

                _ = Task.Run(async () =>
                {
                    proc.Risk = await ProcessManager.EvaluateRiskAsync(p);
                    proc.CmdLine = await ProcessManager.GetCommandLineAsync(p);
                });
            }

            return Task.CompletedTask;
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadProcesses();
        }

        private async void EndProcess_Click(object sender, RoutedEventArgs e)
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
                    await LoadProcesses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}