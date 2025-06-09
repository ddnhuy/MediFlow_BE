using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;

namespace BuildingBlocks.Messaging.Extensions
{
    public static class EmailSubjectCodeExtensions
    {
        public static string GetSubjectText(this EmailSubjectCode code)
        {
            return code switch
            {
                EmailSubjectCode.ResetPasswordSuccess => "Your password has been reset successfully",
                EmailSubjectCode.AppointmentConfirmed => "Your vaccination appointment has been confirmed",
                EmailSubjectCode.AppointmentReminder => "Upcoming vaccination appointment reminder",
                EmailSubjectCode.AppointmentCancelled => "Your appointment has been cancelled",
                EmailSubjectCode.ProfileUpdated => "Your profile has been updated",
                _ => "Notification from MediFlow"
            };
        }
    }
}
