using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerControl_WPF_API {
    public class CodeExampleGroupChildrenSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            if (item is CodeExampleGroup)
                return ((CodeExampleGroup)item).Examples;
            return null;
        }
    }
}
