namespace BuildingBlocks.Messaging.Enums
{
    namespace BuildingBlocks.Messaging.Enums
    {
        public enum EmailSubjectCode
        {
            /// <summary>
            /// Reset password successful
            /// </summary>
            /// Template: ResetPasswordSuccess.cshtml
            /// Required TemplateData:
            /// - FullName
            /// - ResetTime
            /// - NewPassword
            ResetPasswordSuccess,

            /// <summary>
            /// Successfully scheduled injection
            /// </summary>
            /// Template: AppointmentConfirmed.cshtml
            /// Required TemplateData:
            /// - FullName
            /// - AppointmentDate
            /// - VaccineName
            /// - LocationName
            /// - AppointmentCode
            AppointmentConfirmed,

            /// <summary>
            /// Reminder of upcoming vaccination schedule
            /// </summary>
            /// Template: AppointmentReminder.cshtml
            /// Required TemplateData:
            /// - FullName
            /// - AppointmentDate
            /// - VaccineName
            /// - LocationName
            AppointmentReminder,

            /// <summary>
            /// Confirm appointment cancellation
            /// </summary>
            /// Template: AppointmentCancelled.cshtml
            /// Required TemplateData:
            /// - FullName
            /// - AppointmentDate
            /// - VaccineName
            /// - LocationName
            /// - CancelReason
            AppointmentCancelled,

            /// <summary>
            /// Update user profile
            /// </summary>
            /// Template: ProfileUpdated.cshtml
            /// Required TemplateData:
            /// - FullName
            /// - UpdateTime
            /// - UpdatedFields (e.g., list string join with ", ")
            ProfileUpdated
        }
    }
}