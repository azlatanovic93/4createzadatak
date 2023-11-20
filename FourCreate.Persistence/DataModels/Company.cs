using FourCreate.Persistence.DataModels.Common;

namespace FourCreate.Persistence.DataModels
{
    public class Company : BaseDataModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Employment> Employments { get; set; }
    }
}
