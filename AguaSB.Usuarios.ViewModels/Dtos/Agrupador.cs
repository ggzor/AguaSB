using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Utilerias;
using AguaSB.Utilerias.Solicitudes;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public interface IAgrupador
    {
        string Nombre { get; }

        Propiedad Propiedad { get; }

        IEnumerable<Grupo> Agrupar(IEnumerable<ResultadoUsuario> elementos);
    }

    public class Agrupador<T> : Notificante, IAgrupador
    {
        public string Nombre { get; set; }

        public Propiedad Propiedad { get; set; }

        public Func<ResultadoUsuario, T> SelectorClave { get; set; }

        public Func<T, string> SelectorNombre { get; set; }

        public IComparer<T> Comparador { get; set; }

        public IEnumerable<Grupo> Agrupar(IEnumerable<ResultadoUsuario> elementos) =>
            elementos.GroupBy(SelectorClave)
            .OrderBy(g => g.Key, Comparador)
            .Select(g => new Grupo { Nombre = SelectorNombre(g.Key), Valores = g.ToList() })
            .ToList();

        public override string ToString() => Nombre;
    }
}
