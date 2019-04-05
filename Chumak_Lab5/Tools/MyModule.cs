using System.Diagnostics;

namespace Chumak_Lab5.Tools
{
    public class MyModule
    {
        private readonly ProcessModule _module;

        public string Name => _module.ModuleName;
        public string Path => _module.FileName;

        internal MyModule(ProcessModule module)
        {
            this._module = module;
        }

        public override bool Equals(object obj)
        {
            return obj is MyModule another && _module.Equals(another._module);
        }

        public override int GetHashCode()
        {
            return _module.GetHashCode();
        }
    }
}