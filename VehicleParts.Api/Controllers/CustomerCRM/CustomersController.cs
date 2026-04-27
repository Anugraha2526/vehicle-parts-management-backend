using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.CustomerCRM.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerCRM;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerCrmService _customerCrmService;

    public CustomersController(ICustomerCrmService customerCrmService)
    {
        _customerCrmService = customerCrmService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCustomer(CancellationToken cancellationToken)
    {
        var result = await _customerCrmService.RegisterCustomerAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> SearchCustomers([FromQuery] string query, CancellationToken cancellationToken)
    {
        var result = await _customerCrmService.SearchCustomersAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetCustomerHistory(int id, CancellationToken cancellationToken)
    {
        var result = await _customerCrmService.GetCustomerHistoryAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, CancellationToken cancellationToken)
    {
        var result = await _customerCrmService.UpdateProfileAsync(id, cancellationToken);
        return Ok(result);
    }
}
