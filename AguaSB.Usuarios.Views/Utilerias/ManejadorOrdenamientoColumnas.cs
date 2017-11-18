using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Views.Controles;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AguaSB.Usuarios.Views.Utilerias
{
    public class ManejadorOrdenamientoColumnas
    {
        private GridViewColumnHeader[] Encabezados { get; }

        public ManejadorOrdenamientoColumnas(ListView listado)
        {
            if (listado == null)
                throw new ArgumentNullException(nameof(listado));

            if (!(listado.View is GridView))
                throw new InvalidCastException("La instancia ListView proporcionada no tiene un GridView como propiedad View.");

            Encabezados = ((GridView)listado.View).Columns.Select(_ => _.Header).OfType<GridViewColumnHeader>().ToArray();
        }

        private IOrdenamiento ordenamientoActual;
        private GridViewColumnHeader columnaActual;

        public void SeleccionarOrdenamiento(IOrdenamiento ordenamientoNuevo)
        {
            if (ordenamientoNuevo != null)
            {
                if (ordenamientoActual == ordenamientoNuevo)
                    RemoverAdornos();
                else
                    RemoverOrdenamientoActual();

                EstablecerNuevoOrdenamiento(ordenamientoNuevo);
            }
            else
            {
                RemoverOrdenamientoActual();
            }
        }

        private SortAdorner adornadorActual;

        private void RemoverOrdenamientoActual()
        {
            if (ordenamientoActual != null)
                ordenamientoActual.Direccion = null;

            ordenamientoActual = null;

            RemoverAdornos();
        }

        private void RemoverAdornos()
        {
            if (adornadorActual != null && columnaActual != null)
                AdornerLayer.GetAdornerLayer(columnaActual).Remove(adornadorActual);

            adornadorActual = null;
            columnaActual = null;
        }

        private void EstablecerNuevoOrdenamiento(IOrdenamiento nuevoOrdenamiento)
        {
            if (nuevoOrdenamiento.Direccion is ListSortDirection direccion)
            {
                ordenamientoActual = nuevoOrdenamiento;
                columnaActual = Encabezados.Single(_ => _.Tag == nuevoOrdenamiento);

                adornadorActual = new SortAdorner(columnaActual, direccion);

                AdornerLayer.GetAdornerLayer(columnaActual).Add(adornadorActual);
            }
        }
    }
}
