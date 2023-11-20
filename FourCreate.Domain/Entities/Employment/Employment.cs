using FourCreate.Domain.Entities.Common;

namespace FourCreate.Domain.Entities.Employment
{
    public class Employment : BaseEntity<Guid>, IEmployment
    {
        public Guid EmployeeId { get; set; }
        public long CompanyId { get; set; }
    }
}
