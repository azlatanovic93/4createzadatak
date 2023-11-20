using CSharpFunctionalExtensions;
using FourCreate.Application.Common;
using FourCreate.Application.Contracts.Persistence;
using FourCreate.Domain.Entities.Company;
using FourCreate.Domain.Entities.Employee;
using FourCreate.Domain.Entities.Employee.Factories;
using FourCreate.Domain.Entities.Employment;
using MediatR;

namespace FourCreate.Application.UseCases.Employee.Commands.Create
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, ResponseResult<CreateEmployeeResponse>>
    {
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmployeeHandler(
            IEmployeeFactory employeeFactory,
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IEmploymentRepository employmentRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeFactory = employeeFactory;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _companyRepository = companyRepository;
            _employmentRepository = employmentRepository;
        }

        public async Task<ResponseResult<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            Result<IEmployee> employeeResult = _employeeFactory.Create(
                request.Title,
                request.Email,
                Guid.Empty);

            if (employeeResult.IsFailure)
            {
                return ResponseResult<CreateEmployeeResponse>.ValidationError(employeeResult.Error);
            }

            IEmployee existingEmployee = await _employeeRepository.GetByEmailAsync(request.Email);
            if (!(existingEmployee is NoEmployee))
            {
                return ResponseResult<CreateEmployeeResponse>.ValidationError($"Employee with email {request.Email} already exists");
            }

            await _unitOfWork.CreateTransactionAsync();

            Guid id = await _employeeRepository
                .AddAsync(employeeResult.Value);

            // if need to be added to companies
            if (request.CompanyIds.Any())
            {
                IList<ICompany> companies = await _companyRepository.GetByIdsAsync(request.CompanyIds);
                if (!companies.Any())
                {
                    await _unitOfWork.RollbackAsync();
                    return ResponseResult<CreateEmployeeResponse>.ValidationError("Some of the provided Companies ids don't exist");
                }

                IList<IEmployment> employments = new List<IEmployment>();

                foreach (ICompany company in companies)
                {
                    Result addEmployeeResult = company.Add(employeeResult.Value);
                    if (addEmployeeResult.IsFailure)
                    {
                        await _unitOfWork.RollbackAsync();
                        return ResponseResult<CreateEmployeeResponse>.ValidationError(addEmployeeResult.Error);
                    }

                    employments.Add(new Employment
                    {
                        CompanyId = company.Id,
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
                        return ResponseResult<CreateEmployeeResponse>.ValidationError("Employments can't be added correctly");
                    }
                }
            }

            await _unitOfWork.CommitAsync();

            CreateEmployeeResponse response = new CreateEmployeeResponse
            {
                Id = id
            };

            return ResponseResult<CreateEmployeeResponse>.Success(response);
        }

    }
}
