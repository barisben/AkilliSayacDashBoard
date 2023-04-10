using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkilliSayac.Data
{
    public class ApplicationDbContext : IdentityDbContext<AkilliSayacUser>
    {
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<LogType> LogTypes { get; set; }
        public virtual DbSet<MalwareType> MalwareTypes { get; set; }
        public virtual DbSet<AnomalyType> AnomalyTypes { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Malware> Malwares { get; set; }
        public virtual DbSet<Anomaly> Anomalies { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
