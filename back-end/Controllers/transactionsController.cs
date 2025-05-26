using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Helper;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TransactionsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new transaction and splits it among group members.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto transaction)
    {
        if (transaction == null)
            return BadRequest("Invalid transaction data.");

        // Create new Transaction object from incoming request
        var newTransaction = new Transaction
        {
            Amount = transaction.Amount,
            Date = DateTime.Now, // only works if user is in the same timezone
            Description = transaction.Description,
            PayerId = transaction.PayerId,
            GroupId = transaction.GroupId,
            SType = transaction.SType,
            SplitValues = transaction.SplitValues
        };

        // Load the group and its members from DB
        var group = await _db.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == transaction.GroupId);

        // Fetch the payer (who created the transaction)
        var payer = await _db.Users.FindAsync(transaction.PayerId);

        if (group == null || payer == null)
        {
            return BadRequest("Group or payer not found.");
        }

        // Splits the tansaction among group members, and makes new DebtTracker entries
        await TransactionSplitter.Split(newTransaction, group, payer, _db);

        _db.Transactions.Add(newTransaction);
        await _db.SaveChangesAsync();

        var transactionDto = _mapper.Map<TransactionDto>(newTransaction);

        return Created(string.Empty, transactionDto);
    }
}
