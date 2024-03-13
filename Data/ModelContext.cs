using Microsoft.EntityFrameworkCore;
using MultipleUserLoginForm.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Data
{
    public class ModelContext:DbContext
    {
        public DbSet<Subject> Subjects {get;  set; }

        public DbSet<Model.Object> Objects { get; set; }
        public string Path {  get; set; }

        public ModelContext()
        {
            Path = Directory.GetCurrentDirectory() + "//data.db";

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = $"Data Source={Path}";
            optionsBuilder.UseSqlite(connectionString);

        }
    }
}
