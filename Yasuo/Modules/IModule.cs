using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace Yasuo.Modules
{
    using Yasuo.Common.Utility.Enums;

    interface IModule
    {
        ModuleMode GetModuleMode();

        void Execute(EventArgs args);
    }
}