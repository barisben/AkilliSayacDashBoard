using AkilliSayac.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkilliSayac.Data
{
    public class DbContext : IdentityDbContext
    {

        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }
    }
}
