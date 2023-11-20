using CSharpFunctionalExtensions;
using FourCreate.Domain.Entities.Common;
using FourCreate.Domain.Entities.Company.ValueObjects;
using FourCreate.Domain.Entities.Employee;
using FourCreate.Domain.Entities.Employee.Enumerations;

namespace FourCreate.Domain.Entities.Company
{
    public class Company : BaseEntity<long>, ICompany
    {
        private readonly List<IEmployee> _employees;
        public IReadOnlyList<IEmployee> EmployeesReadOnly => new List<IEmployee>(_employees);

        private Company(Name name)
        {
            Name = name;
            _employees = new List<IEmployee>();
        }

        public Name Name { get; private set; }

        public static Result<Company> Create(string name, long id)
        {
            Result<Name> nameResult = Name.Create(name);
            if (nameResult.IsFailure)
                return Result.Failure<Company>(nameResult.Error);

            Company company = new Company(
                nameResult.Value);

            company.Id = id;

            return Result.Success(company);
        }

        public Result Add(IEmployee employee)
        {
            if (_employees.Any())
            {
                int total = _employees.Where(e => (e.Title == Title.Manager && e.Title == employee.Title)
                || (e.Title == Title.Developer && e.Title == employee.Title))
                    .Select(e => e.Title)
                    .ToList()
                    .Count();

                if (total > 0)
                    return Result.Failure($"Company can't have more than 1 {employee.Title.DisplayName}");

                if (_employees.Where(e => e.Email == employee.Email).Any())
                    return Result.Failure($"Employee with mail {employee.Email.Value} is already added to company {Name}");

                if (_employees.Where(e => (employee.Id != Guid.Empty && employee.Id == e.Id)).Any())
                    return Result.Failure($"Employee with id {employee.Id} is already added to company {Name}");
            }

            _employees.Add(employee);
            return Result.Success();
        }

        public void ClearEmployees()
        {
            _employees.Clear();
        }


    }
}
