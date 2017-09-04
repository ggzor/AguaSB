using NUnit.Framework;
using Ploeh.AutoFixture;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class ColaNavegacionTests
    {

        private static readonly Fixture Cualquiera = new Fixture();

        [Test]
        public void DeberiaRegresar_PrimerElementoCuandoSeLlamaSiguiente()
        {
            var primerElemento = Cualquiera.Create<string>();
            var cola = new ColaNavegacion(primerElemento, new object(), new object());

            var siguiente = cola.Siguiente<string>();

            Assert.AreEqual(primerElemento, siguiente);
        }

        [Test]
        public void DeberiaRegresar_SiguienteNull_CuandoPrimerElementoNoCoincideEnTipo()
        {
            var cola = new ColaNavegacion(new object());

            var siguiente = cola.Siguiente<string>();

            Assert.IsNull(siguiente);
        }

        [Test]
        public void DeberiaRegresar_VerdaderoTieneInformacion_CuandoColaNavegacionTieneAlgunElemento()
        {
            var cola = new ColaNavegacion(new object());

            var tieneInformacion = cola.TieneInformacion;

            Assert.True(tieneInformacion);
        }
    }
}
