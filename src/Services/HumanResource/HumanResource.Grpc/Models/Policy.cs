namespace HumanResource.Grpc.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string ResourceType { get; set; } = default!;
        public List<string> Actions { get; set; } = new List<string>();
    }
}
