using FourCreate.Domain.Entities.Employee.Enumerations;
using FourCreate.Domain.Entities.Employee.ValueObjects;

namespace FourCreate.Domain.Entities.Employee
{
    public class NoEmployee : IEmployee
    {
        public Guid Id { get => Guid.Empty; set => Id = Guid.Empty; }
        public Title Title => Title.None;
        public Email Email => Email.None;
    }
}
