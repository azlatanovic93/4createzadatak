using FourCreate.Domain.Entities.Common;
using FourCreate.Domain.Entities.Employee.Enumerations;
using FourCreate.Domain.Entities.Employee.ValueObjects;

namespace FourCreate.Domain.Entities.Employee
{
    public class Employee : BaseEntity<Guid>, IEmployee
    {
        public Employee(
            Title title,
            Email email)
        {
            Email = email;
            Title = title;
        }

        public new Guid Id { get; set; }
        public Title Title { get; private set; }
        public Email Email { get; private set; }
    }
}
