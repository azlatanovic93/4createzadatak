using FourCreate.Persistence.DataModels;

namespace FourCreate.Persistence.Factories
{
    public static class EmployeeDataModelFactory
    {
        public static Employee Create(Domain.Entities.Employee.IEmployee employee)
        {
            return new Employee
            {
                Id = Guid.Empty,
                Email = employee.Email.Value,
                Title = employee.Title.DisplayName
            };
        }
    }
}
