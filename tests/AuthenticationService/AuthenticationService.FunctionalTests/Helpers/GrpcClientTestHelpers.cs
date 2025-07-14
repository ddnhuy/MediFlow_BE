using Grpc.Core;

namespace AuthenticationService.FunctionalTests.Helpers
{
    public static class GrpcClientTestHelpers
    {
        public static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response) where T : class
        {
            return new AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );
        }
    }
}
