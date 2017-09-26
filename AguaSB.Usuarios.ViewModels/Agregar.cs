using AguaSB.Nucleo;
using GGUtils.MVVM.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel
    {
        #region Campos

        private Persona persona = new Persona();

        #endregion

        #region Propiedades

        private Usuario Usuario { get; set; }

        public Persona Persona
        {
            get { return persona; }
            set { SetProperty(ref persona, value); }
        }

        #endregion

        #region Comandos

        public DelegateCommand ReestablecerPersonaComando { get; }
        public DelegateCommand ReestablecerNegocioComando { get; }

        public AsyncDelegateCommand<int> AgregarUsuarioComando { get; }
        public AsyncDelegateCommand<int> AgregarPersonaComando { get; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; }

        #endregion

        public Agregar()
        {
            ReestablecerPersonaComando = new DelegateCommand(() => Persona = new Persona());
            ReestablecerNegocioComando = new DelegateCommand(() => Persona = new Persona());

            AgregarUsuarioComando = new AsyncDelegateCommand<int>(AgregarUsuario, PuedeAgregarUsuario);
            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, PuedeAgregarPersona);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, PuedeAgregarNegocio);
        }

        private bool PuedeAgregarNegocio()
        {
            return true;
        }

        private async Task<int> AgregarNegocio()
        {
            //TODO: Usuario = Negocio;
            return await EjecutarAgregarUsuario();
        }

        private bool PuedeAgregarPersona()
        {
            return true;
        }

        private async Task<int> AgregarPersona()
        {
            Usuario = Persona;
            return await EjecutarAgregarUsuario();
        }


        private async Task<int> EjecutarAgregarUsuario()
        {
            await AgregarUsuarioComando.ExecuteAsync(null);
            return AgregarUsuarioComando.Execution.Result;
        }

        private bool PuedeAgregarUsuario() => !Usuario.HasErrors;

        private async Task<int> AgregarUsuario(IProgress<(double, string)> progreso)
        {
            async Task Reportar(string textoProgreso)
            {
                progreso.Report((0.0, textoProgreso));
                await Task.Delay(1000);
            }

            await Reportar("Comenzando ejecución...");
            await Reportar("Buscando duplicados...");
            await Reportar("Registrando usuario...");

            return 0;
        }
    }
}
