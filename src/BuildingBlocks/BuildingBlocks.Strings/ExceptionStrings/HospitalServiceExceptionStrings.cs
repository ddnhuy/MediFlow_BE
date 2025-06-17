using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Strings.ExceptionStrings
{
    public static class HospitalServiceExceptionStrings
    {
        public const string FAILED_CREATE_SERVICE = "Tạo dịch vụ thất bại";
        public const string SERVICE_NOT_FOUND = "Không tìm thấy dịch vụ với ID {0}";
        public const string FAILED_DELETE_SERVICE = "Xóa dịch vụ thất bại";
        public const string FAILED_UPDATE_SERVICE = "Cập nhật dịch vụ thất bại";
        public const string INVALID_SERVICE_ID = "Mã dịch vụ phải lớn hơn 0.";

        // Add constant cho ServiceGroup
        public const string FAILED_ADD_SERVICES_TO_GROUP = "Thêm dịch vụ vào nhóm thất bại";
        public const string EMPTY_SERVICE_IDS = "Danh sách dịch vụ không được để trống.";
        public const string INVALID_SERVICE_GROUP_ID = "Mã nhóm dịch vụ phải lớn hơn 0.";
        public const string FAILED_REMOVE_SERVICES_FROM_GROUP = "Xóa dịch vụ khỏi nhóm thất bại";
        public const string FAILED_UPDATE_SERVICE_GROUP = "Cập nhật nhóm dịch vụ thất bại";
        public const string EMPTY_GROUP_NAME = "Tên nhóm dịch vụ không được để trống.";
        public const string FAILED_DELETE_SERVICE_GROUP = "Xóa nhóm dịch vụ thất bại";
        public const string FAILED_CREATE_SERVICE_GROUP = "Tạo nhóm dịch vụ thất bại";

        // Add constant cho DiseaseGroup
        public const string INVALID_DISEASE_GROUP_ID = "Mã nhóm bệnh phải lớn hơn 0.";
        public const string FAILED_DELETE_DISEASE_GROUP = "Xóa nhóm bệnh thất bại";
        public const string EMPTY_DISEASE_GROUP_NAME = "Tên nhóm bệnh không được để trống.";
        public const string FAILED_UPDATE_DISEASE_GROUP = "Cập nhật nhóm bệnh thất bại";
        public const string FAILED_ADD_SERVICES_TO_DISEASE_GROUP = "Thêm dịch vụ vào nhóm bệnh thất bại";
        public const string FAILED_REMOVE_SERVICES_FROM_DISEASE_GROUP = "Xóa dịch vụ khỏi nhóm bệnh thất bại";
        public const string DISEASE_GROUP_NOT_FOUND = "Không tìm thấy nhóm bệnh với ID {0}";
        public const string FAILED_CREATE_DISEASE_GROUP = "Tạo nhóm bệnh thất bại";

    }
}
