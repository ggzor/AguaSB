using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;
using MoreLinq;

using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.ViewModels;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos
        private Contrato contrato;

        private TipoContrato tipoContrato;
        private Seccion seccion;
        private Calle calle;

        private IEnumerable<TipoContrato> tiposContrato = Enumerable.Empty<TipoContrato>();
        private IEnumerable<Seccion> secciones = Enumerable.Empty<Seccion>();
        private IEnumerable<Calle> calles = Enumerable.Empty<Calle>();
        #endregion

        #region Propiedades
        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }

        [Required(ErrorMessage = "Debe seleccionar un tipo de contrato existente.")]
        public TipoContrato TipoContrato
        {
            get { return tipoContrato; }
            set { SetPropertyAndValidate(ref tipoContrato, value); }
        }

        [Required(ErrorMessage = "Debe seleccionar una sección existente.")]
        public Seccion Seccion
        {
            get { return seccion; }
            set
            {
                SetPropertyAndValidate(ref seccion, value);
                if (value != null)
                {
                    Calles = callesAgrupadas[value];
                    Calle = Calles.FirstOrDefault();
                }
                else
                {
                    Calle = null;
                    Calles = null;
                }
            }
        }

        [Required(ErrorMessage = "Debe seleccionar una calle registrada.")]
        public Calle Calle
        {
            get { return calle; }
            set { SetPropertyAndValidate(ref calle, value); }
        }

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return tiposContrato; }
            set { SetProperty(ref tiposContrato, value); }
        }

        public IEnumerable<Seccion> Secciones
        {
            get { return secciones; }
            set { SetProperty(ref secciones, value); }
        }

        public IEnumerable<Calle> Calles
        {
            get { return calles; }
            set { SetProperty(ref calles, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; set; }
        #endregion

        #region Dependencias
        private IRepositorio<TipoContrato> TiposContratoRepo { get; }
        private IRepositorio<Seccion> SeccionesRepo { get; }
        private IRepositorio<Calle> CallesRepo { get; }
        #endregion

        public INodo Nodo { get; }

        public Agregar(IRepositorio<TipoContrato> tiposContrato, IRepositorio<Seccion> secciones, IRepositorio<Calle> calles)
        {
            TiposContratoRepo = tiposContrato ?? throw new ArgumentNullException(nameof(tiposContrato));
            SeccionesRepo = secciones ?? throw new ArgumentNullException(nameof(secciones));
            CallesRepo = calles ?? throw new ArgumentNullException(nameof(calles));

            ReestablecerComando = new DelegateCommand(Reestablecer);

            Nodo = new NodoHoja() { PrimeraEntrada = Inicializar };

            Reestablecer();
        }

        private Dictionary<Seccion, IEnumerable<Calle>> callesAgrupadas;

        private async Task Inicializar()
        {
            callesAgrupadas = await Task.Run(() =>
            {
                return (from seccion in SeccionesRepo.Datos
                        orderby seccion.Orden, seccion.Nombre
                        select new { Seccion = seccion, Calles = seccion.Calles.OrderBy(calle => calle.Nombre) })
                       .ToDictionary(g => g.Seccion, g => (IEnumerable<Calle>)g.Calles);
            });

            TiposContrato = await Task.Run(() =>
            {
                return (from tipo in TiposContratoRepo.Datos
                        orderby tipo.Nombre
                        select tipo).ToList();
            });

            Secciones = callesAgrupadas.Keys;

            ReestablecerTipoContratoYCalles();
        }

        private void Reestablecer()
        {
            Contrato = new Contrato()
            {
                Domicilio = new Domicilio()
            };

            ReestablecerTipoContratoYCalles();
        }

        private void ReestablecerTipoContratoYCalles()
        {
            TipoContrato = TiposContrato.FirstOrDefault();
            Seccion = Secciones.FirstOrDefault();
        }
    }
}
