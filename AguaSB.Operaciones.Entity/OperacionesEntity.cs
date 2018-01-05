using System;
using Mehdime.Entity;
using EntityRepo = AguaSB.Datos.Entity;

namespace AguaSB.Operaciones.Entity
{
    public class OperacionesEntity
    {
        private IAmbientDbContextLocator Localizador { get; }

        protected EntityRepo.EntidadesDbContext BaseDeDatos =>
            Localizador.Get<EntityRepo.EntidadesDbContext>() ?? throw new InvalidOperationException("No se pudo obtener la base de datos en el contexto actual.");

        public OperacionesEntity(IAmbientDbContextLocator localizador)
        {
            Localizador = localizador ?? throw new ArgumentNullException(nameof(localizador));
        }
    }
}
