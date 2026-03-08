using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace kfh_expense_app.Domain.Entities;

public class AppUser:IdentityUser
{
    public string Role { get; set; }
}
