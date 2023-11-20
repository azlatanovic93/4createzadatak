using FourCreate.Persistence.DataModels.Common;

namespace FourCreate.Persistence.DataModels
{
    public class Employee : BaseDataModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Employment> Employments { get; set; }
    }
}
