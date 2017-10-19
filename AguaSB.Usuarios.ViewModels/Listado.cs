using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using System.Waf.Foundation;
using System.Collections;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Nucleo;

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Campos
        private string textoBusqueda;

        private IEnumerable criteriosOrdenamiento;
        private string criterioOrdenamiento;

        private bool agrupar;
        private IEnumerable criteriosAgrupacion;

        private IEnumerable secciones;
        private IEnumerable calles;

        private FiltroDomicilio filtroDomicilio = new FiltroDomicilio();
        private FiltroRango<decimal?> filtroAdeudo = new FiltroRango<decimal?>();
        private FiltroRango<DateTime> filtroFechaRegistro = new FiltroRango<DateTime>()
        {
            Desde = new DateTime(2018, 01, 01),
            Hasta = DateTime.Today
        };
        #endregion

        #region Propiedades
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; }
        }

        public IEnumerable CriteriosOrdenamiento
        {
            get { return criteriosOrdenamiento; }
            set { SetProperty(ref criteriosOrdenamiento, value); }
        }

        public string CriterioOrdenamiento
        {
            get { return criterioOrdenamiento; }
            set { SetPropertyAndValidate(ref criterioOrdenamiento, value); }
        }

        public bool Agrupar
        {
            get { return agrupar; }
            set { agrupar = value; }
        }

        public IEnumerable CriteriosAgrupacion
        {
            get { return criteriosAgrupacion; }
            set { SetProperty(ref criteriosAgrupacion, value); }
        }

        public IEnumerable Secciones
        {
            get { return secciones; }
            set { secciones = value; }
        }

        public IEnumerable Calles
        {
            get { return calles; }
            set { calles = value; }
        }

        public FiltroDomicilio FiltroDomicilio
        {
            get { return filtroDomicilio; }
            set { SetProperty(ref filtroDomicilio, value); }
        }

        public FiltroRango<decimal?> FiltroAdeudo
        {
            get { return filtroAdeudo; }
            set { SetProperty(ref filtroAdeudo, value); }
        }

        public FiltroRango<DateTime> FiltroFechaRegistro
        {
            get { return filtroFechaRegistro; }
            set { SetProperty(ref filtroFechaRegistro, value); }
        }
        #endregion


        public INodo Nodo { get; }

        public Listado()
        {
            Nodo = new Nodo();
            CriteriosOrdenamiento = new[] { "Mayor adeudo", "Menor adeudo", "Pago reciente", "Registro reciente", "Calle" };
            CriteriosAgrupacion = new[]
            {
                new Agrupador { Nombre = "Sección" },
                new Agrupador { Nombre = "Calle" },
                new Agrupador { Nombre = "Deudor", Descripcion = "¿Es deudor o no?" },
                new Agrupador { Nombre = "Adeudo", Descripcion = "En pesos" },
                new Agrupador { Nombre = "Fecha de registro" }
            };
        }
    }
}
