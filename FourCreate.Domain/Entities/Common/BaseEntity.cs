namespace FourCreate.Domain.Entities.Common
{
    /// <summary>
    /// Base types for all Entities which track state using a given Id.
    /// </summary>
    public abstract class BaseEntity<TID>
    {
        public TID Id { get; set; }
    }

}

