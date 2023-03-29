using Microsoft.AspNetCore.Identity;

namespace AkilliSayac.Areas.Identity.Data;

public class AkilliSayacUser : IdentityUser
{
    [PersonalData]
    public string? FirstName { get; set; }
    [PersonalData]
    public string? LastName { get; set; }
    
    public string? RoleName { get; set; }
}