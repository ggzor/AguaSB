using GGUtils.MVVM.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Foundation;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel
    {
        #region Comandos

        public AsyncDelegateCommand<int> AgregarComando { get; }

        #endregion

        public Agregar()
        {
            AgregarComando = new AsyncDelegateCommand<int>(AgregarUsuario, PuedeAgregarUsuario);
        }

        private bool PuedeAgregarUsuario()
        {
            return true;
        }

        private Task<int> AgregarUsuario()
        {
            return Task.FromResult(0);
        }
    }
}
