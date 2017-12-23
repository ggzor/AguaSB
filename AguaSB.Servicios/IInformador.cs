namespace AguaSB.Servicios
{
    /// <summary>
    /// Representa un componente que informa al usuario de un tipo de datos dado. (Impresión, reportes, notificaciones por correo, etc.)
    /// </summary>
    /// <typeparam name="T">El tipo de datos del que se informa</typeparam>
    public interface IInformador<T>
    {
        void Informar(T informacion);
    }
}
