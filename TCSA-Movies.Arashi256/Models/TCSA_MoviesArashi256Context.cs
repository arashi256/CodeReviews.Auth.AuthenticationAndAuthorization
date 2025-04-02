using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TCSA_Movies.Arashi256.Models
{
    public class TCSA_MoviesArashi256Context : IdentityDbContext
    {
        public TCSA_MoviesArashi256Context (DbContextOptions<TCSA_MoviesArashi256Context> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; } = default!;
    }
}
