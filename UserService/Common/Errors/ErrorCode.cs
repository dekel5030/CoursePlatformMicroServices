namespace UserService.Common.Errors
{
    public enum ErrorCode
    {
        DuplicateEmail,
        DatabaseError,
        UserNotFound,
        InvalidPassword,
        Unexpected
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
                ErrorCode.DatabaseError => false,
                ErrorCode.Unexpected => false,
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
                _ => StatusCodes.Status500InternalServerError
            };
        }

    }
    
}