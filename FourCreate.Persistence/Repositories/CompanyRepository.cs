using CSharpFunctionalExtensions;
using FourCreate.Application.Contracts.Persistence;
using FourCreate.Domain.Entities.Employee.Factories;
using FourCreate.Persistence.DataModels;
using FourCreate.Persistence.Factories;
using Microsoft.EntityFrameworkCore;
using DomainCompany = FourCreate.Domain.Entities.Company;
using DomainEmployee = FourCreate.Domain.Entities.Employee;

namespace FourCreate.Persistence.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private NpgsqlDbContext _dbContext;
        private readonly IEmployeeFactory _employeeFactory;

        public CompanyRepository(NpgsqlDbContext dbContext,
            IEmployeeFactory employeeFactory)
        {
            _dbContext = dbContext;
            _employeeFactory = employeeFactory;
        }

        public async Task<long> AddAsync(DomainCompany.ICompany entity)
        {
            Company company = CompanyDataModelFactory.Create(entity);

            await _dbContext.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            return company.Id;
        }

        public async Task<IList<DomainCompany.ICompany>> GetByIdsAsync(IList<long> ids)
        {
            if (!ids.Any())
                return new List<DomainCompany.ICompany>();

            List<Employment> employments = await _dbContext.Employments
                .Include(e => e.Company)
                .Include(e => e.Employee)
                .Where(e => ids.Contains(e.Company.Id))
                .ToListAsync();

            if (!employments.Any())
                return new List<DomainCompany.ICompany>();

            List<DomainCompany.ICompany> companiesDomain = new List<DomainCompany.ICompany>();

            foreach (Company company in employments
                .Select(e => e.Company)
                .Distinct())
            {
                Result<DomainCompany.Company> companyDomainResult = DomainCompany.Company.Create(
                    company.Name,
                    company.Id);

                if (companyDomainResult.IsFailure)
                    return new List<DomainCompany.ICompany>();

                List<Employee> employees = employments
                    .Where(e => companyDomainResult.Value.Id == e.CompanyId)
                    .Select(e => e.Employee)
                    .ToList();

                foreach (Employee employee in employees)
                {
                    Result<DomainEmployee.IEmployee> employeeDomainResult = _employeeFactory.Create(
                        employee.Title,
                        employee.Email,
                        employee.Id);

                    if (employeeDomainResult.IsFailure)
                        return new List<DomainCompany.ICompany>();

                    Result addEmployeeResult = companyDomainResult.Value.Add(employeeDomainResult.Value);
                    if (addEmployeeResult.IsFailure)
                        return new List<DomainCompany.ICompany>();
                }

                companiesDomain.Add(companyDomainResult.Value);
            }

            return companiesDomain;
        }

        public async Task<DomainCompany.ICompany> GetByNameAsync(string name)
        {
            Company? company = await _dbContext.Companies
                .Where(c => c.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (company == null)
                return new DomainCompany.NoCompany();

            Result<DomainCompany.Company> companyResult = DomainCompany.Company.Create(
                company.Name, company.Id);

            if (companyResult.IsFailure)
                return new DomainCompany.NoCompany();


            return companyResult.Value;
        }
    }
}
