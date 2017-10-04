using NUnit.Framework;
using NSubstitute;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class NavegadorTests
    {
        [Test]
        public void DeberiaLlamar_NavegarConCadenas_CuandoSeUsaUnaCadena()
        {
            var navegador = Substitute.For<Navegador>();
            var direccion = "Nodo1/Subnodo1";
            var esperado = new[] { "Nodo1", "Subnodo1" };

            navegador.NavegarA(direccion);

            navegador.Received().NavegarA(esperado);
        }

        [Test]
        public void DeberiaLlamar_NavegarConColaNavegacion_CuandoSeUsanVariosParametros()
        {
            var navegador = Substitute.For<Navegador>();
            var direccion = new[] { "Nodo1", "Subnodo1" };
            var esperado = new ColaNavegacion("Nodo1", "Subnodo1");

            navegador.NavegarA(direccion);

            navegador.Received().NavegarA(esperado);
        }
    }
}
