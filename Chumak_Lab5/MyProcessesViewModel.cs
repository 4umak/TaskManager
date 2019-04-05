using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Chumak_Lab5.Tools;

namespace Chumak_Lab5
{
    class MyProcessesViewModel : INotifyPropertyChanged
    {
        private RelayCommand _endTaskCommand;
        private readonly CancellationToken _token;
        private RelayCommand _openFileLocationCommand;
        public bool IsItemSelected => SelectedProcess != null;
        private MyProcess _selectedProcess;
        public ObservableCollection<MyProcess> ProcessesList { get; set; }
        public ObservableCollection<MyModule> Modules { get; set; }
        public RelayCommand OpenCommand => _openFileLocationCommand ?? (_openFileLocationCommand = new RelayCommand(OpenFileLocationImpl));
        public RelayCommand EndTaskCommand => _endTaskCommand ?? (_endTaskCommand = new RelayCommand(EndTaskImpl));
        private async void GetInfoImpl(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        var process = Process.GetProcessById(SelectedProcess.Id);
                        try
                        {
                            Modules = (new MyProcess(process)).Modules;
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                }, _token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
                var process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    Process.Start("explorer.exe", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }, _token);
        }


        internal MyProcessesViewModel()
        {
            var dueTime = TimeSpan.FromSeconds(0);
            var interval = TimeSpan.FromSeconds(5);
            var tokenSource = new CancellationTokenSource();
            _token = tokenSource.Token;
            StartBackgroundTask(dueTime, interval);
        }

        private async void StartBackgroundTask(TimeSpan dueTime, TimeSpan interval)
        {
            await RunPeriodicAsync(OnTick, dueTime, interval, _token);
        }

        private static async Task RunPeriodicAsync(Action onTick,
            TimeSpan dueTime,
            TimeSpan interval,
            CancellationToken token)
        {
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            while (!token.IsCancellationRequested)
            {
                onTick?.Invoke();
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }
        private async void OnTick()
        {
            await Task.Run(() =>
            {
                GetProcesses();
                RefreshMetaData();
            }, _token);
        }

        private void RefreshMetaData()
        {
            foreach (var p in ProcessesList)
            {
                p.RefreshModules();
                p.RefreshThreads();
            }
        }

        private void GetProcesses()
        {
            var processes = Process.GetProcesses();
            ProcessesList = new ObservableCollection<MyProcess>();
            foreach (var process in processes)
            {
                ProcessesList.Add(new MyProcess(process));
            }
            OnPropertyChanged();
            OnPropertyChanged($"ProcessesList");
        }

        private void EndTaskImpl(object o)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var process = Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    process.Kill();
                    ProcessesList.Remove(SelectedProcess);
                    GetProcesses();
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        public MyProcess SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
                OnPropertyChanged($"IsItemSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}