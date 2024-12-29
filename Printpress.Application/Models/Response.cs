using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public class Response<T> : Response
    {
        public Response()
        {
        }

        public Response(ResponseStatus status, string message, T data) : base(status, message)
        {
            Data = data;
        }
        public T Data { get; set; }

    }

    public class Response
    {
        public Response()
        {
        }
        public Response(ResponseStatus status, string message)
        {
            Status = status;
            Message = message;
        }
        public Response(ResponseStatus status, string message, string error) : this(status, message)
        {
            Errors = new List<string> { error };
        }
        public Response(ResponseStatus status, string message, List<string> errors) : this(status, message)
        {
            Errors = errors;
        }

        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess => Status == ResponseStatus.Success;
        public List<string> Errors { get; set; }

    }

    public static class ResponseMessage
    {
        public const string Success = "Request completed successfully.";
        public const string InternalServerError = "Internal Server Error.";
        public const string ValidationFailure = "One or more validation errors occurred.";
    }

}
