namespace YarpApiGateWay
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public string? MessageKey { get; set; } = string.Empty;
        public T? Data { get; set; }

        public BaseResponse() { }

        public BaseResponse(int statusCode, string? messageKey, T? data = default)
        {
            StatusCode = statusCode;
            MessageKey = messageKey;
            Data = data;
        }

        public static BaseResponse<T> Success(T data, string? messageKey = null, int statusCode = 200)
            => new BaseResponse<T>(statusCode, messageKey, data);

        public static BaseResponse<T> Fail(string messageKey, int statusCode = 400)
            => new BaseResponse<T>(statusCode, messageKey, default);
    }
}
