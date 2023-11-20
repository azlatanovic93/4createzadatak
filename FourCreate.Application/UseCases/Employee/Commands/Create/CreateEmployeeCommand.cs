using FourCreate.Application.Common;
using MediatR;

namespace FourCreate.Application.UseCases.Employee.Commands.Create
{
    public class CreateEmployeeCommand : IRequest<ResponseResult<CreateEmployeeResponse>>
    {
        public string Email { get; set; }
        public string Title { get; set; }
        public List<long> CompanyIds { get; set; } = new List<long>();
    }
}
