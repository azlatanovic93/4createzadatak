using CSharpFunctionalExtensions;
using FourCreate.Domain.Entities.Company.ValueObjects;
using FourCreate.Domain.Entities.Employee;

namespace FourCreate.Domain.Entities.Company
{
    public class NoCompany : ICompany
    {
        public long Id { get => 0; set => Id = 0; }
        public Name Name => Name.None;

        public IReadOnlyList<IEmployee> EmployeesReadOnly => new List<IEmployee>();

        public Result Add(IEmployee employee) => Result.Success();

        public void ClearEmployees() { }
    }
}
