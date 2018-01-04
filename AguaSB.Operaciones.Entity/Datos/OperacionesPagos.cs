using System;
using System.Linq;

using Mehdime.Entity;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Datos.Entity
{
    public class OperacionesPagos : IOperacionesPagos
    {
        #region Inicializacion
        public IAmbientDbContextLocator Localizador { get; }

        public EntidadesDbContext BaseDeDatos
        {
            get
            {
                return Localizador.Get<EntidadesDbContext>()
                    ?? throw new InvalidOperationException("No se pudo obtener la base de datos en el contexto actual.");
            }
        }

        public OperacionesPagos(IAmbientDbContextLocator localizador)
        {
            Localizador = localizador ?? throw new ArgumentNullException(nameof(localizador));
        }
        #endregion

        public IQueryable<Pago> Todos => BaseDeDatos.Pagos;

        public void Hacer(Pago pago)
        {

        }

        public void Deshacer(Pago pago)
        {

        }
    }
}
