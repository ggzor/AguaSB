using NUnit.Framework;

namespace AguaSB.Nucleo.Utilerias.Tests
{
    [TestFixture]
    public class CantidadALetraTests
    {
        #region 0 - 9
        [TestCase(0, "Cero pesos")]
        [TestCase(1, "Un peso")]
        [TestCase(2, "Dos pesos")]
        [TestCase(3, "Tres pesos")]
        [TestCase(4, "Cuatro pesos")]
        [TestCase(5, "Cinco pesos")]
        [TestCase(6, "Seis pesos")]
        [TestCase(7, "Siete pesos")]
        [TestCase(8, "Ocho pesos")]
        [TestCase(9, "Nueve pesos")]
        #endregion
        public void CuandoNumeroEs_Menor10_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 11 - 19
        [TestCase(11, "Once pesos")]
        [TestCase(12, "Doce pesos")]
        [TestCase(13, "Trece pesos")]
        [TestCase(14, "Catorce pesos")]
        [TestCase(15, "Quince pesos")]
        [TestCase(16, "Dieciséis pesos")]
        [TestCase(17, "Diecisiete pesos")]
        [TestCase(18, "Dieciocho pesos")]
        [TestCase(19, "Diecinueve pesos")]
        #endregion
        public void CuandoNumeroEs_Menor20_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 10 - 90
        [TestCase(10, "Diez pesos")]
        [TestCase(20, "Veinte pesos")]
        [TestCase(30, "Treinta pesos")]
        [TestCase(40, "Cuarenta pesos")]
        [TestCase(50, "Cincuenta pesos")]
        [TestCase(60, "Sesenta pesos")]
        [TestCase(70, "Setenta pesos")]
        [TestCase(80, "Ochenta pesos")]
        [TestCase(90, "Noventa pesos")]
        #endregion
        public void CuandoNumeroEs_UnaDecena_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 20 - 99
        [TestCase(20, "Veinte pesos")]
        [TestCase(25, "Veinte y cinco pesos")]
        [TestCase(36, "Treinta y seis pesos")]
        [TestCase(49, "Cuarenta y nueve pesos")]
        [TestCase(60, "Sesenta pesos")]
        [TestCase(64, "Sesenta y cuatro pesos")]
        [TestCase(81, "Ochenta y un pesos")]
        [TestCase(99, "Noventa y nueve pesos")]
        #endregion
        public void CuandoNumeroEs_Mayor20_Menor100_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 100 - 900
        [TestCase(100, "Cien pesos")]
        [TestCase(200, "Doscientos pesos")]
        [TestCase(300, "Trescientos pesos")]
        [TestCase(400, "Cuatrocientos pesos")]
        [TestCase(500, "Quinientos pesos")]
        [TestCase(600, "Seiscientos pesos")]
        [TestCase(700, "Setecientos pesos")]
        [TestCase(800, "Ochocientos pesos")]
        [TestCase(900, "Novecientos pesos")]
        #endregion
        public void CuandoNumeroEs_UnaCentena_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 101 - 999
        [TestCase(101, "Ciento un pesos")]
        [TestCase(110, "Ciento diez pesos")]
        [TestCase(111, "Ciento once pesos")]
        [TestCase(121, "Ciento veinte y un pesos")]
        [TestCase(120, "Ciento veinte pesos")]
        [TestCase(144, "Ciento cuarenta y cuatro pesos")]
        [TestCase(169, "Ciento sesenta y nueve pesos")]
        [TestCase(240, "Doscientos cuarenta pesos")]
        [TestCase(256, "Doscientos cincuenta y seis pesos")]
        [TestCase(512, "Quinientos doce pesos")]
        [TestCase(666, "Seiscientos sesenta y seis pesos")]
        [TestCase(720, "Setecientos veinte pesos")]
        [TestCase(999, "Novecientos noventa y nueve pesos")]
        #endregion
        public void CuandoNumeroEs_Mayor100_Menor1000_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 1000 - 999,999
        [TestCase(1_000, "Un mil pesos")]
        [TestCase(1_111, "Un mil ciento once pesos")]
        [TestCase(1_500, "Un mil quinientos pesos")]
        [TestCase(2_000, "Dos mil pesos")]
        [TestCase(2_500, "Dos mil quinientos pesos")]
        [TestCase(5_100, "Cinco mil cien pesos")]
        [TestCase(10_000, "Diez mil pesos")]
        [TestCase(15_000, "Quince mil pesos")]
        [TestCase(100_000, "Cien mil pesos")]
        [TestCase(666_666, "Seiscientos sesenta y seis mil seiscientos sesenta y seis pesos")]
        [TestCase(999_999, "Novecientos noventa y nueve mil novecientos noventa y nueve pesos")]
        #endregion
        public void CuandoNumeroEs_Mayor1000_MenorMillon_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        #region 1,000,000 - 999,999,999
        [TestCase(1_000_000, "Un millón de pesos")]
        [TestCase(1_234_567, "Un millón doscientos treinta y cuatro mil quinientos sesenta y siete pesos")]
        [TestCase(611_510_000, "Seiscientos once millones quinientos diez mil pesos")]
        [TestCase(900_000_000, "Novecientos millones de pesos")]
        [TestCase(999_999_999, "Novecientos noventa y nueve millones novecientos noventa y nueve mil novecientos noventa y nueve pesos")]
        #endregion
        public void CuandoNumeroEs_MayorMillon_Menor1000Millones_ResultadoEs(int numero, string resultado) =>
            RevisarResultadoEsIgualAConversion(numero, resultado);

        private void RevisarResultadoEsIgualAConversion(int numero, string resultado) =>
            Assert.AreEqual(resultado, CantidadALetra.Convertir(numero));
    }
}
