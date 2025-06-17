using CustomerInfo.Grpc.Models;

namespace CustomerInfo.Grpc.SeedDatas
{
    public static class PatientSeedData
    {
        private static readonly TimeZoneInfo VietNamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        private static readonly DateTime SeedDateTime = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), VietNamTimeZone);

        public static IEnumerable<Patient> GetSeedData()
        {
            return new List<Patient>
            {
                CreatePatient(1, "BN001", "Nguyễn Văn An", 1, new DateTime(1990, 5, 15), "0987654321", "testpatient.01@gmail.com", "123456789", "123 Đường Nguyễn Huệ", "TP. Hồ Chí Minh", "Quận 1", "Phường Bến Nghé", false, false),
                CreatePatient(2, "BN002", "Trần Thị Bình", 0, new DateTime(1985, 8, 20), "0987654322", "testpatient.02@gmail.com", "234567890", "456 Đường Lê Lợi", "TP. Hồ Chí Minh", "Quận 3", "Phường 6", true, false),
                CreatePatient(3, "BN003", "Lê Văn Cường", 1, new DateTime(1995, 3, 10), "0987654323", "testpatient.03@gmail.com", "345678901", "789 Đường Đồng Khởi", "TP. Thủ Đức", "Phường Linh Trung", "Phường Linh Trung", false, false),
                CreatePatient(4, "BN004", "Phạm Thị Dung", 0, new DateTime(1988, 12, 25), "0987654324", "testpatient.04@gmail.com", "456789012", "321 Đường Nguyễn Du", "TP. Hà Nội", "Quận Hoàn Kiếm", "Phường Hàng Bạc", false, false),
                CreatePatient(5, "BN005", "John Smith", 1, new DateTime(1980, 7, 5), "0987654325", "testpatient.05@gmail.com", "567890123", "654 Đường Lê Duẩn", "TP. Đà Nẵng", "Quận Hải Châu", "Phường Thạch Thang", false, true),
                CreatePatient(6, "BN006", "Hoàng Văn Minh", 1, new DateTime(1992, 4, 18), "0987654326", "testpatient.07@gmail.com", "678901234", "987 Đường Pasteur", "TP. Cần Thơ", "Quận Ninh Kiều", "Phường Xuân Khánh", false, false),
                CreatePatient(7, "BN007", "Nguyễn Thị Hương", 0, new DateTime(1993, 9, 30), "0987654327", "testpatient.08@gmail.com", "789012345", "147 Đường Võ Văn Tần", "TP. Hải Phòng", "Quận Ngô Quyền", "Phường Lạch Tray", true, false),
                CreatePatient(8, "BN008", "Trần Văn Phúc", 1, new DateTime(1987, 6, 12), "0987654328", "testpatient.09@gmail.com", "890123456", "258 Đường Nguyễn Đình Chiểu", "TP. Biên Hòa", "Phường Tân Hiệp", "Phường Tân Hiệp", false, false),
                CreatePatient(9, "BN009", "Sarah Johnson", 0, new DateTime(1991, 11, 8), "0987654329", "testpatient.10@gmail.com", "901234567", "369 Đường Lý Tự Trọng", "TP. Huế", "TP. Huế", "Phường Phú Hội", false, true),
                CreatePatient(10, "BN010", "Lê Thị Mai", 0, new DateTime(1994, 2, 28), "0987654330", "testpatient.11@gmail.com", "012345678", "741 Đường Đồng Khởi", "TP. Nha Trang", "TP. Nha Trang", "Phường Vĩnh Hòa", true, false),
            };
        }

        private static Patient CreatePatient(
            int id, string code, string name, int gender, DateTime dob, string phone, string email,
            string identityCard, string address, string province, string district, string ward,
            bool isPregnant, bool isForeigner)
        {
            var dobWithUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(dob, DateTimeKind.Unspecified), VietNamTimeZone);

            return new Patient
            {
                Id = id,
                Code = code,
                Name = name,
                Gender = gender,
                DOB = dobWithUtc,
                PhoneNumber = phone,
                Email = email,
                IdentityCard = identityCard,
                AddressDetail = address,
                Province = province,
                District = district,
                Ward = ward,
                IsPregnant = isPregnant,
                IsForeigner = isForeigner,
                CreatedAt = SeedDateTime
            };
        }
    }
}