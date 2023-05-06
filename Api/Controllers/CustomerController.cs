using Data.Dtos.Base;
using Data.Dtos.Customer;
using Data.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get All Customers 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<List<CustomerListDto>>))]
        public async Task<IActionResult> GetAllCustomer()
        {
            return Ok(await _customerService.GetAllCustomer());
        }

        /// <summary>
        /// Get Customers By Search
        /// </summary>
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<List<CustomerListDto>>))]
        public async Task<IActionResult> GetCustomersBySearch([FromQuery] GetCustomersBySearchRequestParameters model)
        {
            return Ok(await _customerService.GetCustomersBySearch(model));
        }

        /// <summary>
        /// Get Customer By Id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<CustomerDto>))]
        public async Task<IActionResult> GetCustomerById([FromQuery] BaseRequestParameters model)
        {
            return Ok(await _customerService.GetCustomerById(model));
        }

        /// <summary>
        /// Add Customer 
        /// </summary>
        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<bool>))]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerRequestParameters model)
        {
            return Ok(await _customerService.AddCustomer(model));
        }

        /// <summary>
        /// Romeve Customer 
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<bool>))]
        public async Task<IActionResult> RemoveCustomer([FromBody] BaseRequestParameters model)
        {
            return Ok(await _customerService.RemoveCustomer(model));
        }

        /// <summary>
        /// Update Customer 
        /// </summary>
        [HttpPut]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<bool>))]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerRequestParameters model)
        {
            return Ok(await _customerService.UpdateCustomer(model));
        }
    }
}
