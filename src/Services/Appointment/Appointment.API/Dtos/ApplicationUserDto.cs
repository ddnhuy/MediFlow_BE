namespace Appointment.API.Dtos
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
