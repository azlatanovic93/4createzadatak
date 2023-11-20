using FourCreate.Persistence.DataModels;

namespace FourCreate.Persistence.Factories
{
    public static class EmploymentDataModelFactory
    {
        public static Employment Create(Domain.Entities.Employment.IEmployment employment)
        {
            return new Employment
            {
                CompanyId = employment.CompanyId,
                EmployeeId = employment.EmployeeId
            };
        }
    }
}
