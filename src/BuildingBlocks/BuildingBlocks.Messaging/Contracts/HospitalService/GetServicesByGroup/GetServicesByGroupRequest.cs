namespace BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup
{
    public class GetServicesByGroupRequest
    {
        public int GroupId { get; set; }
        public string? GroupType { get; set; }
    }
}
