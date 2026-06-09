using BankApi.Implementation;
using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controller
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly ICustomerCacheService _service;
        public CustomersController(ICustomerRepository repository, ICustomerCacheService service)
        {
            _repository = repository;
            _service = service;
        }


        [HttpGet("{customerId}")]
        public async Task<IActionResult> Get(string customerId)
        {
            var customer =
                await _service.GetCustomer(
                    customerId);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _repository.GetAll();

            return Ok(customers);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var customer = await _repository.GetByEmail(email);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            customer.CreatedDate = DateTime.UtcNow;

            await _repository.Create(customer);

            return Ok();
        }
    }
}