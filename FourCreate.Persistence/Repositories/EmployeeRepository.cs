using CSharpFunctionalExtensions;
using FourCreate.Application.Contracts.Persistence;
using FourCreate.Domain.Entities.Employee.Factories;
using FourCreate.Persistence.DataModels;
using FourCreate.Persistence.Factories;
using Microsoft.EntityFrameworkCore;

namespace FourCreate.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private NpgsqlDbContext _dbContext;
        private readonly IEmployeeFactory _employeeFactory;

        public EmployeeRepository(NpgsqlDbContext dbContext,
            IEmployeeFactory employeeFactory)
        {
            _dbContext = dbContext;
            _employeeFactory = employeeFactory;
        }

        public async Task<Guid> AddAsync(Domain.Entities.Employee.IEmployee entity)
        {
            Employee employee = EmployeeDataModelFactory.Create(entity);

            await _dbContext.AddAsync(employee);
            await _dbContext.SaveChangesAsync();

            return employee.Id;
        }

        public async Task<IList<Guid>> AddRangeAsync(IList<Domain.Entities.Employee.IEmployee> entities)
        {
            if (!entities.Any())
            {
                return new List<Guid>();
            }

            List<Employee> employees = new List<Employee>();
            foreach (Domain.Entities.Employee.IEmployee entity in entities)
            {
                employees.Add(EmployeeDataModelFactory.Create(entity));
            }

            await _dbContext.AddRangeAsync(employees);
            await _dbContext.SaveChangesAsync();

            return employees.Select(e => e.Id).ToList();
        }

        public async Task<IList<Domain.Entities.Employee.IEmployee>> GetByIdsAsync(IList<Guid> uuids)
        {
            List<Employee> employees = await _dbContext.Employees
                .Where(e => uuids.Contains(e.Id))
                .ToListAsync();

            if (!employees.Any())
                return new List<Domain.Entities.Employee.IEmployee>();

            List<Domain.Entities.Employee.IEmployee> employeesDomain = new List<Domain.Entities.Employee.IEmployee>();

            foreach (Employee employee in employees)
            {
                Result<Domain.Entities.Employee.IEmployee> employeeDomainResult = _employeeFactory.Create(
                    employee.Title,
                    employee.Email,
                    employee.Id);

                if (employeeDomainResult.IsFailure)
                    return new List<Domain.Entities.Employee.IEmployee>();

                employeesDomain.Add(employeeDomainResult.Value);
            }

            return employeesDomain;
        }

        public async Task<Domain.Entities.Employee.IEmployee> GetByEmailAsync(string email)
        {
            Employee? employee = await _dbContext.Employees
                .Where(e => e.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();

            if (employee is null)
                return new Domain.Entities.Employee.NoEmployee();

            Result<Domain.Entities.Employee.IEmployee> employeeResult = _employeeFactory.Create(
                employee.Title,
                employee.Email,
                employee.Id);

            if (employeeResult.IsFailure)
                return new Domain.Entities.Employee.NoEmployee();

            return employeeResult.Value;
        }

        public async Task<IList<Domain.Entities.Employee.IEmployee>> GetByEmailsAsync(IList<string> emails)
        {
            if (!emails.Any())
                return new List<Domain.Entities.Employee.IEmployee>();

            List<Employee> employees = await _dbContext.Employees
                .Where(e => emails.Contains(e.Email))
                .ToListAsync();

            if (!employees.Any())
                return new List<Domain.Entities.Employee.IEmployee>();

            List<Domain.Entities.Employee.IEmployee> employeesDomain = new List<Domain.Entities.Employee.IEmployee>();

            foreach (Employee employee in employees)
            {
                Result<Domain.Entities.Employee.IEmployee> employeeDomainResult = _employeeFactory.Create(
                    employee.Title,
                    employee.Email,
                    employee.Id);

                if (employeeDomainResult.IsFailure)
                    return new List<Domain.Entities.Employee.IEmployee>();

                employeesDomain.Add(employeeDomainResult.Value);
            }

            return employeesDomain;
        }
    }
}
