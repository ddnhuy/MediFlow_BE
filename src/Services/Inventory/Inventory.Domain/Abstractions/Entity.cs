namespace Inventory.Domain.Abstractions
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }

        private readonly List<IDomainEvent> _domainEvents = [];

        public List<IDomainEvent> DomainEvents => [.. _domainEvents];

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void Raise(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
