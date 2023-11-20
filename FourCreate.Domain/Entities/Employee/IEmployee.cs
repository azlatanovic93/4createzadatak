using FourCreate.Domain.Entities.Employee.Enumerations;
using FourCreate.Domain.Entities.Employee.ValueObjects;

namespace FourCreate.Domain.Entities.Employee
{
    public interface IEmployee
    {
        Guid Id { get; }
        Title Title { get; }
        Email Email { get; }
    }
}
