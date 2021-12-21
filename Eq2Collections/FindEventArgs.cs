using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eq2Collections
{
    class FindEventArgs : EventArgs
    {
        public string name;

        public FindEventArgs(string name)
        {
            this.name = name;
        }
    }
}
