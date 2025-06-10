using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;

namespace BuildingBlocks.Messaging.Extensions
{
    public static class EmailSubjectCodeExtensions
    {
        public static string GetSubjectText(this EmailSubjectCode code)
        {
            return code switch
            {
                EmailSubjectCode.ResetPasswordSuccess => "Mật khẩu của bạn đã được đặt lại thành công",
                EmailSubjectCode.AppointmentConfirmed => "Cuộc hẹn tiêm chủng của bạn đã được xác nhận",
                EmailSubjectCode.AppointmentReminder => "Nhắc nhở cuộc hẹn tiêm chủng sắp tới",
                EmailSubjectCode.AppointmentCancelled => "Cuộc hẹn của bạn đã bị hủy",
                EmailSubjectCode.ProfileUpdated => "Hồ sơ của bạn đã được cập nhật",
                _ => "Thông báo từ MediFlow"
            };
        }
    }
}
