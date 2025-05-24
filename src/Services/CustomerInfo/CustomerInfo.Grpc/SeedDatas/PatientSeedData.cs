using CustomerInfo.Grpc.Models;

namespace CustomerInfo.Grpc.SeedDatas
{
    public static class PatientSeedData
    {
        private static readonly DateTime SeedDateTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static IEnumerable<Patient> GetSeedData()
        {
            return new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyễn Văn An",
                    Gender = 1,
                    DOB = new DateTime(1990, 5, 15),
                    PhoneNumber = "0987654321",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Đường Nguyễn Huệ",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Bến Nghé",
                    IsPregnant = false,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 2,
                    Code = "BN002",
                    Name = "Trần Thị Bình",
                    Gender = 0,
                    DOB = new DateTime(1985, 8, 20),
                    PhoneNumber = "0987654322",
                    IdentityCard = "234567890",
                    AddressDetail = "456 Đường Lê Lợi",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Bến Thành",
                    IsPregnant = true,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 3,
                    Code = "BN003",
                    Name = "Lê Văn Cường",
                    Gender = 1,
                    DOB = new DateTime(1995, 3, 10),
                    PhoneNumber = "0987654323",
                    IdentityCard = "345678901",
                    AddressDetail = "789 Đường Đồng Khởi",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Nguyễn Thái Bình",
                    IsPregnant = false,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 4,
                    Code = "BN004",
                    Name = "Phạm Thị Dung",
                    Gender = 0,
                    DOB = new DateTime(1988, 12, 25),
                    PhoneNumber = "0987654324",
                    IdentityCard = "456789012",
                    AddressDetail = "321 Đường Nguyễn Du",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Bến Nghé",
                    IsPregnant = false,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 5,
                    Code = "BN005",
                    Name = "John Smith",
                    Gender = 1,
                    DOB = new DateTime(1980, 7, 5),
                    PhoneNumber = "0987654325",
                    IdentityCard = "567890123",
                    AddressDetail = "654 Đường Lê Duẩn",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Bến Thành",
                    IsPregnant = false,
                    IsForeigner = true,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 6,
                    Code = "BN006",
                    Name = "Hoàng Văn Minh",
                    Gender = 1,
                    DOB = new DateTime(1992, 4, 18),
                    PhoneNumber = "0987654326",
                    IdentityCard = "678901234",
                    AddressDetail = "987 Đường Pasteur",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 3",
                    Ward = "Phường Võ Thị Sáu",
                    IsPregnant = false,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 7,
                    Code = "BN007",
                    Name = "Nguyễn Thị Hương",
                    Gender = 0,
                    DOB = new DateTime(1993, 9, 30),
                    PhoneNumber = "0987654327",
                    IdentityCard = "789012345",
                    AddressDetail = "147 Đường Võ Văn Tần",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 3",
                    Ward = "Phường 6",
                    IsPregnant = true,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 8,
                    Code = "BN008",
                    Name = "Trần Văn Phúc",
                    Gender = 1,
                    DOB = new DateTime(1987, 6, 12),
                    PhoneNumber = "0987654328",
                    IdentityCard = "890123456",
                    AddressDetail = "258 Đường Nguyễn Đình Chiểu",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 3",
                    Ward = "Phường 5",
                    IsPregnant = false,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 9,
                    Code = "BN009",
                    Name = "Sarah Johnson",
                    Gender = 0,
                    DOB = new DateTime(1991, 11, 8),
                    PhoneNumber = "0987654329",
                    IdentityCard = "901234567",
                    AddressDetail = "369 Đường Lý Tự Trọng",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Bến Thành",
                    IsPregnant = false,
                    IsForeigner = true,
                    CreatedAt = SeedDateTime
                },
                new Patient
                {
                    Id = 10,
                    Code = "BN010",
                    Name = "Lê Thị Mai",
                    Gender = 0,
                    DOB = new DateTime(1994, 2, 28),
                    PhoneNumber = "0987654330",
                    IdentityCard = "012345678",
                    AddressDetail = "741 Đường Đồng Khởi",
                    Province = "TP. Hồ Chí Minh",
                    District = "Quận 1",
                    Ward = "Phường Nguyễn Thái Bình",
                    IsPregnant = true,
                    IsForeigner = false,
                    CreatedAt = SeedDateTime
                }
            };
        }
    }
}