using System;

using AguaSB.Extensiones.Tests.LibreriaBase;

namespace AguaSB.Extensiones.Tests.Cliente
{
    public class Cliente1 : Clase1
    {
        protected override void HacerAlgo()
        {
            Console.WriteLine("Ya que cliente2 dirá algo raro, yo no lo hare... soy cliente 1.");
        }
    }
}
