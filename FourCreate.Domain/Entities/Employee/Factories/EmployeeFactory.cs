using CSharpFunctionalExtensions;
using FourCreate.Domain.Entities.Employee.Enumerations;
using FourCreate.Domain.Entities.Employee.ValueObjects;

namespace FourCreate.Domain.Entities.Employee.Factories
{
    public class EmployeeFactory : IEmployeeFactory
    {
        public Result<IEmployee> Create(string title, string email, Guid uuid)
        {
            Result<Email> emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return Result.Failure<IEmployee>(emailResult.Error);

            Title titleEnum = Title.FromNullableName(title.ToUpper());
            if (titleEnum is null)
            {
                return Result.Failure<IEmployee>($"Title [{title}] doesn't exist");
            }

            Employee employee = new Employee(
                titleEnum,
                emailResult.Value);

            employee.Id = uuid;

            return Result.Success<IEmployee>(employee);

        }
    }
}
