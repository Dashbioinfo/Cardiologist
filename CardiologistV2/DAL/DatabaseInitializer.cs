using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CardiologistV2.DAL
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    //public class DatabaseInitializer : DropCreateDatabaseAlways<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {

        }
    }
}