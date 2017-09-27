using System;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;

using AguaSB.Nucleo;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel
    {
        #region Campos

        private Persona persona;
        private Negocio negocio;

        #endregion

        #region Propiedades

        private Usuario Usuario { get; set; }

        public Persona Persona
        {
            get { return persona; }
            set { SetProperty(ref persona, value); }
        }

        public Negocio Negocio
        {
            get { return negocio; }
            set { SetProperty(ref negocio, value); }
        }

        #endregion

        #region Comandos

        public DelegateCommand ReestablecerPersonaComando { get; }
        public DelegateCommand ReestablecerNegocioComando { get; }

        public AsyncDelegateCommand<int> AgregarPersonaComando { get; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; }

        #endregion

        public Agregar()
        {
            var telefono = new TipoContacto() { Nombre = "Teléfono", ExpresionRegular = @"\A[0-9 ]*\z" };

            ReestablecerPersonaComando = new DelegateCommand(() =>
            {
                Persona = new Persona();
                Persona.Contactos.Add(new Contacto() { TipoContacto = telefono });
            });

            ReestablecerNegocioComando = new DelegateCommand(() =>
            {
                Negocio = new Negocio()
                {
                    Representante = new Persona()
                };

                Negocio.Contactos.Add(new Contacto() { TipoContacto = telefono });
                Negocio.Representante.Contactos.Add(new Contacto() { TipoContacto = telefono });
            });

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, PuedeAgregarPersona);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, PuedeAgregarNegocio);
        }

        private bool PuedeAgregarNegocio() => !Negocio.HasErrors;

        private async Task<int> AgregarNegocio(IProgress<(double, string)> progreso)
        {
            Usuario = Negocio;
            return await AgregarUsuario(progreso);
        }

        private bool PuedeAgregarPersona() => !Persona.HasErrors;

        private async Task<int> AgregarPersona(IProgress<(double, string)> progreso)
        {
            Usuario = Persona;
            return await AgregarUsuario(progreso);
        }

        private async Task<int> AgregarUsuario(IProgress<(double, string)> progreso = null)
        {
            async Task Reportar(string textoProgreso)
            {
                progreso?.Report((0.0, textoProgreso));
                await Task.Delay(5000);
            }

            await Reportar("Comenzando ejecución...");
            await Reportar("Buscando duplicados...");
            await Reportar("Registrando usuario...");

            return 0;
        }
    }
}
