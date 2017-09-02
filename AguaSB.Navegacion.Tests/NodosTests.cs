using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Navegacion.Tests
{
    [TestFixture]
    public class NodosTests
    {

        private static readonly Fixture Cualquiera = new Fixture();

        [Test]
        public async Task DeberiaLlamar_Inicializacion_CuandoSeLlamaEntrar()
        {
            var llamado = false;
            var nodo = new NodoHoja()
            {
                Inicializacion = () =>
                {
                    llamado = true;
                    return Task.CompletedTask;
                }
            };
            var colaNavegacion = Cualquiera.Create<ColaNavegacion>();

            await nodo.Entrar(colaNavegacion);

            Assert.True(llamado);
        }

    }
}
