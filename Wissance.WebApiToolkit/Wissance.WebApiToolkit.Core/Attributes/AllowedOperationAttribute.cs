using System;
using Wissance.WebApiToolkit.Core.Operations;

namespace Wissance.WebApiToolkit.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AllowedOperationAttribute : Attribute
    {
        public AllowedOperationAttribute(ControllerOperation operations)
        {
            Operations = operations;
        }
        
        public ControllerOperation Operations { get; internal set; }
    }
}