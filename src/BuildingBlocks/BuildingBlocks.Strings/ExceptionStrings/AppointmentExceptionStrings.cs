namespace BuildingBlocks.Strings.ExceptionStrings
{
    public static class AppointmentExceptionStrings
    {
        public static string NOT_FOUND_APPOINTMENT_WITH_ID(int id) => $"Không tìm thấy cuộc hẹn với ID \"{id}\".";
        public static string NOT_FOUND_APPOINTMENT_WITH_PATIENT_ID(int patientId) => $"Không tìm thấy cuộc hẹn với ID bệnh nhân \"{patientId}\".";
        public static string NOT_FOUND_APPOINTMENT_WITH_DATE(DateTime date) => $"Không tìm thấy cuộc hẹn với ngày \"{date:yyyy-MM-dd}\".";
    }
}
