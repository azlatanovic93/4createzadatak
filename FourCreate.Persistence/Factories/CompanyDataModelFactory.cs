using FourCreate.Persistence.DataModels;

namespace FourCreate.Persistence.Factories
{
    public static class CompanyDataModelFactory
    {
        public static Company Create(Domain.Entities.Company.ICompany company)
        {
            return new Company
            {
                Id = 0,
                Name = company.Name.Value
            };
        }

    }
}
