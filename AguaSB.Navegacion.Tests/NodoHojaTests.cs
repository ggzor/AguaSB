using System.Threading.Tasks;

using NUnit.Framework;
using Ploeh.AutoFixture;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class NodoHojaTests
    {

        private static readonly Fixture Cualquiera = new Fixture();

        private VerificadorFuncionLlamada Verificador;

        [SetUp]
        public void Inicializar()
        {
            Verificador = new VerificadorFuncionLlamada();
        }

        #region Funciones de ciclo de vida

        [Test]
        public async Task DeberiaLlamar_PrimeraEntrar_CuandoSeLlamaEntrar()
        {
            INodo nodo = new NodoHoja()
            {
                PrimeraEntrada = Verificador.Funcion
            };

            await ConfigurarYRealizarLlamada(nodo);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_Entrada_CuandoSeLlamaEntrar()
        {
            INodo nodo = new NodoHoja()
            {
                Entrada = _ => Verificador.Funcion()
            };

            await ConfigurarYRealizarLlamada(nodo);

            Assert.True(Verificador.Llamado);
        }


        [Test]
        public async Task DeberiaNoLlamar_PrimeraEntrada_CuandoSeLlamaEntrarPorSegundaVez()
        {
            INodo nodo = new NodoHoja()
            {
                PrimeraEntrada = Verificador.Funcion
            };

            await ConfigurarYRealizarLlamada(nodo);
            await ConfigurarYRealizarLlamada(nodo);

            Assert.AreEqual(1, Verificador.NumeroDeVeces);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoAlgunaDeLasFuncionesEsNula()
        {
            INodo nodo = new NodoHoja()
            {
                PrimeraEntrada = null,
                Entrada = null
            };
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Entrar(colaNavegacion);
        }
        #endregion

        #region Utilerias

        private static async Task ConfigurarYRealizarLlamada(INodo nodo)
        {
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Entrar(colaNavegacion);
        }

        #endregion

    }
}
