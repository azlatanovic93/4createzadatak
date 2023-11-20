using CSharpFunctionalExtensions;

namespace FourCreate.Domain.Entities.Employee.Factories
{
    public interface IEmployeeFactory
    {
        Result<IEmployee> Create(
            string title,
            string email,
            Guid uuid);
    }

}
