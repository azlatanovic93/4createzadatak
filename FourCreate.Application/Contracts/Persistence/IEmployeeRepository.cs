using FourCreate.Domain.Entities.Employee;

namespace FourCreate.Application.Contracts.Persistence
{
    public interface IEmployeeRepository : IGenericRepository<IEmployee, Guid>
    {
        // SPECIFICATION PATTERN CAN BE APPLIED HERE
        // FOR NOW I DON'T NEED THEM YET
        Task<IList<IEmployee>> GetByIdsAsync(IList<Guid> uuids);
        Task<IEmployee> GetByEmailAsync(string email);
        Task<IList<IEmployee>> GetByEmailsAsync(IList<string> emails);
        Task<IList<Guid>> AddRangeAsync(IList<IEmployee> entities);
    }
}
