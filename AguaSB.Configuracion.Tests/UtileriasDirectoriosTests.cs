using System;
using NUnit.Framework;

namespace AguaSB.Configuracion.Tests
{
    [TestFixture]
    public class UtileriasDirectoriosTests
    {
        [TestCase("tmp", typeof(string), ".json", "tmp/String.json")]
        [TestCase("tmp/", typeof(string), ".json", "tmp/String.json")]
        [TestCase("tmp\\", typeof(string), ".json", "tmp/String.json")]
        [TestCase("", typeof(string), ".json", "String.json")]
        [TestCase("\\tmp/", typeof(string), ".json", "tmp/String.json")]
        public void Combinar_Deberia_CombinarCorrectamente(string subdirectorio, Type tipo, string extension, string resultado)
        {
            var resultadoCombinacion = UtileriasDirectorios.Combinar(subdirectorio, tipo.Name, extension);

            Assert.AreEqual(resultado, resultadoCombinacion);
        }
    }
}
