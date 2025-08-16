using Grpc.Core;

namespace Inventory.Application.Helpers
{
    public static class GrpcMetaDataHelper
    {
        public static Metadata CreateAuthMetadata(int? userId = null, string? roles = null)
        {
            var metadata = new Metadata();
            metadata.Add("x-user-id", $"{userId}");

            if (!string.IsNullOrEmpty(roles))
            {
                metadata.Add("x-roles", roles);
            }
            return metadata;
        }
    }
}
