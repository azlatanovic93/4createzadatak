using FourCreate.Application.Contracts.Persistence;
using FourCreate.Persistence.DataModels;
using FourCreate.Persistence.Factories;

namespace FourCreate.Persistence.Repositories
{
    public class EmploymentRepository : IEmploymentRepository
    {
        private NpgsqlDbContext _dbContext;

        public EmploymentRepository(NpgsqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(Domain.Entities.Employment.IEmployment entity)
        {
            Employment employment = EmploymentDataModelFactory.Create(entity);

            await _dbContext.AddAsync(employment);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> AddRangeAsync(IList<Domain.Entities.Employment.IEmployment> entities)
        {
            List<Employment> employments = new List<Employment>();
            foreach (Domain.Entities.Employment.IEmployment entity in entities)
            {
                employments.Add(EmploymentDataModelFactory.Create(entity));
            }

            await _dbContext.AddRangeAsync(employments);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
