using BankApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult>
        GetAll()
    {
        return Ok(
            await _repository.GetAll());
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

    [HttpGet("search-fuzzy")]
    public async Task<IActionResult>
    SearchFuzzy(
        [FromQuery] string term)
    {
        return Ok(
            await _repository
                .SearchNotesFuzzy(term));
    }


    [HttpGet("search-regex")]
    public async Task<IActionResult>
    SearchRegex(
        [FromQuery] string term)
    {
        return Ok(
            await _repository
                .SearchNotesRegex(term));
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
}
