using System;

namespace AguaSB.Extensiones.Tests.LibreriaBase
{
    public abstract class Clase1
    {

        public void LlamarHacerAlgo()
        {
            Console.WriteLine("Ahora intentaremos llamar a hacer algo en la subclase...");
            HacerAlgo();
        }

        protected abstract void HacerAlgo();
    }
}
