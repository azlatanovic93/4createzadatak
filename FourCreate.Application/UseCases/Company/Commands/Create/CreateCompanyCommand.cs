using FourCreate.Application.Common;
using MediatR;

namespace FourCreate.Application.UseCases.Company.Commands.Create
{
    public class CreateCompanyCommand : IRequest<ResponseResult<CreateCompanyResponse>>
    {
        public string Name { get; set; }
        public List<EmployeeItem> Employees { get; set; } = new List<EmployeeItem>();

        public class EmployeeItem
        {
            public string Email { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public Guid? Id { get; set; } = null;
        }
    }
}
