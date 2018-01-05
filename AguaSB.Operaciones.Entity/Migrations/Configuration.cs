using System.Data.Entity.Migrations;

namespace AguaSB.Operaciones.Entity.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EntidadesDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}
