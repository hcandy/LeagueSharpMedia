using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp.Common;

namespace Yasuo.Common.Extensions
{
    public static class MenuExtensions
    {
        public static void AddToolTip(Menu menu, string helpText)
        {
            menu.AddItem(new MenuItem(menu.Name +" Helper", "Helper").SetTooltip(helpText));
        }
    }
}
