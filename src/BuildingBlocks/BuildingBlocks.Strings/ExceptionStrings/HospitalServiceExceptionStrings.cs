using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Strings.ExceptionStrings
{
    public static class HospitalServiceExceptionStrings
    {
        public const string FAILED_ADD_SERVICES_TO_GROUP = "Thêm dịch vụ vào nhóm thất bại";
        public const string EMPTY_SERVICE_IDS = "Danh sách dịch vụ không được để trống.";
        public const string INVALID_SERVICE_GROUP_ID = "Mã nhóm dịch vụ phải lớn hơn 0.";
        public const string FAILED_REMOVE_SERVICES_FROM_GROUP = "Xóa dịch vụ khỏi nhóm thất bại";
        public const string FAILED_UPDATE_SERVICE_GROUP = "Cập nhật nhóm dịch vụ thất bại";
        public const string EMPTY_GROUP_NAME = "Tên nhóm dịch vụ không được để trống.";
        public const string FAILED_DELETE_SERVICE_GROUP = "Xóa nhóm dịch vụ thất bại";
    }
}
