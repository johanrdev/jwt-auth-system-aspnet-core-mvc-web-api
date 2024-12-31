using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.API.Data {
    public class AppDbContext : IdentityDbContext<IdentityUser> {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}