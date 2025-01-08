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

        public Response<T> SuccessResponse(T data)
        {
            return new Response<T>(ResponseStatus.Success, ResponseMessage.Success, data);
        }
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
}
