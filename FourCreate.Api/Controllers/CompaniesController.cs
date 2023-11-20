using FourCreate.Application.Common;
using FourCreate.Application.UseCases.Company.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FourCreate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : BaseApiController
    {
        public CompaniesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        // Swagger needs this
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCompanyResponse))]
        public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
        {
            ResponseResult<CreateCompanyResponse> result = await _mediator.Send(command);
            return ResponseHandler(result);
        }
    }
}
