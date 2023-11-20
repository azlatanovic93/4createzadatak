using CSharpFunctionalExtensions;
using FourCreate.Application.Common;
using FourCreate.Application.Contracts.Persistence;
using FourCreate.Application.UseCases.Employee.Commands.Create;
using FourCreate.Domain.Entities.Company;
using FourCreate.Domain.Entities.Company.ValueObjects;
using FourCreate.Domain.Entities.Employee;
using FourCreate.Domain.Entities.Employee.Enumerations;
using FourCreate.Domain.Entities.Employee.Factories;
using FourCreate.Domain.Entities.Employee.ValueObjects;
using FourCreate.Domain.Entities.Employment;
using Moq;
using Shouldly;

namespace FourCreate.Test.UseCases.Employee
{
    public class CreateEmployeeTests
    {
        private Mock<IEmployeeFactory> _employeeFactory;
        private Mock<IEmployeeRepository> _employeeRepository;
        private Mock<ICompanyRepository> _companyRepository;
        private Mock<IEmploymentRepository> _employmentRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private CreateEmployeeHandler _createEmployeeHandler;
        private ResponseResult<CreateEmployeeResponse> _responseResult;

        private static CreateEmployeeCommand CreateEmployeeCommand =>
            new CreateEmployeeCommand
            {
                Email = "my@demo.com",
                Title = "developer",
                CompanyIds = { 1 }
            };


        private static CreateEmployeeCommand CreateEmployeeCommandWithInvalidCompanyId =>
            new CreateEmployeeCommand
            {
                Email = "my@demo.com",
                Title = "developer",
                CompanyIds = { 101 }
            };

        private static CreateEmployeeCommand CreateEmployeeCommandWithInvalidEmail =>
            new CreateEmployeeCommand
            {
                Email = "mydemogmail.com",
                Title = "developer",
                CompanyIds = { 1 }
            };

        public CreateEmployeeTests()
        {
            _employeeFactory = new Mock<IEmployeeFactory>();
            _employeeRepository = new Mock<IEmployeeRepository>();
            _companyRepository = new Mock<ICompanyRepository>();
            _employmentRepository = new Mock<IEmploymentRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();


            _createEmployeeHandler = new CreateEmployeeHandler(
                _employeeFactory.Object,
                _employeeRepository.Object,
                _companyRepository.Object,
                _employmentRepository.Object,
                _unitOfWork.Object);
        }

        [Fact]
        public void When_email_is_invalid_respond_with_a_validation_error_and_terminate()
        {
            _employeeFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Returns(Result.Failure<IEmployee>("Email is invalid"));

            _responseResult = _createEmployeeHandler.Handle(CreateEmployeeCommandWithInvalidEmail, default).Result;
            _responseResult.StatusCode.ShouldBe(400);
            _responseResult.FailureReason.ShouldBe("Email is invalid");
        }

        [Fact]
        public void When_employee_already_exists_respond_with_a_validation_error_and_terminate()
        {
            Mock<IEmployee> employee = new Mock<IEmployee>();
            employee.Setup(x => x.Email).Returns(Email.Create("my@demo.com").Value);
            employee.Setup(x => x.Id).Returns(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));
            employee.Setup(x => x.Title).Returns(Title.Developer);

            _employeeFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Returns(Result.Success(new Mock<IEmployee>().Object));

            _employeeRepository
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(employee.Object);

            _responseResult = _createEmployeeHandler.Handle(CreateEmployeeCommand, default).Result;
            _responseResult.StatusCode.ShouldBe(400);
            _responseResult.FailureReason.ShouldBe($"Employee with email my@demo.com already exists");

            _employeeRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void When_company_id_is_wrong_respond_with_a_validation_error_and_terminate()
        {
            Mock<IEmployee> employee = new Mock<IEmployee>();
            employee.Setup(x => x.Email).Returns(Email.Create("my@demo.com").Value);
            employee.Setup(x => x.Id).Returns(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));
            employee.Setup(x => x.Title).Returns(Title.Developer);

            _employeeFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Returns(Result.Success(new Mock<IEmployee>().Object));

            _employeeRepository
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new NoEmployee());

