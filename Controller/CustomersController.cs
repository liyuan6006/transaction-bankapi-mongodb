using BankApi.Implementation;
using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
            var sw = Stopwatch.StartNew();
            var customer =
                await _service.GetCustomer(
                    customerId);
            sw.Stop();

            Console.WriteLine(
                $"Elapsed = {sw.ElapsedMilliseconds} ms");
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


        [HttpPut("{customerId}")]
        public async Task<IActionResult> Update(string customerId,Customer customer)
        {
            if (customerId != customer.Id)
            {
                return BadRequest(
                    "CustomerId mismatch");
            }

            await _repository.Update(
                customer);

            await _service.RemoveCustomerCache(
                customerId);

            return Ok(
                "Customer updated and cache cleared");
        }



        [HttpPost("{customerId}")]
        public async Task<IActionResult>SaveFeatures(string customerId,CustomerFeatures features)
        {
            await _service.SaveFeatures(
                customerId,
                features);

            return Ok();
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult>GetFeatures(string customerId)
        {
            var features =
                await _service.GetFeatures(
                    customerId);

            if (features == null)
                return NotFound();

            return Ok(features);
        }
    }
}