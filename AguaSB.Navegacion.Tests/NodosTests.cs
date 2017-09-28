﻿using System.Threading.Tasks;

using NUnit.Framework;
using Ploeh.AutoFixture;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class NodosTests
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
        public async Task DeberiaLlamar_Inicializacion_CuandoSeLlamaEntrar()
        {
            INodo<string> nodo = new NodoHoja<string>()
            {
                Inicializacion = _ => Verificador.Funcion()
            };

            await ConfigurarYRealizarLlamada(nodo);

            Assert.True(Verificador.Llamado);
        }

        [Test]
        public async Task DeberiaLlamar_Entrada_CuandoSeLlamaEntrar()
        {
            INodo<string> nodo = new NodoHoja<string>()
            {
                Entrada = _ => Verificador.Funcion()
            };

            await ConfigurarYRealizarLlamada(nodo);

            Assert.True(Verificador.Llamado);
        }


        [Test]
        public async Task DeberiaNoLlamar_Inicializacion_CuandoSeLlamaEntrarPorSegundaVez()
        {
            INodo<string> nodo = new NodoHoja<string>()
            {
                Inicializacion = _ => Verificador.Funcion()
            };

            await ConfigurarYRealizarLlamada(nodo);
            await ConfigurarYRealizarLlamada(nodo);

            Assert.AreEqual(1, Verificador.NumeroDeVeces);
        }

        [Test]
        public async Task DeberiaNoLanzarExcepcion_CuandoAlgunaDeLasFuncionesEsNula()
        {
            INodo<string> nodo = new NodoHoja<string>()
            {
                Inicializacion = null,
                Entrada = null,
                Finalizacion = null
            };
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Entrar(colaNavegacion);
            await nodo.Finalizar();
        }

        [Test]
        public async Task DeberiaLlamar_Finalizacion_CuandoSeLlamaFinalizar()
        {
            INodo<string> nodo = new NodoHoja<string>()
            {
                Finalizacion = Verificador.Funcion
            };
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Finalizar();

            Assert.True(Verificador.Llamado);
        }

        #endregion

        #region Utilerias

        private static async Task ConfigurarYRealizarLlamada(INodo<string> nodo)
        {
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Entrar(colaNavegacion);
        }

        #endregion

    }
}
