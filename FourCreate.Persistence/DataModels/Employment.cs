using FourCreate.Persistence.DataModels.Common;

namespace FourCreate.Persistence.DataModels
{
    public class Employment : BaseDataModel
    {
        public Guid EmployeeId { get; set; }
        public long CompanyId { get; set; }

        // Be aware:
        // In entity framework the non-virtual properties represent the actual columns in your tables;
        // the virtual properties represent the relations between the tables
        public virtual Employee Employee { get; set; }
        public virtual Company Company { get; set; }
    }
}
