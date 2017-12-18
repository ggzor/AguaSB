using System;
using AguaSB.Utilerias;

namespace AguaSB.ViewModels
{
    public sealed class ControladorCubierta : Notificante
    {
        private bool mostrarCubierta;
        private string textoCubierta;

        public bool MostrarCubierta
        {
            get { return mostrarCubierta; }
            set { N.Set(ref mostrarCubierta, value); }
        }

        public string TextoCubierta
        {
            get { return textoCubierta; }
            set { N.Set(ref textoCubierta, value); }
        }

        public Cubierta Mostrar(string textoInicial = "") => new Cubierta(this, textoInicial);
    }

    public sealed class Cubierta : IDisposable
    {
        public ControladorCubierta Controlador { get; }

        public Cubierta(ControladorCubierta controlador, string textoInicial)
        {
            Controlador = controlador ?? throw new ArgumentNullException(nameof(controlador));

            Controlador.TextoCubierta = textoInicial;
            Controlador.MostrarCubierta = true;
        }

        public string Texto
        {
            get { return Controlador.TextoCubierta; }
            set { Controlador.TextoCubierta = value; }
        }

        public void Dispose()
        {
            Controlador.MostrarCubierta = false;
        }
    }
}
