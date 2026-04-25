using Microsoft.EntityFrameworkCore;
using NexEraTech.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexEraTech.Infrastructure.Data.Interface
{
    public interface INexEraTechDB
    {
        public IDbConnection Connection { get; }
        public DbSet<Users> Users { get; set; }
    }
}
