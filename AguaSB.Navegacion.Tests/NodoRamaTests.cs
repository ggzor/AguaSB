using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;
using NSubstitute;
using Ploeh.AutoFixture;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class NodoRamaTests
    {
        private const string NodoExistente = "NodoExistente";
        private const string NodoInexistente = "NodoInexistente";

        public static readonly IReadOnlyDictionary<string, INodo> Subnodos = new Dictionary<string, INodo>()
        {
            [NodoExistente] = new NodoHoja(),
        };

        private static readonly Fixture Cualquiera = new Fixture();
        private static readonly Random Aleatorio = new Random();
        private VerificadorFuncionLlamada Verificador;
        private ColaNavegacion ColaSinArgumentos;
        private ColaNavegacion ColaConSubnodoValido;

        [SetUp]
        public void Inicializar()
        {
            Verificador = new VerificadorFuncionLlamada();
            ColaSinArgumentos = new ColaNavegacion();
            ColaConSubnodoValido = new ColaNavegacion(NodoExistente);
        }


        [TestCase(true, NodoExistente)]
        [TestCase(false, NodoInexistente)]
        public async Task Deberia__Llamar_SeleccionSubnodo_CuandoSeEntraConUnNombreDeNodo__(bool resultado, string nombreDeNodo)
        {
            var nodo = new NodoRama(Subnodos)
            {
                SeleccionSubnodo = _ => Verificador.Funcion()
            };
            var colaNavegacion = new ColaNavegacion(nombreDeNodo);

            await nodo.Entrar(colaNavegacion);

            Assert.AreEqual(resultado, Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_Entrada_CuandoSeLlamaEntrar_Con_ColaSinArgumentos()
        {
            var nodo = new NodoRama(Subnodos)
            {
                Entrada = _ => Verificador.Funcion()
            };

            await nodo.Entrar(ColaSinArgumentos);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_Entrada_CuandoSeLlamaEntrar_Y_ElArgumentoEnColaNavegacionEsUnSubnodoInexistente()
        {
            var nodo = new NodoRama(Subnodos)
            {
                Entrada = _ => Verificador.Funcion()
            };
            var colaNavegacion = new ColaNavegacion(NodoInexistente);

            await nodo.Entrar(colaNavegacion);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoSeLlamaEntrar_Con_ColaSubnodoExistente_Y_SeleccionSubnodoEsNulo()
        {
            var nodo = new NodoRama(Subnodos)
            {
                SeleccionSubnodo = null
            };

            await nodo.Entrar(ColaConSubnodoValido);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoSeLlamaEntrar_Con_ColaSinArgumentos_Y_EntradaSinArgumentosEsNulo()
        {
            var nodo = new NodoRama(Subnodos)
            {
                Entrada = null
            };

            await nodo.Entrar(ColaSinArgumentos);
        }

        #region Interacción con subnodos
        [Test]
        public async Task DeberiaEstablecer_NavegadorEnSubnodo_CuandoSeLlamaEntrar_Con_SubnodoExistente()
        {
            var navegador = Substitute.For<Navegador>();
            var nodo = new NodoRama(Subnodos)
            {
                Navegador = navegador
            };

            await nodo.Entrar(ColaConSubnodoValido);

            var navegadorSubnodo = Subnodos[NodoExistente].Navegador;
            Assert.AreEqual(navegador, navegadorSubnodo);
        }

        [Test]
        public async Task DeberiaLlamar_EntrarEnSubnodo_CuandoSeLlamaEntrar_Con_SubnodoExistente()
        {
            var subnodo = Substitute.For<INodo>();
            var subnodos = new Dictionary<string, INodo>()
            {
                [NodoExistente] = subnodo
            };
            var nodo = new NodoRama(subnodos);

            await nodo.Entrar(ColaConSubnodoValido);

            await subnodo.ReceivedWithAnyArgs().Entrar(null);
        }
        #endregion
    }
}
