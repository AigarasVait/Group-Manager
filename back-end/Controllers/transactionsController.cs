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

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto transaction )
    {
        var newTransaction = new Transaction
        {
            Amount = transaction.Amount,
            Date = DateTime.Now, // only works if user is where the server is
            Description = transaction.Description,
            PayerId = transaction.PayerId,
            GroupId = transaction.GroupId,
            SType = transaction.SType,
            SplitValues = transaction.SplitValues
        };
        
        var group = await _db.Groups
                            .Include(g => g.Members)
                            .FirstOrDefaultAsync(g => g.Id == transaction.GroupId);
        var payer = await _db.Users.FindAsync(transaction.PayerId);
        if (group == null || payer == null)
        {
            return BadRequest("Payer user not found in the group, or group does not exist.");
        }
            
        await TransactionSplitter.Split(newTransaction, group, payer, _db);

        _db.Transactions.Add(newTransaction);
        await _db.SaveChangesAsync();
        var transactionDto = _mapper.Map<TransactionDto>(newTransaction);
        return Created("", transactionDto);
    }
}