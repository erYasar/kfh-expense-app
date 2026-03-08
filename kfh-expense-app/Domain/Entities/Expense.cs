using System.ComponentModel.DataAnnotations;

namespace kfh_expense_app.Domain.Entities;
public class Expense
{
    public int Id { get; set; }
    [Required, MaxLength(20)]
    public string Title { get; set; }
    public decimal Amount { get; set; }
    [MaxLength(3)]
    public string Currency { get; set; }
    public string Category { get; set; }
    public DateOnly OccurredOn { get; set; }
    public ExpenseStatus Status { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid RowVersion { get; set; }
}

public enum ExpenseStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
