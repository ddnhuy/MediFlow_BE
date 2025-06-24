namespace BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByIds
{
    public class GetServicesByIdsResponse
    {
        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}
