using BankApi.DTO;
using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TransactionsController
    : ControllerBase
{
    private readonly ITransactionRepository
        _repository;

    public TransactionsController(
        ITransactionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult>GetAll()
    {
        return Ok(
            await _repository.GetAll());
    }


    [HttpPost]
    public async Task<IActionResult>Search([FromBody]TransactionSearchRequest request)
    {
        var results =
            await _repository
                .Search(request);

        return Ok(results);
    }

    [HttpGet()]
    public async Task<IActionResult> Autocomplete(
    [FromQuery] string term)
    {
        var result =
            await _repository
                .SearchAutocomplete(term);

        return Ok(result);
    }








    [HttpGet]
    public async Task<IActionResult>GetPaged_Traditional(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var transactions =
            await _repository
                .GetTransactions_traditional_pagination(
                    pageNumber,
                    pageSize);

        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>GetById(string id)
    {
        var transaction =await _repository.GetById(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult>
        Create(
            TransactionEvent transaction)
    {
        await _repository.Create(
            transaction);

        return CreatedAtAction(
            nameof(GetById),
            new { id = transaction.Id },
            transaction);
    }

    [HttpPut]
    public async Task<IActionResult>
        Update(
            TransactionEvent transaction)
    {
        var existing =
            await _repository.GetById(
                transaction.Id);

        if (existing == null)
        {
            return NotFound();
        }

        await _repository.Update(
            transaction);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>
        Delete(
            string id)
    {
        var existing =
            await _repository.GetById(id);

        if (existing == null)
        {
            return NotFound();
        }

        await _repository.Delete(id);

        return NoContent();
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult>
        GetByTransactionId(
            string transactionId)
    {
        var transaction =
            await _repository
                .GetByTransactionId(
                    transactionId);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult>
        GetByCustomer(
            string customerId)
    {
        return Ok(
            await _repository
                .GetByCustomer(
                    customerId));
    }

    [HttpGet("search")]
    public async Task<IActionResult>
        Search(
            [FromQuery] string term)
    {
        return Ok(
            await _repository
                .SearchNotes(term));
    }

    [HttpGet]
    public async Task<IActionResult>SearchFuzzy([FromQuery] string term)
    {
        return Ok(await _repository.SearchNotesFuzzy(term));
    }

    [HttpGet("search-regex")]
    public async Task<IActionResult>SearchRegex([FromQuery] string term)
    {
        return Ok(
            await _repository
                .SearchNotesRegex(term));
    }

    [HttpGet("top-customers")]
    public async Task<IActionResult>
        GetTopCustomers()
    {
        return Ok(
            await _repository
                .GetTopCustomers());
    }
}