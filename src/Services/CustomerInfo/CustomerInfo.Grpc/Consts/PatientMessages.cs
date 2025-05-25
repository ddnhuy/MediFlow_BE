namespace CustomerInfo.Grpc.Consts
{
    public static class PatientMessages
    {
        public static class Messages
        {
            public const string PatientNotFound = "Không tìm thấy bệnh nhân với ID {0}";
            public const string PatientCodeExists = "Mã bệnh nhân '{0}' đã tồn tại trong hệ thống. Vui lòng sử dụng mã khác.";
            public const string CreateError = "Không thể tạo bệnh nhân. Vui lòng thử lại sau.";
            public const string UpdateError = "Không thể cập nhật thông tin bệnh nhân. Vui lòng thử lại sau.";
            public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau.";
            public const string ValidationFailed = "Dữ liệu không hợp lệ.";
        }

        public static class PatientLogMessages
        {
            public const string ListingPatients = "Listing patients: page: {PageIndex}, size: {PageSize}";
            public const string FoundPatients = "Found {Count} patients matching criteria";
            public const string ReturningPatients = "Returning {Count} patients for page {PageIndex}";

            public const string GettingPatient = "Getting patient with ID: {PatientId}";
            public const string FoundPatient = "Found patient: {PatientName} (ID: {PatientId})";

            public const string CreatingPatient = "Creating new patient with code: {PatientCode}";
            public const string CreatedPatient = "Successfully created patient: {PatientName} (ID: {PatientId})";

            public const string UpdatingPatient = "Updating patient with ID: {PatientId}";
            public const string UpdatedPatient = "Successfully updated patient: {PatientName} (ID: {PatientId})";
            public const string PatientNotFoundForUpdate = "Patient not found with ID: {PatientId}";

            public const string DeletingPatient = "Deleting patient with ID: {PatientId}";
            public const string DeletedPatient = "Successfully deleted patient: {PatientName} (ID: {PatientId})";
            public const string PatientNotFoundForDelete = "Patient not found for deletion with ID: {PatientId}";

            public const string DbCreateError = "Database error while creating patient: {ErrorMessage}";
            public const string DbUpdateError = "Database error while updating patient {PatientId}: {ErrorMessage}";
            public const string UnexpectedCreateError = "Unexpected error while creating patient: {ErrorMessage}";
            public const string UnexpectedUpdateError = "Unexpected error while updating patient {PatientId}: {ErrorMessage}";
        }
    }
}