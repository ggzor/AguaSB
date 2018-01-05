using AguaSB.Utilerias;
using System;
using System.Threading.Tasks;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public sealed class Busqueda<T> : Notificante
    {
        private bool? buscando;
        private Exception error;
        private T resultado;

        public bool? Buscando
        {
            get { return buscando; }
            set { N.Set(ref buscando, value); }
        }

        public Exception Error
        {
            get { return error; }
            set { N.Set(ref error, value); N.Change(nameof(TieneError)); }
        }

        public bool TieneError => Error != null;

        public T Resultado
        {
            get { return resultado; }
            set { N.Set(ref resultado, value); }
        }

        public Busqueda(T @default = default)
        {
            Resultado = @default;
        }

        public async Task BuscarAsync(Func<T> buscador)
        {
            if (buscador == null)
                throw new ArgumentNullException(nameof(buscador));

            Buscando = true;

            try
            {
                Resultado = await Task.Run(buscador).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
            finally
            {
                Buscando = false;
            }
        }
    }

    public static class Busquedas
    {
        public static Busqueda<T> Nueva<T>(T @default = default) => new Busqueda<T>(@default);
    }
}
