using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    /// <summary>
    /// Representa la clase base de un objeto que puede ser navegado.
    /// </summary>
    public interface INodo
    {
        /// <summary>
        /// Entrar con la <see cref="ColaNavegacion"/> especificada.
        /// </summary>
        Task Entrar(object informacion);
    }
}
