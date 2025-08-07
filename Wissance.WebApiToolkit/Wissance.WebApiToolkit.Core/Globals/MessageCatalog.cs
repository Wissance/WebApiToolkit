namespace Wissance.WebApiToolkit.Core.Globals
{
    public static class MessageCatalog
    {
        public const string ResourceNotFoundTemplate = "Recource of type \"{0}\" with id: {1} was not found";
        public const string CurrentUserIsNotResourceOwnerTemplate = "Current user is not \"{0}\" owner";
        public const string CreateFailureMessageTemplate = "An error occurred during \"{0}\" create with error: {1}";
        public const string UpdateFailureMessageTemplate = "An error occurred during \"{0}\" update with id: \"{1}\", error: {2}";
        public const string BulkUpdateFailureMessageTemplate = "An error occurred during \"{0}\" bulk update, error: {1}";
        public const string UpdateFailureNotFoundMessageTemplate = "{0} with id: {1} was not found";
        public const string DeleteFailureMessageTemplate = "An error occurred during \"{0}\" delete with id: \"{1}\", error: {2}";
        public const string BulkDeleteFailureMessageTemplate = "An error occurred during \"{0}\" bulk delete, error: {1}";
        public const string OperationErrorMessageTemplate = "An error occurred during \"{0}\" \"{1}\" operation, error: {2}";

        public const string UnknownErrorMessageTemplate = "An error occurred during \"{0}\" \"{1}\"";
        public const string UserNotAuthenticatedMessage = "User is not authenticated";

        public const string BadSourceProvidedTemplate = "Source: \"{0}\" does not exist or was not provided";
        public const string FileResourceType = "File";
        public const string DirectoryResourceType = "Directory";
        public const string ResourceConflictTemplate = "Resource of type\"{0}\" with \"{1}\" property with value - \"{2}\" already exists";
        public const string BadRequestErrorTemplate = "Resource of type\"{0}\" with \"{1}\" property with value - \"{2}\" is not supported";
        public const string OperationNotImplementedTemplate = "Operation \"{0}\" was not implemented for resource of type\"{1}\"";
    }
}