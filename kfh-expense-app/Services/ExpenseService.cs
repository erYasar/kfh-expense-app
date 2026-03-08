using kfh_expense_app.Domain.Entities;
using kfh_expense_app.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace kfh_expense_app.Services;


public record PagedResult<T>(IEnumerable<T> Items,
    int TotalCount, int Page, int PageSize);

public record ExpenseDto(int Id, string Title, decimal Amount, string Currency, string Category, DateOnly OccurredOn,
    string Status, string CreatedByUserId, DateTime CreatedAtUtc, Guid RowVersion);

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;
    private readonly AuthSessionService _auth;
    public ExpenseService(AppDbContext context, AuthSessionService auth)
    {
        _context = context;
        _auth = auth;
    }

    public async Task<PagedResult<ExpenseDto>> GetPagedResultAsync(string? status, int page, int pageSize)
    {
        var query = _context.Expenses.AsQueryable();

        if (!_auth.IsApprover)
        {
            query = query.Where(e => e.CreatedByUserId == _auth.UserId);
        }

        if (Enum.TryParse<ExpenseStatus>(status, true, out var s))
        {
            query = query.Where(e => e.Status == s);
        }

        var totalCount = await query.CountAsync();

        var items = await query
                .OrderByDescending(e => e.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => ToDto(e))
                .ToListAsync();

        return new PagedResult<ExpenseDto>(items, totalCount, page, pageSize);
    }



    public async Task<ExpenseDto> GetByIdAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense is null)
        {
            return null;
        }
        ;

        if (!_auth.IsApprover && expense.CreatedByUserId != _auth.UserId)
        {
            return null;
        }

        return ToDto(expense);
    }

    public async Task<ExpenseDto> CreateAsync(ExpenseDto newExpense)
    {
        var expense = new Expense
        {
            Title = newExpense.Title,
            Amount = newExpense.Amount,
            Currency = newExpense.Currency,
            Category = newExpense.Category,
            OccurredOn = newExpense.OccurredOn,
            CreatedByUserId = _auth.UserId!,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return ToDto(expense);
    }

    public Task<bool> ApproveAsync(int id)
    {
       return SetStatusAsync(id, ExpenseStatus.Approved);
    }

    public async Task<bool> RejectAsync(int id)
    {
        return await SetStatusAsync(id, ExpenseStatus.Rejected);
    }

    private async Task<bool> SetStatusAsync(int id, ExpenseStatus newStatus)
    {
        if (!_auth.IsApprover) return false;

        var expense = await _context.Expenses.FindAsync(id);
        
        if (expense is null)
        {
            return false;
        }
        
        if (expense.Status != ExpenseStatus.Pending)
        {
            return false;
        }
        
        expense.Status = newStatus;
        
        await _context.SaveChangesAsync();
        
        return true;
    }
    private static ExpenseDto ToDto(Expense e) => new(e.Id, e.Title, e.Amount, e.Currency, e.Category, e.OccurredOn,
        e.Status.ToString(), e.CreatedByUserId, e.CreatedAtUtc, e.RowVersion);

}
