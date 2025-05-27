namespace CustomerInfo.Grpc.Helpers
{
    public interface ICurrentUserHelper
    {
        int UserId { get; }
        void SetUserId(int userId);
    }

    public class CurrentUserHelper : ICurrentUserHelper
    {
        private int _userId;
        private readonly ILogger<CurrentUserHelper> _logger;

        public CurrentUserHelper(ILogger<CurrentUserHelper> logger)
        {
            _logger = logger;
        }

        public int UserId => _userId;

        public void SetUserId(int userId)
        {
            _userId = userId;
            _logger.LogDebug("User ID set to: {UserId}", userId);
        }
    }
}