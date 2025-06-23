using Wissance.WebApiToolkit.Core.Globals;

namespace Wissance.WebApiToolkit.Core.Managers.Helpers
{
    public static class ResponseMessageBuilder
    {
        /// <summary>
        /// Method for getting Create Failure reason message using entity and reason
        /// </summary>
        /// <param name="entity">Entity type/table</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns>Formatted text with object creation error</returns>
        public static string GetCreateFailureMessage(string entity, string exceptionMessage)
        {
            return string.Format(MessageCatalog.CreateFailureMessageTemplate, entity, exceptionMessage);
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
        /// <param name="entity">Entity type/table</param>
        /// <param name="id">Item identifier</param>
        /// <param name="exceptionMessage">Exception method</param>
        /// <returns></returns>
        public static string GetUpdateFailureMessage(string entity, int id, string exceptionMessage)
        {
            return string.Format(MessageCatalog.UpdateFailureMessageTemplate, entity, id, exceptionMessage);
        }
        
        /// <summary>
        /// Method for getting Resource Not Found Message (Update Method)
        /// </summary>
        /// <param name="entity">Entity type/table</param>
        /// <param name="id">Item identifier</param>
        /// <returns></returns>
        public static string GetUpdateNotFoundMessage(string entity, int id)
        {
            return string.Format(MessageCatalog.UpdateFailureNotFoundMessageTemplate, entity, id);
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
        /// <param name="operation">Operation type = Create, Update, Read or Delete</param>
        /// <param name="resource">Resource = entity/table</param>
        /// <returns></returns>
        public static string GetUnknownErrorMessage(string operation, string resource)
        {
            return string.Format(MessageCatalog.UnknownErrorMessageTemplate, operation, resource);
        }
    }
}