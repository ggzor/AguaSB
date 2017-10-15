using System.Collections.Generic;

namespace AguaSB.Inicializadores
{
    /// <summary>
    /// Asegura que todos los inicializadores sean ejecutados
    /// </summary>
    public class Inicializador : IInicializador
    {
        public Inicializador(IEnumerable<IInicializador> inicializadores) { }
    }
}
