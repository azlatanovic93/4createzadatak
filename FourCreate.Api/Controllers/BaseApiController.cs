using FourCreate.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FourCreate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IMediator _mediator;

        public BaseApiController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        protected ActionResult ResponseHandler<T>(ResponseResult<T> response)
        {
            return response.StatusCode switch
            {
                200 => Ok(response.Payload),
                204 => NoContent(),
                400 => BadRequest(response.FailureReason),
                404 => NotFound(response.FailureReason),
                _ => throw new Exception($"Not supported status code {response.StatusCode}")
            };
        }

    }

}
