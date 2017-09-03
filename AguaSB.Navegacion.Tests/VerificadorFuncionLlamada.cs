using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion.Tests
{
    public class VerificadorFuncionLlamada
    {
        public Func<Task> Funcion { get; }

        public bool Llamado { get; private set; }

        public DateTime Timestamp { get; private set; }

        public int NumeroDeVeces { get; private set; }

        public VerificadorFuncionLlamada()
        {
            Funcion = () =>
            {
                NumeroDeVeces++;
                Llamado = true;
                Timestamp = DateTime.Now;
                return Task.CompletedTask;
            };
        }
    }
}
