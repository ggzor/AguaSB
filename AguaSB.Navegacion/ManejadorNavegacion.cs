using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class ManejadorNavegacion<T>
    {
        public Func<T, object, Task> Navegar { get; set; }

        public Func<string, Task> EnDireccionNoEncontrada { get; set; }
    }
}
