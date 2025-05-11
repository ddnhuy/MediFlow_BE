namespace HumanResource.Grpc.Helpers
{
    public interface ICurrentUserHelper
    {
        int UserId { get; }
        void SetUserId(int userId);
    }

    public class CurrentUserHelper : ICurrentUserHelper
    {
        private int _userId;

        public int UserId => _userId;

        public void SetUserId(int userId)
        {
            _userId = userId;
        }
    }
}
