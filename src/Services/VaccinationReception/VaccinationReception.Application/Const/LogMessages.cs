using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Const
{
    public static class LogMessages
    {
        public static class PatientLogMessages
        {
            public const string ListPatients_SendingRequest = "[ListPatients] Sending request: PageIndex={PageIndex}, PageSize={PageSize}";
            public const string ListPatients_Received = "[ListPatients] Received {Count} patients";
            public const string ListPatients_Error = "[ListPatients] Error while retrieving patient list";

            public const string GetPatient_SendingRequest = "[GetPatient] Sending request for ID={Id}";
            public const string GetPatient_Success = "[GetPatient] Successfully retrieved patient with ID={Id}";
            public const string GetPatient_Error = "[GetPatient] Error while retrieving patient with ID={Id}";

            public const string CreatePatient_Success = "[CreatePatient] Successfully created patient with ID={Id}";
            public const string CreatePatient_Error = "[CreatePatient] Error while creating patient";

            public const string UpdatePatient_SendingRequest = "[UpdatePatient] Sending update request for ID={Id}";
            public const string UpdatePatient_Success = "[UpdatePatient] Successfully updated patient with ID={Id}";
            public const string UpdatePatient_Error = "[UpdatePatient] Error while updating patient with ID={Id}";

            public const string DeletePatient_SendingRequest = "[DeletePatient] Sending delete request for ID={Id}";
            public const string DeletePatient_Success = "[DeletePatient] Successfully deleted patient with ID={Id}";
            public const string DeletePatient_Error = "[DeletePatient] Error while deleting patient with ID={Id}";

            public const string GetPatient_Handler_Request = "[GetPatientHandler] Getting patient with ID: {PatientId}";
            public const string GetPatient_Handler_Success = "[GetPatientHandler] Found patient with ID: {PatientId}";
            public const string GetPatient_Handler_Error = "[GetPatientHandler] Error getting patient with ID: {PatientId}";
        }
    }
}