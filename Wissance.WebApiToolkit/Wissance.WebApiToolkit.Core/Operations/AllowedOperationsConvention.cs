using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Wissance.WebApiToolkit.Core.Attributes;
using Wissance.WebApiToolkit.Core.Controllers;

namespace Wissance.WebApiToolkit.Core.Operations
{
    public class AllowedOperationsConvention: IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            ControllerModel controller = action.Controller;
            AllowedOperationAttribute attr = controller.ControllerType.GetCustomAttribute<AllowedOperationAttribute>();
            // if there is no attribute, all operations are allowed
            if (attr == null) 
                return;

            // All these names takes from
            ControllerOperation operation = action.ActionMethod.Name switch
            {
                // from BasicReadController
                "ReadAsync" => ControllerOperation.Read,
                "ReadByIdAsync" => ControllerOperation.ReadOne,
                // from BasicCrudController
                "CreateAsync" => ControllerOperation.Create,
                "UpdateAsync" => ControllerOperation.Update,
                "DeleteAsync" => ControllerOperation.Delete,
                // from BasicBulkController
                "BulkCreateAsync" => ControllerOperation.BulkCreate,
                "BulkUpdateAsync" => ControllerOperation.BulkUpdate,
                "BulkDeleteAsync" => ControllerOperation.BulkDelete,
                _ => ControllerOperation.None
            };

            // Если операция не определена или не разрешена – удаляем действие
            if (operation == ControllerOperation.None || !attr.Operations.HasFlag(operation))
            {
                action.ActionName = null;            // убираем имя действия
                action.Selectors.Clear();            // удаляем все маршруты
                action.ApiExplorer.IsVisible = false; // скрываем из Swagger
            }
        }
    }
}