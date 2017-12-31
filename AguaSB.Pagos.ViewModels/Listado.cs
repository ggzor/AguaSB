using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using Mehdime.Entity;
using AguaSB.Datos;
using AguaSB.Nucleo;
using System.Waf.Foundation;
using AguaSB.Pagos.ViewModels.Dtos;

namespace AguaSB.Pagos.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Campos
        private BusquedaPagos busqueda;
        private ResultadoBusquedaPagos resultados = new ResultadoBusquedaPagos(null);
        #endregion

        #region Propiedades
        public BusquedaPagos Busqueda
        {
            get { return busqueda; }
            set { SetProperty(ref busqueda, value); }
        }

        public ResultadoBusquedaPagos Resultados
        {
            get { return resultados; }
            set { SetProperty(ref resultados, value); }
        }
        #endregion

        #region Dependencias
        private IDbContextScopeFactory Ambito { get; }
        private IRepositorio<Pago> PagosRepo { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado(IDbContextScopeFactory ambito, IRepositorio<Pago> pagosRepo)
        {
            Ambito = ambito;
            PagosRepo = pagosRepo;

            Nodo = new Nodo { Entrada = Entrar };
        }

        private Task Entrar(object arg)
        {
            Buscar();

            return Task.CompletedTask;
        }

        private void Buscar()
        {
            using (Ambito.CreateReadOnly())
            {
                var busqueda = Busqueda = new BusquedaPagos();
                Resultados = busqueda.Ejecutar(PagosRepo.Datos);
            }
        }
    }
}
