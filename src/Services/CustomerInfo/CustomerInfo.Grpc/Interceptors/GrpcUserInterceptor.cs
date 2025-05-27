using Grpc.Core.Interceptors;
using Grpc.Core;
using CustomerInfo.Grpc.Helpers;

namespace CustomerInfo.Grpc.Interceptors
{
    public class GrpcUserInterceptor : Interceptor
    {
        private readonly ICurrentUserHelper _currentUserService;
        private readonly ILogger<GrpcUserInterceptor> _logger;

        public GrpcUserInterceptor(
            ICurrentUserHelper currentUserService,
            ILogger<GrpcUserInterceptor> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var userIdHeader = context.RequestHeaders.FirstOrDefault(h => h.Key == "user-id");
            if (userIdHeader != null)
            {
                if (int.TryParse(userIdHeader.Value, out int userId))
                {
                    _currentUserService.SetUserId(userId);
                    _logger.LogInformation("Set user ID: {UserId} for request: {RequestType}",
                        userId, typeof(TRequest).Name);
                }
                else
                {
                    _logger.LogWarning("Invalid user ID format in header: {Value}", userIdHeader.Value);
                }
            }
            else
            {
                _logger.LogWarning("No user-id header found in request: {RequestType}",
                    typeof(TRequest).Name);
            }

            return await continuation(request, context);
        }
    }
}