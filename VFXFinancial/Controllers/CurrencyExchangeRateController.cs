using Core.Dtos;
using Core.Features.CurrencyExchangeRate.Commands;
using Core.Features.CurrencyExchangeRate.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VFXFinancial.Controllers;

[ApiController]
[Route("foreignExchange")]
public sealed class CurrencyExchangeRateController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<CurrencyExchangeRateDto> GetCurrencyExchangeRateController(
        [FromQuery] CurrencyExchangeRateFilter filter)
    {
        return await _mediator.Send(new GetCurrencyExchangeRate.Command(filter));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCurrencyExchangeRateController([FromBody] CurrencyExchangeRateCreateDto dto)
    {
        await _mediator.Send(new CreateCurrencyExchangeRate.Command(dto));
        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCurrencyExchangeRateController(
        [FromBody] UpdateCurrencyExchangeRate.Command dto)
    {
        await _mediator.Send(dto);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCurrencyExchangeRateController(DeleteCurrencyExchangeRate.Command dto)
    {
        await _mediator.Send(dto);
        return NoContent();
    }
}