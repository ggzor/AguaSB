using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using AguaSB.Nucleo;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class ContactosView : UserControl
    {
        public ContactosView()
        {
            InitializeComponent();
            Reestablecer();
        }

        public void Reestablecer() => Contactos = new List<Contacto>();

        public IEnumerable<TipoContacto> TiposContacto
        {
            get { return (IEnumerable<TipoContacto>)GetValue(TiposContactoProperty); }
            set { SetValue(TiposContactoProperty, value); }
        }

        public IList<Contacto> Contactos
        {
            get { return (IList<Contacto>)GetValue(ContactosProperty); }
            set { SetValue(ContactosProperty, value); }
        }

        public static readonly DependencyProperty TiposContactoProperty =
            DependencyProperty.Register(nameof(TiposContacto), typeof(IEnumerable<TipoContacto>), typeof(ContactosView), new PropertyMetadata(Enumerable.Empty<TipoContacto>()));

        public static readonly DependencyProperty ContactosProperty =
            DependencyProperty.Register(nameof(Contactos), typeof(IList<Contacto>), typeof(ContactosView), new PropertyMetadata(null));
    }
}
