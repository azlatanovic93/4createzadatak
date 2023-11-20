namespace FourCreate.Application.Common
{
    /// <summary>
    /// Wrapper for Mediator handler results, to indicate success or failure and an optional statusCode for the http response.
    /// </summary>
    /// <typeparam name="T">Payload. Any generic class with a paramterless constructor</typeparam>
    public class ResponseResult<T>
    {
        //constructors for failure
        private ResponseResult(string reason) => FailureReason = reason;
        private ResponseResult(string reason, int statusCode)
        {
            FailureReason = reason;
            StatusCode = statusCode;
        }

        private ResponseResult(object reason) => FailureReasonObject = reason;
        private ResponseResult(object reason, int statusCode)
        {
            FailureReasonObject = reason;
            StatusCode = statusCode;
        }

        //constructors for Success
        private ResponseResult(T payload) => Payload = payload;
        private ResponseResult(T payload, int statusCode)
        {
            Payload = payload;
            StatusCode = statusCode;
        }

        public T Payload { get; }
        public string FailureReason { get; }
        public object FailureReasonObject { get; }
        public bool IsSuccess => FailureReason == null && FailureReasonObject == null;
        public int StatusCode { get; } = 200;

        public static ResponseResult<T> Success(T payload) => new ResponseResult<T>(payload);

        public static ResponseResult<T> Fail(string reason) => new ResponseResult<T>(reason);
        public static ResponseResult<T> Fail(string reason, int statusCode) => new ResponseResult<T>(reason, statusCode);
        public static ResponseResult<T> Fail(object reason, int statusCode) => new ResponseResult<T>(reason, statusCode);

        public static ResponseResult<T> ValidationError(string reason) => new ResponseResult<T>(reason, 400);
        public static ResponseResult<T> NotFound(string reason) => new ResponseResult<T>(reason, 404);

        public static ResponseResult<T> NoContent(string reason) => new ResponseResult<T>(reason, 204);
        public static implicit operator bool(ResponseResult<T> result) => result.IsSuccess;

        public static ResponseResult<T> FromResult<TOrigin>(ResponseResult<TOrigin> origin) => new ResponseResult<T>(origin.FailureReason, origin.StatusCode);
    }

}
