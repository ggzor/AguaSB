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
        public const string ValorDeListaRequerido = "El valor proporcionado debe estar en la lista.";

        #region Campos
        private Contrato contrato;
        private Seccion seccion;
        private Calle calle;
        private IEnumerable<Seccion> secciones;
        private IEnumerable<Calle> calles;
        #endregion

        #region Propiedades
        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }

        [Required(ErrorMessage = ValorDeListaRequerido)]
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

        [Required(ErrorMessage = ValorDeListaRequerido)]
        public Calle Calle
        {
            get { return calle; }
            set { SetPropertyAndValidate(ref calle, value); }
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
        private IRepositorio<Seccion> SeccionesRepo { get; }
        private IRepositorio<Calle> CallesRepo { get; }
        #endregion

        public INodo Nodo { get; }

        public Agregar(IRepositorio<Seccion> secciones, IRepositorio<Calle> calles)
        {
            SeccionesRepo = secciones ?? throw new ArgumentNullException(nameof(secciones));
            CallesRepo = calles ?? throw new ArgumentNullException(nameof(calles));

            ReestablecerComando = new DelegateCommand(Reestablecer);

            Nodo = new NodoHoja() { PrimeraEntrada = Inicializar };
        }

        private Dictionary<Seccion, IEnumerable<Calle>> callesAgrupadas;

        private const string Text = "The ComboBox is an ItemsControl, so it can display content other than simple strings. For example, you can create a ComboBox that contains a list of images. When you have content other than strings in the ComboBox, a nonsensical string might appear in the ComboBox when the drop-down list is hidden. To display a string in the ComboBox when it contains non-string items, use the Text or TextPath attached property. ";

        private async Task Inicializar()
        {
            var secciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Select(seccion => new Seccion { Nombre = seccion }).ToArray();

            foreach (var seccion in secciones)
                await SeccionesRepo.Agregar(seccion);

            var callesD = Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var calles = from num in callesD.Batch(20).Index()
                         let index = num.Key
                         let values = num.Value
                         from v in values
                         select new Calle { Nombre = v, Seccion = secciones[index] };

            foreach (var calle in calles)
                await CallesRepo.Agregar(calle);

            callesAgrupadas = await Task.Run(() =>
                CallesRepo.Datos
                .GroupBy(calle => calle.Seccion)
                .ToDictionary(g => g.Key, g => (IEnumerable<Calle>)g));

            Secciones = callesAgrupadas.Keys;
            Seccion = callesAgrupadas.Keys.FirstOrDefault();
        }

        private void Reestablecer()
        {

        }
    }
}
