using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CodeTweet.IdentityDal.Model
{
    public class ApplicationUser : IdentityUser
    {
        public bool Notifications { get; set; }
    }
}