namespace FourCreate.Domain.Entities.Employment
{
    public interface IEmployment
    {
        Guid EmployeeId { get; }
        long CompanyId { get; }
    }
}
