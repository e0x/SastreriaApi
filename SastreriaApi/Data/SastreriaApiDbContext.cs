using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SastreriaApi.Models;

namespace SastreriaApi.Data
{
    public class SastreriaApiDbContext : DbContext
    {
        public SastreriaApiDbContext(DbContextOptions<SastreriaApiDbContext> options):base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Client> Clients { get; set; }

        public DbSet<Payment> payments { get; set; }


    
    }
}
