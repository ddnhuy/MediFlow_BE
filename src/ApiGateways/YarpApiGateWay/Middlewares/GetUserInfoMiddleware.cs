namespace YarpApiGateWay.Middlewares
{
    public class GetUserInfoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GetUserInfoMiddleware> _logger;

        public GetUserInfoMiddleware(RequestDelegate next, ILogger<GetUserInfoMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Authorization = $"Bearer {token}";
            }

            await _next(context);
        }
    }
}
