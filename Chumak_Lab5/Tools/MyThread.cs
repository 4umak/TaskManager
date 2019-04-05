using System;
using System.Diagnostics;

namespace Chumak_Lab5.Tools
{
    public class MyThread
    {
        private readonly ProcessThread _thread;

        public int Id => _thread.Id;
        public ThreadState State => _thread.ThreadState;
        public DateTime LaunchDateTime => _thread.StartTime;

        internal MyThread(ProcessThread thread)
        {
            this._thread = thread;
        }

        public override bool Equals(object obj)
        {
            return obj is MyThread another && _thread.Equals(another._thread);
        }

        public override int GetHashCode()
        {
            return _thread.GetHashCode();
        }
    }
}