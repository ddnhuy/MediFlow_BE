namespace CustomerInfo.Grpc.Consts
{
    public static class PatientMessages
    {
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