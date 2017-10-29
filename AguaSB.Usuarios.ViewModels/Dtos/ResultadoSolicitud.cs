using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class ResultadoSolicitud
    {
        public IEnumerable<ResultadoUsuario> Resultados { get; set; }

        public long Conteo { get; set; }
    }
}
