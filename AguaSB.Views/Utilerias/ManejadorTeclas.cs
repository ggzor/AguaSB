using System;
using System.Windows;
using System.Windows.Input;

namespace AguaSB.Views.Utilerias
{
    public class ManejadorTeclas
    {
        public IInputElement Elemento { get; }

        public Action<Key> Manejador { get; }

        public Action<Key> ManejadorCtrl { get; }

        public ManejadorTeclas(IInputElement elemento, Action<Key> manejador, Action<Key> manejadorCtrl = null)
        {
            Elemento = elemento ?? throw new ArgumentNullException(nameof(elemento));

            Manejador = manejador ?? throw new ArgumentNullException(nameof(manejador));
            ManejadorCtrl = manejadorCtrl;

            elemento.PreviewKeyDown += ManejarTeclas;
        }

        private void ManejarTeclas(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
                ManejadorCtrl?.Invoke(e.Key);
            else
                Manejador(e.Key);
        }
    }
}