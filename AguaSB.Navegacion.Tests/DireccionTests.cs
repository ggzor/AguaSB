using System;
using System.Linq;

using NUnit.Framework;

namespace AguaSB.Navegacion.Tests
{
    public class DireccionTests
    {
        [Test]
        public void DeberiaRegresar_Vacio_CuandoDireccionEstaVacia()
        {
            var direccion = "";

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(Enumerable.Empty<string>(), resultado);
        }

        [Test]
        public void DeberiaLanzarExcepcion_CuandoDireccionEsNula()
        {
            string direccion = null;

            Assert.Throws<ArgumentNullException>(() => Direccion.Descomponer(direccion));
        }

        [Test]
        public void DeberiaTener_UnSoloElementoResultado_CuandoDireccionSoloTieneUnValor()
        {
            string direccion = "Elemento";
            var esperado = new[] { direccion };

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void DeberiaTener_ElementosDeDireccion_CuandoDireccionTieneVariosValores()
        {
            string direccion = "Nodo/Subnodo/Subnodo2";
            var esperado = new[] { "Nodo", "Subnodo", "Subnodo2" };

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void DeberiaTener_ElementosDeDireccion_Y_Parametros_CuandoDireccionTieneParametros()
        {
            string direccion = "Nodo/Subnodo/Subnodo2?{ \"Nombre\" = \"GGzor\" }";
            var esperado = new[] { "Nodo", "Subnodo", "Subnodo2", "{ \"Nombre\" = \"GGzor\" }" };

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void Deberia_IgnorarCaracteresDeInterrogacionDespuesDelPrimero()
        {
            string direccion = "Nodo/Subnodo/Subnodo2?{ \"Nombre\" = \"GGzor?\" }";
            var esperado = new[] { "Nodo", "Subnodo", "Subnodo2", "{ \"Nombre\" = \"GGzor?\" }" };

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void Deberia_EliminarCaracteresBlancos_En_Direccion()
        {
            string direccion = "Nodo /    Subnodo     ?   {}";
            var esperado = new[] { "Nodo", "Subnodo", "{}" };

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }
    }
}
