using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;
using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using AguaSB.Navegacion;
using AguaSB.Datos;
using AguaSB.Operaciones;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos

        // Serán inicializadas junto con los comandos.
        private Persona persona;
        private Negocio negocio;

        private bool puedeReestablecerPersona = true;
        private bool mostrarMensajeErrorPersona = true;

        private bool puedeReestablecerNegocio = true;
        private bool mostrarMensajeErrorNegocio = true;

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

        public bool MostrarMensajeErrorPersona
        {
            get { return mostrarMensajeErrorPersona; }
            set { SetProperty(ref mostrarMensajeErrorPersona, value); }
        }

        public bool MostrarMensajeErrorNegocio
        {
            get { return mostrarMensajeErrorNegocio; }
            set { SetProperty(ref mostrarMensajeErrorNegocio, value); }
        }

        public bool PuedeReestablecerPersona
        {
            get { return puedeReestablecerPersona; }
            set
            {
                puedeReestablecerPersona = value;
                ReestablecerPersonaComando.RaiseCanExecuteChanged();
            }
        }

        public bool PuedeReestablecerNegocio
        {
            get { return puedeReestablecerNegocio; }
            set
            {
                puedeReestablecerNegocio = value;
                ReestablecerNegocioComando.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Comandos

        public DelegateCommand ReestablecerPersonaComando { get; private set; }
        public DelegateCommand ReestablecerNegocioComando { get; private set; }

        public AsyncDelegateCommand<int> AgregarPersonaComando { get; private set; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; private set; }

        #endregion

        #region Dependencias
        public IRepositorio<Usuario> Usuarios { get; set; }
        #endregion

        public INodo Nodo { get; }

        public Agregar(IRepositorio<Usuario> usuarios)
        {
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));

            Nodo = new Nodo();

            ConfigurarComandos();

            new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[] {
                    Persona, Negocio, Negocio.Representante,
                }
                .Concat(Persona.Contactos)
                .Concat(Negocio.Contactos)
                .Concat(Negocio.Representante.Contactos),
                () => new[] { this },
                () => new[] { AgregarPersonaComando, AgregarNegocioComando });
        }

        private void ConfigurarComandos()
        {
            var telefono = new TipoContacto() { Nombre = "Teléfono", ExpresionRegular = @"\A[0-9 ]*\z" };

            ReestablecerPersonaComando = new DelegateCommand(() =>
            {
                var persona = new Persona();
                persona.Contactos.Add(new Contacto() { TipoContacto = telefono });

                Persona = persona;

                MostrarMensajeErrorPersona = false;
            }, () => PuedeReestablecerPersona);

            ReestablecerNegocioComando = new DelegateCommand(() =>
            {
                var negocio = new Negocio()
                {
                    Representante = new Persona()
                };

                negocio.Contactos.Add(new Contacto() { TipoContacto = telefono });
                negocio.Representante.Contactos.Add(new Contacto() { TipoContacto = telefono });

                Negocio = negocio;

                MostrarMensajeErrorNegocio = false;
            }, () => PuedeReestablecerNegocio);

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, PuedeAgregarPersona);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, PuedeAgregarNegocio);
        }

        private bool PuedeAgregarPersona() =>
            UtileriasErrores.NingunoTieneErrores(
                (Persona as INotifyDataErrorInfo)
                .Concat(Persona.Contactos)
                .ToArray())
            && !Persona.TieneCamposRequeridosVacios;

        private bool PuedeAgregarNegocio() =>
            UtileriasErrores.NingunoTieneErrores(
                new INotifyDataErrorInfo[] { Negocio, Negocio.Representante }
                .Concat(Negocio.Contactos)
                .Concat(Negocio.Representante.Contactos)
                .ToArray())
            && !Negocio.TieneCamposRequeridosVacios && !Negocio.Representante.TieneCamposRequeridosVacios;

        private async Task<int> AgregarPersona(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorPersona = true;
            return await AgregarUsuarioManejando(Persona, b => PuedeReestablecerPersona = b, ReestablecerPersonaComando, progreso);
        }

        private async Task<int> AgregarNegocio(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorNegocio = true;
            return await AgregarUsuarioManejando(Negocio, b => PuedeReestablecerNegocio = b, ReestablecerNegocioComando, progreso);
        }

        private async Task<int> AgregarUsuarioManejando(Usuario usuario, Action<bool> puedeReestablecer, ICommand reestablecer, IProgress<(double, string)> progreso)
        {
            foreach (var contacto in usuario.Contactos.ToArray())
                if (string.IsNullOrWhiteSpace(contacto?.Informacion))
                    usuario.Contactos.Remove(contacto);

            puedeReestablecer(false);
            Usuario = usuario;

            try
            {
                var resultado = await AgregarUsuario(progreso);

                puedeReestablecer(true);
                reestablecer.Execute(null);

                return resultado;
            }
            finally
            {
                puedeReestablecer(true);
            }
        }

        private async Task<int> AgregarUsuario(IProgress<(double, string)> progreso = null)
        {
            progreso.Report((0.0, "Buscando duplicados..."));

            if (await OperacionesUsuarios.BuscarDuplicadosAsync(Usuario, Usuarios) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

            progreso.Report((50.0, "Agregando usuario..."));

            var usuario = await Usuarios.Agregar(Usuario);

            progreso.Report((100.0, "Completado."));

            return usuario.Id;
        }
    }
}
