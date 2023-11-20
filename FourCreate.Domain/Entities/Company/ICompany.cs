using CSharpFunctionalExtensions;
using FourCreate.Domain.Entities.Company.ValueObjects;
using FourCreate.Domain.Entities.Employee;

namespace FourCreate.Domain.Entities.Company
{
    public interface ICompany
    {
        IReadOnlyList<IEmployee> EmployeesReadOnly { get; }
        long Id { get; }
        Name Name { get; }
        Result Add(IEmployee employee);
        void ClearEmployees();
    }
}
