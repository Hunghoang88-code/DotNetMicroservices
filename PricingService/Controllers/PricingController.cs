using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PricingService.Api.Commands;

namespace PricingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PricingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CalculatePriceCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return new JsonResult(result);
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage
                }));
            }
        }
    }
}
