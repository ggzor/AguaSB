﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

using GGUtils.MVVM.Async;
using MoreLinq;

using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos
        private bool mostrarMensajeError = true;
        private bool puedeReestablecer = true;

        private Contrato contrato;

        private TipoContrato tipoContrato;
        private Seccion seccion;
        private Calle calle;

        private IEnumerable<TipoContrato> tiposContrato = Enumerable.Empty<TipoContrato>();
        private IEnumerable<Seccion> secciones = Enumerable.Empty<Seccion>();
        private IEnumerable<Calle> calles = Enumerable.Empty<Calle>();
        #endregion

        #region Propiedades
        public bool MostrarMensajeError
        {
            get { return mostrarMensajeError; }
            set { SetProperty(ref mostrarMensajeError, value); }
        }

        public bool PuedeReestablecer
        {
            get { return puedeReestablecer; }
            set
            {
                puedeReestablecer = value;
                ReestablecerComando.RaiseCanExecuteChanged();
            }
        }

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

        public IEnumerable<string> SugerenciasMedidasToma { get; } = new[] { "1/2", "1", "1 1/2", "2" };
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; }

        public AsyncDelegateCommand<int> AgregarContratoComando { get; }
        #endregion

        #region Dependencias
        private IRepositorio<Contrato> Contratos { get; }
        private IRepositorio<TipoContrato> TiposContratoRepo { get; }
        private IRepositorio<Seccion> SeccionesRepo { get; }
        private IRepositorio<Calle> CallesRepo { get; }
        #endregion

        public INodo Nodo { get; }

        public Agregar(IRepositorio<Contrato> contratos, IRepositorio<TipoContrato> tiposContrato, IRepositorio<Seccion> secciones, IRepositorio<Calle> calles)
        {
            Contratos = contratos ?? throw new ArgumentNullException(nameof(contratos));
            TiposContratoRepo = tiposContrato ?? throw new ArgumentNullException(nameof(tiposContrato));
            SeccionesRepo = secciones ?? throw new ArgumentNullException(nameof(secciones));
            CallesRepo = calles ?? throw new ArgumentNullException(nameof(calles));

            AgregarContratoComando = new AsyncDelegateCommand<int>(AgregarContrato, PuedeAgregarContrato);
            ReestablecerComando = new DelegateCommand(Reestablecer, () => PuedeReestablecer);

            Nodo = new NodoHoja() { PrimeraEntrada = Inicializar };

            Reestablecer();

            new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[]
                {
                    this, Contrato, Contrato.Domicilio
                },
                () => Enumerable.Empty<INotifyPropertyChanged>(),
                () => new ICommand[]
                {
                    AgregarContratoComando, ReestablecerComando
                });
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
            MostrarMensajeError = false;
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

        private bool PuedeAgregarContrato() =>
            UtileriasErrores.NingunoTieneErrores(this, Contrato, Contrato.Domicilio)
            && !Contrato.TieneCamposRequeridosVacios
            && !Contrato.Domicilio.TieneCamposRequeridosVacios;

        private async Task<int> AgregarContrato(IProgress<(double, string)> progreso)
        {
            MostrarMensajeError = true;
            PuedeReestablecer = false;

            try
            {
                progreso.Report((0.0, "Agregando contrato..."));
                var resultado = await Contratos.Agregar(Contrato);

                progreso.Report((100.0, "Completado."));

                PuedeReestablecer = true;
                Reestablecer();

                return resultado.Id;
            }
            finally
            {
                PuedeReestablecer = true;
            }
        }
    }
}
