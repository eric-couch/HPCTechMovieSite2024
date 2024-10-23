using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared;

public class UserEditDto
{
    public string Id { get; set; }
    [EmailAddress]
    public string UserName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [MinLength(3, ErrorMessage="Your name isn't that short")]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool Admin { get; set; }
}
