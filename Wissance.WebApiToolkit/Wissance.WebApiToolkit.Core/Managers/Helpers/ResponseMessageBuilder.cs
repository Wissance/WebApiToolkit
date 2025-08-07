using System;
using Wissance.WebApiToolkit.Core.Globals;

namespace Wissance.WebApiToolkit.Core.Managers.Helpers
{
    public static class ResponseMessageBuilder
    {
        /// <summary>
        /// Method for getting Create Failure reason message using entity and reason
        /// </summary>
        /// <param name="resource">Entity type/table</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns>Formatted text with object creation error</returns>
        public static string GetCreateFailureMessage(string resource, string exceptionMessage)
        {
            return string.Format(MessageCatalog.CreateFailureMessageTemplate, resource, exceptionMessage);
        }
        
        /// <summary>
        ///  Method for getting Resource Not Found Message (Get Method)
        /// </summary>
        /// <param name="resource">Resource = entity/table</param>
        /// <param name="id">item identifier</param>
        /// <returns></returns>
        public static string GetResourceNotFoundMessage<TId>(string resource, TId id)
        {
            return string.Format(MessageCatalog.ResourceNotFoundTemplate, resource, id);
        }
        
        /// <summary>
        /// Method for getting Update Failure reason message using entity and reason
        /// </summary>
        /// <param name="resource">Entity type/table</param>
        /// <param name="id">Item identifier</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns></returns>
        public static string GetUpdateFailureMessage(string resource, string id, string exceptionMessage)
        {
            return string.Format(MessageCatalog.UpdateFailureMessageTemplate, resource, id, exceptionMessage);
        }

        /// <summary>
        /// Method for getting Bulk Update Failure reason message
        /// </summary>
        /// <param name="resource">Entity type/table</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns></returns>
        public static string GetBulkUpdateFailureMessage(string resource, string exceptionMessage)
        {
            return string.Format(MessageCatalog.BulkUpdateFailureMessageTemplate, resource, exceptionMessage);
        }

        /// <summary>
        /// Method for getting Resource Not Found Message (Update Method)
        /// </summary>
        /// <param name="resource">Entity type/table</param>
        /// <param name="id">Item identifier</param>
        /// <returns></returns>
        public static string GetUpdateNotFoundMessage(string resource, string id)
        {
            return string.Format(MessageCatalog.UpdateFailureNotFoundMessageTemplate, resource, id);
        }

        /// <summary>
        /// Method for getting User Has No Access to Resource Message
        /// </summary>
        /// <param name="resource">Resource = entity/table</param>
        /// <returns></returns>
        public static string GetCurrentUserResourceAccessErrorMessage(string resource)
        {
            return string.Format(MessageCatalog.CurrentUserIsNotResourceOwnerTemplate, resource);
        }
        
        /// <summary>
        /// Method for getting Unknown Error Message
        /// </summary>
        /// <param name="resource">Resource = entity/table</param>
        /// <param name="operation">Operation type = Create, Update, Read or Delete</param>
        /// <returns></returns>
        public static string GetUnknownErrorMessage(string resource, string operation)
        {
            return string.Format(MessageCatalog.UnknownErrorMessageTemplate, resource, operation);
        }
        
        /// <summary>
        /// Method for getting Generic error during some operation
        /// </summary>
        /// <param name="resource">Resource = entity/table or file or something else</param>
        /// <param name="operation">>Operation type = Create, Update, Read or Delete</param>
        /// <param name="error">exception message</param>
        /// <returns></returns>
        public static string GetOperationErrorMessage(string resource, string operation, string error)
        {
            return string.Format(MessageCatalog.OperationErrorMessageTemplate, resource, operation, error);
        }

        /// <summary>
        /// Method for getting bad source error message
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <returns></returns>
        public static string GetBadSourceErrorMessage(string source)
        {
            return string.Format(MessageCatalog.BadSourceProvidedTemplate, source);
        }

        /// <summary>
        /// Method for getting Conflict Error Message
        /// </summary>
        /// <param name="resource">resource type</param>
        /// <param name="property">name of a property that has conflicting value, i.e. index value</param>
        /// <param name="value">conflicting value</param>
        /// <returns></returns>
        public static string GetConflictErrorMessage(string resource, string property, string value)
        {
            return string.Format(MessageCatalog.ResourceConflictTemplate, resource, property, value);
        }

        /// <summary>
        /// Method for getting Bad Error Message
        /// </summary>
        /// <param name="resource">resource type</param>
        /// <param name="property">name of a property that has conflicting value, i.e. index value</param>
        /// <param name="value">conflicting value</param>
        /// <returns></returns>
        public static string GetBadRequestErrorMessage(string resource, string property, string value)
        {
            return string.Format(MessageCatalog.BadRequestErrorTemplate, resource, property, value);
        }

        /// <summary>
        /// Method for getting Not Implemented Error Message
        /// </summary>
        /// <param name="resource">resource type</param>
        /// <param name="operation">string representation of operation</param>
        /// <returns></returns>
        public static string GetNotImplementedErrorMessage(string resource, string operation)
        {
            return string.Format(MessageCatalog.OperationNotImplementedTemplate, operation, resource);
        }
        
        /// <summary>
        /// Method for getting Delete Failure reason message using entity and reason
        /// </summary>
        /// <param name="resource">>Entity type/table</param>
        /// <param name="id">Item identifier</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns></returns>
        public static string GetDeleteFailureMessage(string resource, string id, string exceptionMessage)
        {
            return string.Format(MessageCatalog.DeleteFailureMessageTemplate, resource, id, exceptionMessage);
        }
        
        /// <summary>
        /// Method for getting Bulk Delete Failure reason message
        /// </summary>
        /// <param name="resource">Entity type/table</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns></returns>
        public static string GetBulkDeleteFailureMessage(string resource, string exceptionMessage)
        {
            return string.Format(MessageCatalog.BulkDeleteFailureMessageTemplate, resource, exceptionMessage);
        }
    }
}