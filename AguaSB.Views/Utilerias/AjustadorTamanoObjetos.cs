using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AguaSB.Views.Utilerias
{
    /// <summary>
    /// Calcula el ancho de un objeto que se ajusta a un contenedor del tamaño especificado.
    /// Su principal aplicación es en la clase <see cref="System.Windows.Controls.WrapPanel"/>
    /// </summary>
    public class AjustadorTamanoObjetos : INotifyPropertyChanged
    {
        #region Logica INotify
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        #endregion

        #region Campos y propiedades
        private double anchoContenedor;
        private double margen;
        private double anchoObjetoMinimo;
        private double anchoObjeto;

        public double AnchoContenedor
        {
            get { return anchoContenedor; }
            set
            {
                anchoContenedor = value;
                RecalcularTamanoObjeto();
            }
        }

        public double Margen
        {
            get { return margen; }
            set
            {
                margen = value;
                RecalcularTamanoObjeto();
            }
        }

        public double AnchoObjetoMinimo
        {
            get { return anchoObjetoMinimo; }
            set
            {
                anchoObjetoMinimo = value;
                RecalcularTamanoObjeto();
            }
        }

        public double AnchoObjeto
        {
            get { return anchoObjeto; }
            private set
            {
                anchoObjeto = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private void RecalcularTamanoObjeto()
        {
            var anchoRequerido = AnchoObjetoMinimo + (2 * Margen);

            if (AnchoContenedor < anchoRequerido)
            {
                // Sólo cabe un objeto, lo compactamos de acuerdo al contenedor;
                AnchoObjeto = AnchoContenedor - (2 * Margen);
            }
            else
            {
                var cantidadDeObjetos = (int)(AnchoContenedor / anchoRequerido);

                var espacioPorObjeto = AnchoContenedor / cantidadDeObjetos;

                AnchoObjeto = espacioPorObjeto - (2 * Margen);
            }
        }
    }
}
