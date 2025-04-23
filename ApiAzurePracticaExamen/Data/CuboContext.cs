using ApiAzurePracticaExamen.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAzurePracticaExamen.Data
{
    public class CuboContext : DbContext
    {
        public CuboContext(DbContextOptions<CuboContext> options) : base(options) { }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
