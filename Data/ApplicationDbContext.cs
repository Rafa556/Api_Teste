using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using API_REST.Models;

namespace API_REST.Data
{
    public class ApplicationDbContext  : DbContext
    {
        public DbSet<Produto> Produtos {get; set;}

        public DbSet<Usuario> Usuarios {get; set;}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {

        }
    }
}