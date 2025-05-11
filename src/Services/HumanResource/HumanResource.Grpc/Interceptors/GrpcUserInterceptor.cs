using Grpc.Core;
using Grpc.Core.Interceptors;

namespace HumanResource.Grpc.Interceptors
{
    public class GrpcUserInterceptor : Interceptor
    {
        private readonly ICurrentUserHelper _currentUserService;

        public GrpcUserInterceptor(ICurrentUserHelper currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            int.TryParse(
                context.RequestHeaders.FirstOrDefault(h => h.Key == "user-id")?.Value,
                out int userId);

            _currentUserService.SetUserId(userId);

            return await continuation(request, context);
        }
    }
}