            _employeeRepository
                .Setup(x => x.AddAsync(It.IsAny<IEmployee>()))
                .ReturnsAsync(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));

            _companyRepository
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<ICompany>());

            _responseResult = _createEmployeeHandler.Handle(CreateEmployeeCommandWithInvalidCompanyId, default).Result;
            _responseResult.StatusCode.ShouldBe(400);
            _responseResult.FailureReason.ShouldBe("Some of the provided Companies ids don't exist");

            _employeeRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Once);
            _employeeRepository.Verify(x => x.AddAsync(It.IsAny<IEmployee>()), Times.Once);
            _companyRepository.Verify(x => x.GetByIdsAsync(It.IsAny<List<long>>()), Times.Once);
        }

        [Fact]
        public void When_employee_developer_already_exists_respond_with_a_validation_error_and_terminate()
        {
            Mock<IEmployee> employee = new Mock<IEmployee>();
            employee.Setup(x => x.Email).Returns(Email.Create("my@demo.com").Value);
            employee.Setup(x => x.Id).Returns(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));
            employee.Setup(x => x.Title).Returns(Title.Developer);

            Mock<ICompany> company = new Mock<ICompany>();
            company.Setup(x => x.Name).Returns(Name.Create("Demo company").Value);
            company.Setup(x => x.Id).Returns(1);
            company.Setup(x => x.Add(It.IsAny<IEmployee>())).Returns(Result.Failure($"Company can't have more than 1 DEVELOPER"));

            _employeeFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Returns(Result.Success(new Mock<IEmployee>().Object));

            _employeeRepository
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new NoEmployee());

            _employeeRepository
                .Setup(x => x.AddAsync(It.IsAny<IEmployee>()))
                .ReturnsAsync(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));

            _companyRepository
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<ICompany>() { company.Object });

            _responseResult = _createEmployeeHandler.Handle(CreateEmployeeCommand, default).Result;
            _responseResult.StatusCode.ShouldBe(400);
            _responseResult.FailureReason.ShouldBe($"Company can't have more than 1 DEVELOPER");

            _employeeRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Once);
            _employeeRepository.Verify(x => x.AddAsync(It.IsAny<IEmployee>()), Times.Once);
            _companyRepository.Verify(x => x.GetByIdsAsync(It.IsAny<List<long>>()), Times.Once);
        }

        [Fact]
        public void Execute_use_case_in_happy_path()
        {
            Mock<IEmployee> employee = new Mock<IEmployee>();
            employee.Setup(x => x.Email).Returns(Email.Create("my@demo.com").Value);
            employee.Setup(x => x.Id).Returns(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));
            employee.Setup(x => x.Title).Returns(Title.Developer);

            Mock<ICompany> company = new Mock<ICompany>();
            company.Setup(x => x.Name).Returns(Name.Create("Demo company").Value);
            company.Setup(x => x.Id).Returns(1);
            company.Setup(x => x.Add(It.IsAny<IEmployee>())).Returns(Result.Success);

            _employeeFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Returns(Result.Success(new Mock<IEmployee>().Object));

            _employeeRepository
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new NoEmployee());

            _employeeRepository
                .Setup(x => x.AddAsync(It.IsAny<IEmployee>()))
                .ReturnsAsync(new Guid("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));

            _companyRepository
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<ICompany>() { company.Object });

            _employmentRepository
                .Setup(x => x.AddRangeAsync(It.IsAny<IList<IEmployment>>()))
                .ReturnsAsync(1);

            _responseResult = _createEmployeeHandler.Handle(CreateEmployeeCommand, default).Result;
            _responseResult.StatusCode.ShouldBe(200);
            _responseResult.Payload.Id.ShouldBe(Guid.Parse("58bf83fe-d6a8-48bf-956c-03d0417f7a8e"));

            _employeeRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Once);
            _employeeRepository.Verify(x => x.AddAsync(It.IsAny<IEmployee>()), Times.Once);
            _companyRepository.Verify(x => x.GetByIdsAsync(It.IsAny<List<long>>()), Times.Once);
            _employmentRepository.Verify(x => x.AddRangeAsync(It.IsAny<IList<IEmployment>>()), Times.Once);
        }
    }
}
