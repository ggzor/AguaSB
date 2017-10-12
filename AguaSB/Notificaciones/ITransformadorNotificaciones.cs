namespace AguaSB.Notificaciones
{
    public interface ITransformadorNotificaciones
    {
        NotificacionView Transformar(Notificacion notificacion);
    }
}
