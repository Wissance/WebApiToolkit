namespace Wissance.WebApiToolkit.Globals
{
    public static class MessageCatalog
    {
        public const string ResourceNotFoundTemplate = "Recource of type \"{0}\" with id: {1} was not found";
        public const string CurrentUserIsNotResourceOwnerTemplate = "Current user is not \"{0}\" owner";
        public const string CreateFailureMessageTemplate = "An error occurred during \"{0}\" create with error: {1}";
        public const string UpdateFailureMessageTemplate = "An error occurred during \"{0}\" update with id: \"{1}\", error: {2}";
        public const string UpdateFailureNotFoundMessageTemplate = "{0} with id: {1} was not found";

        public const string UnknownErrorMessageTemplate = "An error occurred during {0} \"{1}\", contact system maintainer";
        public const string UserNotAuthenticatedMessage = "User is not authenticated";
    }
}