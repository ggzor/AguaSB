using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    /// <summary>
    /// Representa la clase base de un objeto que puede ser navegado.
    /// </summary>
    public interface INodo
    {
        /// <summary>
        /// El navegador que ha accedido a este nodo.
        /// </summary>
        Navegador Navegador { get; set; }

        /// <summary>
        /// Entrar con la <see cref="ColaNavegacion"/> especificada.
        /// </summary>
        Task Entrar(ColaNavegacion informacion);
    }
}
