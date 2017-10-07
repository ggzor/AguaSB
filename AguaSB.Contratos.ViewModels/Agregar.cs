using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using System.Waf.Foundation;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : Model, IViewModel
    {
        #region Campos
        private Contrato contrato;
        #endregion

        #region Propiedades
        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }
        #endregion

        public INodo Nodo { get; }

        public Agregar()
        {
            Nodo = new NodoHoja();
        }
    }
}
