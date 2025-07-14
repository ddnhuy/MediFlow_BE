using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;

namespace BuildingBlocks.Pagination
{
    public static class PaginationHelper
    {
        public static void VerifyPaginationRequest(int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                throw new BadRequestException(ExceptionKey.PAGE_INDEX_TOO_SMALL);
            }
            else if (pageIndex >= 1000000)
            {
                throw new BadRequestException(ExceptionKey.PAGE_INDEX_TOO_LARGE);
            }
            if (pageSize <= 0)
            {
                throw new BadRequestException(ExceptionKey.PAGE_SIZE_TOO_SMALL);
            }
            else if (pageSize > 1000)
            {
                throw new BadRequestException(ExceptionKey.PAGE_SIZE_TOO_LARGE);
            }
        }
    }
}
