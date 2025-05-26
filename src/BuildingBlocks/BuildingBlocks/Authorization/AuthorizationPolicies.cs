using BuildingBlocks.Strings;
using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Authorization
{
    public static class AuthorizationPolicies
    {
        // Existing policy
        public const string VACCINE_RECEPTION = "Vaccine Reception";

        // Admin policies
        public const string ADMIN_ACCESS = "Admin Access";
        public const string SYSTEM_MANAGEMENT = "System Management";

        // Medical staff policies
        public const string MEDICAL_STAFF = "Medical Staff";
        public const string PATIENT_RECORDS = "Patient Records";
        public const string PRESCRIBE_MEDICINE = "Prescribe Medicine";

        // Department management
        public const string DEPARTMENT_MANAGEMENT = "Department Management";

        // Lab related policies
        public const string LAB_RESULTS = "Lab Results";
        public const string LAB_MANAGEMENT = "Lab Management";

        // Pharmacy related policies
        public const string PHARMACY_ACCESS = "Pharmacy Access";
        public const string MEDICINE_MANAGEMENT = "Medicine Management";

        // Financial policies
        public const string FINANCIAL_ACCESS = "Financial Access";
        public const string BILLING_MANAGEMENT = "Billing Management";

        // Warehouse policies
        public const string WAREHOUSE_ACCESS = "Warehouse Access";
        public const string INVENTORY_MANAGEMENT = "Inventory Management";

        // Imaging policies
        public const string IMAGING_ACCESS = "Imaging Access";

        // Reception policies
        public const string RECEPTION_DESK = "Reception Desk";

        public static void RegisterPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(VACCINE_RECEPTION, policy =>
            {
                policy.RequireRole(new[] { Roles.DOCTOR, Roles.NURSE, Roles.RECEPTIONIST });
                policy.RequireClaim("Department", DepartmentTypes.VACCINE_RECEPTION);
            });

            options.AddPolicy(ADMIN_ACCESS, policy =>
            {
                policy.RequireRole(Roles.ADMIN);
                policy.RequireClaim("Department", DepartmentTypes.ADMIN);
            });

            options.AddPolicy(SYSTEM_MANAGEMENT, policy =>
            {
                policy.RequireRole(new[] { Roles.ADMIN, Roles.IT_SUPPORT });
                policy.RequireClaim("Department", DepartmentTypes.ADMIN);
            });

            options.AddPolicy(MEDICAL_STAFF, policy =>
            {
                policy.RequireRole(new[] { Roles.DOCTOR, Roles.NURSE });
                policy.RequireClaim("Department", new[]
                {
                    DepartmentTypes.CLINIC,
                    DepartmentTypes.INPATIENT,
                    DepartmentTypes.EMERGENCY
                });
            });

            options.AddPolicy(PATIENT_RECORDS, policy =>
            {
                policy.RequireRole(new[] { Roles.DOCTOR, Roles.NURSE, Roles.HEAD_OF_DEPARTMENT, Roles.LABORATORY_STAFF });
                policy.RequireClaim("Department", new[]
                {
                    DepartmentTypes.CLINIC,
                    DepartmentTypes.LAB,
                    DepartmentTypes.INPATIENT
                });
            });

            options.AddPolicy(PRESCRIBE_MEDICINE, policy =>
            {
                policy.RequireRole(Roles.DOCTOR);
                policy.RequireClaim("Department", DepartmentTypes.CLINIC);
            });

            options.AddPolicy(DEPARTMENT_MANAGEMENT, policy =>
            {
                policy.RequireRole(new[] { Roles.HEAD_OF_DEPARTMENT, Roles.ADMIN });
            });

            options.AddPolicy(LAB_RESULTS, policy =>
            {
                policy.RequireRole(new[] { Roles.DOCTOR, Roles.LABORATORY_STAFF });
                policy.RequireClaim("Department", DepartmentTypes.LAB);
            });

            options.AddPolicy(LAB_MANAGEMENT, policy =>
            {
                policy.RequireRole(Roles.LABORATORY_STAFF);
                policy.RequireClaim("Department", DepartmentTypes.LAB);
            });

            options.AddPolicy(PHARMACY_ACCESS, policy =>
            {
                policy.RequireRole(new[] { Roles.DOCTOR, Roles.PHARMACY_STAFF });
                policy.RequireClaim("Department", new[]
                {
                    DepartmentTypes.CLINIC,
                    DepartmentTypes.PHARMACY
                });
            });

            options.AddPolicy(MEDICINE_MANAGEMENT, policy =>
            {
                policy.RequireRole(Roles.PHARMACY_STAFF);
                policy.RequireClaim("Department", DepartmentTypes.PHARMACY);
            });

            options.AddPolicy(FINANCIAL_ACCESS, policy =>
            {
                policy.RequireRole(new[] { Roles.ACCOUNTANT, Roles.ADMIN });
                policy.RequireClaim("Department", new[]
                {
                    DepartmentTypes.FINANCE,
                    DepartmentTypes.BILLING
                });
            });

            options.AddPolicy(BILLING_MANAGEMENT, policy =>
            {
                policy.RequireRole(Roles.ACCOUNTANT);
                policy.RequireClaim("Department", DepartmentTypes.BILLING);
            });

            options.AddPolicy(WAREHOUSE_ACCESS, policy =>
            {
                policy.RequireRole(new[] { Roles.WAREHOUSE_STAFF, Roles.PHARMACY_STAFF });
                policy.RequireClaim("Department", new[]
                {
                    DepartmentTypes.STORAGE,
                    DepartmentTypes.PHARMACY
                });
            });

            options.AddPolicy(INVENTORY_MANAGEMENT, policy =>
            {
                policy.RequireRole(Roles.WAREHOUSE_STAFF);
                policy.RequireClaim("Department", DepartmentTypes.STORAGE);
            });

            options.AddPolicy(IMAGING_ACCESS, policy =>
            {
                policy.RequireRole(new[] { Roles.IMAGING_TECHNICIAN, Roles.DOCTOR });
                policy.RequireClaim("Department", DepartmentTypes.IMAGING);
            });

            options.AddPolicy(RECEPTION_DESK, policy =>
            {
                policy.RequireRole(Roles.RECEPTIONIST);
                policy.RequireClaim("Department", DepartmentTypes.VACCINE_RECEPTION);
            });
        }
    }
}
