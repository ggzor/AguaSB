using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public sealed class Grupo
    {
        public string Nombre { get; set; }

        public IEnumerable<ResultadoUsuario> Valores { get; set; }

        public override string ToString() => $"{Nombre} ({Valores.Count()})";
    }
}
