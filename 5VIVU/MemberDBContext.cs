using _5VIVU.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.SqlServer;


public class MemberDBContext : DbContext
    {
        public MemberDBContext(DbContextOptions<MemberDBContext> options)
            : base(options)
        {
        }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-U2TNCE6;Database=5vivu_banve; User Id=sa;Password=123; TrustServerCertificate= True");
    }



    public DbSet<Member> Members { get; set; }

        
    }

