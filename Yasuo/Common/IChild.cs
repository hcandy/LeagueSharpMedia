using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasuo.Common
{
    public interface IChild
    {
        bool Enabled { get; }
        string Name { get; }
        bool Initialized { get; }
        bool Unloaded { get; }
        bool Handled { get; }
        void HandleEvents();
    }
}
