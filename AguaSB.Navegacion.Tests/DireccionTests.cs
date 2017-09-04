using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace AguaSB.Navegacion.Tests
{
    public class DireccionTests
    {
        [Test]
        public void DeberiaRegresar_ColaVacia_CuandoDireccionEstaVacia()
        {
            var direccion = "";

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(ColaNavegacion.Vacia, resultado);
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
            var esperado = new ColaNavegacion(direccion);

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void DeberiaTener_ElementosDeDireccion_CuandoDireccionTieneVariosValores()
        {
            string direccion = "Nodo/Subnodo/Subnodo2";
            var esperado = new ColaNavegacion("Nodo", "Subnodo", "Subnodo2");

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void DeberiaTener_ElementosDeDireccion_Y_Parametros_CuandoDireccionTieneParametros()
        {
            string direccion = "Nodo/Subnodo/Subnodo2?{ \"Nombre\" = \"GGzor\" }";
            var esperado = new ColaNavegacion("Nodo", "Subnodo", "Subnodo2", "{ \"Nombre\" = \"GGzor\" }");

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void Deberia_IgnorarCaracteresDeInterrogacionDespuesDelPrimero()
        {
            string direccion = "Nodo/Subnodo/Subnodo2?{ \"Nombre\" = \"GGzor?\" }";
            var esperado = new ColaNavegacion("Nodo", "Subnodo", "Subnodo2", "{ \"Nombre\" = \"GGzor?\" }");

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void Deberia_EliminarCaracteresBlancos_En_Direccion()
        {
            string direccion = "Nodo /    Subnodo     ?   {}";
            var esperado = new ColaNavegacion("Nodo", "Subnodo", "{}");

            var resultado = Direccion.Descomponer(direccion);

            Assert.AreEqual(esperado, resultado);
        }
    }
}
