namespace kfh_expense_app.Services;

public interface IExpenseService
{
    Task<PagedResult<ExpenseDto>> GetPagedResultAsync(string? status, int page, int pageSize);
    Task<ExpenseDto> GetByIdAsync(int id);
    Task<ExpenseDto> CreateAsync(ExpenseDto dto);

    Task<bool> ApproveAsync(int id);
     Task<bool> RejectAsync(int id);
}
