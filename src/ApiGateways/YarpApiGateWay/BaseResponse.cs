namespace YarpApiGateWay
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public BaseResponse() { }

        public BaseResponse(int statusCode, string message, T? data = default)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static BaseResponse<T> Success(T data, string message = "Success", int statusCode = 200)
            => new BaseResponse<T>(statusCode, message, data);

        public static BaseResponse<T> Fail(string message, int statusCode = 400)
            => new BaseResponse<T>(statusCode, message, default);
    }
}
