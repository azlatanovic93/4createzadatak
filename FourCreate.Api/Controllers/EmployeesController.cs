using FourCreate.Application.Common;
using FourCreate.Application.UseCases.Employee.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FourCreate.Api.Controllers
{
    [ApiController]
    public class EmployeesController : BaseApiController
    {
        public EmployeesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        // Swagger needs this
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateEmployeeResponse))]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            ResponseResult<CreateEmployeeResponse> result = await _mediator.Send(command);
            return ResponseHandler(result);
        }

    }
}
