using System;

namespace Wissance.WebApiToolkit.Core.Operations
{
    [Flags]
    public enum ControllerOperation
    {
        None    = 0,
        Read    = 1,
        ReadOne = 2, 
        Create  = 4,
        Update  = 8,
        Delete  = 16,
        BulkCreate = 32,
        BulkUpdate = 64,
        BulkDelete = 128,
    }
}