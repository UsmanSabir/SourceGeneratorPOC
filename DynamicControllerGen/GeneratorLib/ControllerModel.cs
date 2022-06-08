using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorLib
{
    internal record ControllerModel(string NameSpace, string ControllerName, ActionRoute[] Actions);
}
