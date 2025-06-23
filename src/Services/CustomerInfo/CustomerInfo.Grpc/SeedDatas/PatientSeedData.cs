using CustomerInfo.Grpc.Models;

namespace CustomerInfo.Grpc.SeedDatas
{
    public static class PatientSeedData
    {
        private static readonly DateTime SeedDateTime = GetSeedDateTime();

        private static DateTime GetSeedDateTime()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var localTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
                var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
                return DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }
            catch (TimeZoneNotFoundException)
            {
                var offset = TimeSpan.FromHours(7);
                var utcTime = new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0), offset).UtcDateTime;
                return utcTime; // DateTimeOffset.UtcDateTime already returns UTC kind
            }
        }

        private static DateTime ToUtc(DateTime dateTime)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var local = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                var utc = TimeZoneInfo.ConvertTimeToUtc(local, timeZone);
                return DateTime.SpecifyKind(utc, DateTimeKind.Utc); // Explicitly set to UTC kind
            }
            catch (TimeZoneNotFoundException)
            {
                var offset = TimeSpan.FromHours(7);
                var utc = new DateTimeOffset(dateTime, offset).UtcDateTime;
                return utc; // DateTimeOffset.UtcDateTime already returns UTC kind
            }
        }

        public static IEnumerable<Patient> GetSeedData()
        {
            return new List<Patient>
            {
                CreatePatient(1, "BN001", "Nguyễn Văn An", 1, ToUtc(new DateTime(1990, 5, 15)), "0987654321", "123456789", "123 Đường Nguyễn Huệ", "TP. Hồ Chí Minh", "Quận 1", "Phường Bến Nghé", false, false),
                CreatePatient(2, "BN002", "Trần Thị Bình", 0, ToUtc(new DateTime(1985, 8, 20)), "0987654322", "234567890", "456 Đường Lê Lợi", "TP. Hồ Chí Minh", "Quận 3", "Phường 6", true, false),
                CreatePatient(3, "BN003", "Lê Văn Cường", 1, ToUtc(new DateTime(1995, 3, 10)), "0987654323", "345678901", "789 Đường Đồng Khởi", "TP. Thủ Đức", "TP. Thủ Đức", "Phường Linh Trung", false, false),
                CreatePatient(4, "BN004", "Phạm Thị Dung", 0, ToUtc(new DateTime(1988, 12, 25)), "0987654324", "456789012", "321 Đường Nguyễn Du", "TP. Hà Nội", "Quận Hoàn Kiếm", "Phường Hàng Bạc", false, false),
                CreatePatient(5, "BN005", "John Smith", 1, ToUtc(new DateTime(1980, 7, 5)), "0987654325", "567890123", "654 Đường Lê Duẩn", "TP. Đà Nẵng", "Quận Hải Châu", "Phường Thạch Thang", false, true),
                CreatePatient(6, "BN006", "Hoàng Văn Minh", 1, ToUtc(new DateTime(1992, 4, 18)), "0987654326", "678901234", "987 Đường Pasteur", "TP. Cần Thơ", "Quận Ninh Kiều", "Phường Xuân Khánh", false, false),
                CreatePatient(7, "BN007", "Nguyễn Thị Hương", 0, ToUtc(new DateTime(1993, 9, 30)), "0987654327", "789012345", "147 Đường Võ Văn Tần", "TP. Hải Phòng", "Quận Ngô Quyền", "Phường Lạch Tray", true, false),
                CreatePatient(8, "BN008", "Trần Văn Phúc", 1, ToUtc(new DateTime(1987, 6, 12)), "0987654328", "890123456", "369 Đường Trần Phú", "TP. Huế", "Quận Phú Nhuận", "Phường Vĩnh Ninh", false, false)
            };
        }

        private static Patient CreatePatient(
            int id,
            string code,
            string name,
            short gender,
            DateTime dob,
            string phoneNumber,
            string identityCard,
            string addressDetail,
            string province,
            string district,
            string ward,
            bool isPregnant,
            bool isForeigner,
            int createdBy = 1,
            int lastUpdatedBy = 1
        )
        {
            return new Patient
            {
                Id = id,
                Code = code,
                Name = name,
                Gender = gender,
                DOB = dob,
                PhoneNumber = phoneNumber,
                IdentityCard = identityCard,
                AddressDetail = addressDetail,
                Province = province,
                District = district,
                Ward = ward,
                IsPregnant = isPregnant,
                IsForeigner = isForeigner,
                CreatedAt = SeedDateTime,
                LastUpdatedAt = SeedDateTime,
                CreatedBy = createdBy,
                LastUpdatedBy = lastUpdatedBy,
                IsSuspended = false,
                IsCancelled = false
            };
        }
    }
}