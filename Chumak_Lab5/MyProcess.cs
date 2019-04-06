using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Chumak_Lab5.Tools;

namespace Chumak_Lab5
{
    public class MyProcess
    {
        private readonly Process _process;
        public string Name => _process.ProcessName;
        public int Id => _process.Id;
        public bool IsActive => _process.Responding;
        public float Cpu => 0;
        public long Memory => _process.PrivateMemorySize64 / 1024;
        public int ThreadsCount => _process.Threads.Count;
        public string User => _process.MachineName;
        private ObservableCollection<MyModule> _modules;
        private ObservableCollection<MyThread> _threads;
        public string Path
        {
            get
            {
                try
                {
                    return _process.MainModule.FileName;
                }
                catch (Exception)
                {
                    return "Acces denied!";
                }
            }
        }

        public DateTime LaunchDateTime
        {
            get
            {
                try
                {
                    return _process.StartTime;
                }
                catch (Exception)
                {

                    return new DateTime(1960, 1, 1);
                }
            }
        }

        public override bool Equals(object obj)
    {
        return obj is MyProcess another && Id == another.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
    public ObservableCollection<MyThread> Threads
    {
        get
        {
            if (_threads == null)
                RefreshThreads();
            return _threads;
        }
    }
    public ObservableCollection<MyModule> Modules
    {
        get
        {
            if (_modules == null)
                RefreshModules();
            return _modules;
        }
    }

    internal MyProcess(Process process)
    {
        this._process = process;
    }

    internal void RefreshModules()
    {
        // if (_modules == null)
        _modules = new ObservableCollection<MyModule>();
        try
        {
            foreach (ProcessModule m in _process.Modules)
            {
                _modules.Add(new MyModule(m));
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    internal void RefreshThreads()
    {
        //if (_threads == null)
        _threads = new ObservableCollection<MyThread>();
        try
        {
            foreach (ProcessThread t in _process.Threads)
            {
                _threads.Add(new MyThread(t));
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
}