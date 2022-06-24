using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorLib
{
    internal record ControllerModel(string NameSpace, string ControllerName, 
        string ClassName, string ClassFullName, ActionRoute[] Actions);

    //internal class ControllerModel
    //{
    //   public string NameSpace { get; }

    //    public ControllerModel(string nameSpace, string controllerName, ActionRoute[] actions)
    //    {
    //        NameSpace = nameSpace;
    //        ControllerName = controllerName;
    //        Actions = actions;
    //    }

    //    public string ControllerName { get; }
    //    public ActionRoute[] Actions { get; }                
    //}   
}
