using System;
using System.Collections.Generic;

namespace AguaSB.Navegacion
{
    public class NavegadorDirectorio<T> : INavegador
    {
        public IReadOnlyDictionary<string, T> Directorio { get; }

        public Action<T, object> ManejadorNavegacion { get; }

        public Action<string> ManejadorDireccionInvalida { get; }

        public NavegadorDirectorio(IReadOnlyDictionary<string, T> directorio, Action<T, object> manejadorNavegacion, Action<string> manejadorDireccionInvalida)
        {
            Directorio = directorio ?? throw new ArgumentNullException(nameof(directorio));
            ManejadorNavegacion = manejadorNavegacion ?? throw new ArgumentNullException(nameof(manejadorNavegacion));
            ManejadorDireccionInvalida = manejadorDireccionInvalida ?? throw new ArgumentNullException(nameof(manejadorDireccionInvalida));
        }

        public void Navegar(string direccion, object parametro)
        {
            try
            {
                var objeto = Directorio[direccion];
                ManejadorNavegacion(objeto, parametro);
            }
            catch (KeyNotFoundException)
            {
                ManejadorDireccionInvalida(direccion);
            }
        }
    }
}
