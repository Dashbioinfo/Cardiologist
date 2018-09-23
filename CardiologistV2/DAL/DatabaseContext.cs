using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CardiologistV2.Models;

namespace CardiologistV2.DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("DatabaseConnection") { }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Record> Records { get; set; }

        public DbSet<Features> Features { get; set; }

        public DbSet<History> Histories { get; set; }

        public DbSet<Consultation> Consultations { get; set; }

        public DbSet<User> Users { get; set; }
    }
}