using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    /// <summary>
    /// Representa la clase base de un objeto que puede ser navegado.
    /// </summary>
    /// <typeparam name="T">El tipo de parametro al inicializar el nodo</typeparam>
    public interface INodo<T>
    {
        /// <summary>
        /// El navegador que ha accedido a esta página.
        /// </summary>
        Navegador Navegador { get; set; }

        /// <summary>
        /// Llamado cuando se va a inicializar este nodo.
        /// </summary>
        /// <param name="inicializar">El tipo de parametro con el que se inicializará este nodo</param>
        Task Inicializar(T inicializar);

        /// <summary>
        /// Entrar con la <see cref="ColaNavegacion"/> especificada.
        /// </summary>
        Task Entrar(ColaNavegacion informacion);

        /// <summary>
        /// Llamado cuando el navegador es finalizado.
        /// </summary>
        /// <returns></returns>
        Task Finalizar();
    }
}
