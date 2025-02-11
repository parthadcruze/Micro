using System.Collections.Generic;
using Microbiology.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Microbiology.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
    
        public DbSet<Users> Login { get; set; }      
        
        
    }
}
