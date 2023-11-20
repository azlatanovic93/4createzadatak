using CSharpFunctionalExtensions;
using FourCreate.Application.Common;
using FourCreate.Application.Contracts.Persistence;
using FourCreate.Domain.Entities.Company;
using FourCreate.Domain.Entities.Employee;
using FourCreate.Domain.Entities.Employee.Factories;
using FourCreate.Domain.Entities.Employment;
using MediatR;

namespace FourCreate.Application.UseCases.Company.Commands.Create
{
    public class CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, ResponseResult<CreateCompanyResponse>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyHandler(
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository,
            IEmploymentRepository employmentRepository,
            IEmployeeFactory employeeFactory,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _employmentRepository = employmentRepository;
            _employeeFactory = employeeFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseResult<CreateCompanyResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            Result<Domain.Entities.Company.Company> companyResult = Domain.Entities.Company.Company.Create(request.Name, 0);

            if (companyResult.IsFailure)
            {
                return ResponseResult<CreateCompanyResponse>.ValidationError(companyResult.Error);
            }

            // Check if company exists
            ICompany existingCompany = await _companyRepository.GetByNameAsync(request.Name);
            if (!(existingCompany is NoCompany))
            {
                return ResponseResult<CreateCompanyResponse>.ValidationError($"Company with {request.Name} already exist");
            }

            await _unitOfWork.CreateTransactionAsync();

            long companyId = await _companyRepository.AddAsync(companyResult.Value);

            // if employees need to be added
            if (request.Employees.Any())
            {
                List<Guid> employeesIds = new List<Guid>();

                foreach (var employee in request.Employees)
                {
                    if (employee.Id != null)
                    {
                        employeesIds.Add((Guid)employee.Id);
                        continue;
                    }

                    Result<IEmployee> employeeResult = _employeeFactory.Create(
                        employee.Title, employee.Email, Guid.Empty);

                    if (employeeResult.IsFailure)
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateCompanyResponse>.ValidationError(employeeResult.Error);
                    }

                    // Try to add employees to the company (domain only) which will be created
                    Result addEmployeeResult = companyResult.Value.Add(employeeResult.Value);
                    if (addEmployeeResult.IsFailure)
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateCompanyResponse>.ValidationError(addEmployeeResult.Error);
                    }
                }

                // Check if employees with emails already exists and try to add them
                if (companyResult.Value.EmployeesReadOnly.Any())
                {
                    IList<IEmployee> listEmployeesByEmail = await _employeeRepository.GetByEmailsAsync(
                        companyResult.Value.EmployeesReadOnly
                            .Select(e => e.Email.Value)
                            .ToList());

                    if (listEmployeesByEmail.Any())
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateCompanyResponse>.ValidationError("Some of provided Employees emails already exist");
                    }

                    IList<Guid> employeesUuids = await _employeeRepository.AddRangeAsync(
                        companyResult.Value.EmployeesReadOnly.ToList());

                    employeesUuids.ToList().ForEach(elem =>
                    {
                        employeesIds.Add(elem);
                    });
                }

                IList<IEmployee> listEmployees = await _employeeRepository.GetByIdsAsync(employeesIds);
                if (!listEmployees.Any())
                {
                    await _unitOfWork.RollbackAsync();
                    return ResponseResult<CreateCompanyResponse>.ValidationError("Some of provided Employees ids doesn't exist");
                }

                // clear employees, because we want to get existed + newly created with correct ids
                companyResult.Value.ClearEmployees();

                foreach (IEmployee employee in listEmployees)
                {
                    // Try to add employee to company (domain)
                    Result addEmployeeResult = companyResult.Value.Add(employee);
                    if (addEmployeeResult.IsFailure)
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateCompanyResponse>.ValidationError(addEmployeeResult.Error);
                    }
                }

                IList<IEmployment> employments = new List<IEmployment>();

                foreach (Guid id in companyResult.Value.EmployeesReadOnly
                    .Select(e => e.Id)
                    .ToList())
                {
                    employments.Add(new Employment
                    {
                        CompanyId = companyId,
                        EmployeeId = id
                    });
                }

                // employments to add
                if (employments.Any())
                {
                    int added = await _employmentRepository.AddRangeAsync(employments);
                    if (added == 0)
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateCompanyResponse>.ValidationError("Employments can't be added correctly");
                    }
                }
            }

            await _unitOfWork.CommitAsync();

            CreateCompanyResponse response = new CreateCompanyResponse
            {
                Id = companyId
            };

            return ResponseResult<CreateCompanyResponse>.Success(response);
        }
    }
}
