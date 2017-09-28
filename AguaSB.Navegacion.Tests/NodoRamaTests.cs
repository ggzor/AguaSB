using System;
using System.Collections.Generic;
using System.Linq;
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

        public static readonly IReadOnlyDictionary<string, INodo<string>> Subnodos = new Dictionary<string, INodo<string>>()
        {
            [NodoExistente] = new NodoHoja<string>(),
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
            var nodo = new NodoRama<string>(Subnodos)
            {
                SeleccionSubnodo = _ => Verificador.Funcion()
            };
            var colaNavegacion = new ColaNavegacion(nombreDeNodo);

            await nodo.Entrar(colaNavegacion);

            Assert.AreEqual(resultado, Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_EntradaSinArgumentos_CuandoSeLlamaEntrar_Con_ColaSinArgumentos()
        {
            var nodo = new NodoRama<string>(Subnodos)
            {
                EntradaSinArgumentos = Verificador.Funcion
            };

            await nodo.Entrar(ColaSinArgumentos);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_EntradaSinArgumentos_CuandoSeLlamaEntrar_Y_ElArgumentoEnColaNavegacionEsUnSubnodoInexistente()
        {
            var nodo = new NodoRama<string>(Subnodos)
            {
                EntradaSinArgumentos = Verificador.Funcion
            };
            var colaNavegacion = new ColaNavegacion(NodoInexistente);

            await nodo.Entrar(colaNavegacion);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoSeLlamaEntrar_Con_ColaSubnodoExistente_Y_SeleccionSubnodoEsNulo()
        {
            var nodo = new NodoRama<string>(Subnodos)
            {
                SeleccionSubnodo = null
            };

            await nodo.Entrar(ColaConSubnodoValido);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoSeLlamaEntrar_Con_ColaSinArgumentos_Y_EntradaSinArgumentosEsNulo()
        {
            var nodo = new NodoRama<string>(Subnodos)
            {
                EntradaSinArgumentos = null
            };

            await nodo.Entrar(ColaSinArgumentos);
        }

        #region Interacción con subnodos

        [Test]
        public async Task DeberiaEstablecer_NavegadorEnSubnodo_CuandoSeLlamaEntrar_Con_SubnodoExistente()
        {
            var navegador = Substitute.For<Navegador>();
            var nodo = new NodoRama<string>(Subnodos)
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
            var subnodo = Substitute.For<INodo<string>>();
            var subnodos = new Dictionary<string, INodo<string>>()
            {
                [NodoExistente] = subnodo
            };
            var nodo = new NodoRama<string>(subnodos);

            await nodo.Entrar(ColaConSubnodoValido);

            await subnodo.ReceivedWithAnyArgs().Entrar(null);
        }

        [Test]
        public async Task DeberiaLlamar_FinalizarEnCadaSubnodo_CuandoSeLlamaFinalizar()
        {
            var conteoSubnodos = Aleatorio.Next(1, 10);
            var subnodos = Enumerable.Range(0, conteoSubnodos)
                .Select(_ => (Cualquiera.Create<string>(), Substitute.For<INodo<string>>()))
                .ToDictionary(i => i.Item1, i => i.Item2);
            var nodo = new NodoRama<string>(subnodos);

            await nodo.Finalizar();

            foreach (var subnodo in subnodos)
                await subnodo.Value.ReceivedWithAnyArgs().Finalizar();
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoNoTieneNodosYSeLlamaFinalizar()
        {
            var nodo = new NodoRama<string>(new Dictionary<string, INodo<string>>());

            await nodo.Finalizar();
        }

        [Test]
        public async Task DeberiaLlamar_FinalizarEnNodoRama_DespuesDeNodoHijo()
        {
            var verificadorSubnodo = new VerificadorFuncionLlamada();
            var subnodo = new NodoHoja<string>()
            {
                Finalizacion = async () =>
                {
                    await Task.Delay(200);
                    await verificadorSubnodo.Funcion();
                }
            };
            var subnodos = new Dictionary<string, INodo<string>>()
            {
                [NodoExistente] = subnodo
            };
            var nodo = new NodoRama<string>(subnodos)
            {
                Finalizacion = Verificador.Funcion
            };

            await nodo.Finalizar();

            Assert.LessOrEqual(verificadorSubnodo.Timestamp, Verificador.Timestamp);
        }
        #endregion

    }
}
