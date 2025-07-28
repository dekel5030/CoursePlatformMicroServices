namespace UserService.Common.Errors
{
    public enum ErrorCode
    {
        DuplicateEmail,
        DatabaseError,
        UserNotFound,
        InvalidPassword,
        Unexpected,
        EmailNotConfirmed
    }

    public static class ErrorCodeExtensions
    {
        public static bool IsPublic(this ErrorCode code)
        {
            return code switch
            {
                ErrorCode.DuplicateEmail => true,
                ErrorCode.UserNotFound => true,
                ErrorCode.InvalidPassword => true,
                ErrorCode.Unexpected => true,
                ErrorCode.EmailNotConfirmed => true,
                _ => false
            };
        }

        public static int GetHttpStatusCode(this ErrorCode code)
        {
            return code switch
            {
                ErrorCode.DuplicateEmail => StatusCodes.Status409Conflict,
                ErrorCode.UserNotFound => StatusCodes.Status404NotFound,
                ErrorCode.InvalidPassword => StatusCodes.Status400BadRequest,
                ErrorCode.DatabaseError => StatusCodes.Status500InternalServerError,
                ErrorCode.Unexpected => StatusCodes.Status500InternalServerError,
                ErrorCode.EmailNotConfirmed => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
        }

    }
    
}