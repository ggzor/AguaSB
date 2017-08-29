using System;
using System.IO;

using AguaSB.Extensiones.Tests.LibreriaBase;

namespace AguaSB.Extensiones.Tests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var direccion = "AguaSB.Extensiones.Tests.Cliente.dll";
            var archivo = new FileInfo(direccion);
            Console.WriteLine($"Se tratara de cargar {archivo}");

            var clase1Objetos = CargadorExtensiones.Cargar<Clase1>(archivo);
            Console.WriteLine("Objetos clase 1:");
            foreach (var clase1Objeto in clase1Objetos)
            {
                Console.WriteLine($"Yo soy un {clase1Objeto.GetType().Name}");
                Console.WriteLine("Intentaremos hacer algo:");
                clase1Objeto.LlamarHacerAlgo();
            }

            Console.WriteLine("Objetos iinterfaz");
            var iinterfaz1Objetos = CargadorExtensiones.Cargar<IInterfaz1>(archivo);

            foreach (var iinterfaz1Objeto in iinterfaz1Objetos)
            {
                Console.WriteLine($"Yo soy un {iinterfaz1Objeto.GetType().Name}");
                Console.WriteLine("Diré algo:");
                iinterfaz1Objeto.HacerAlgo();
            }

            Console.ReadLine();
        }
    }
}
