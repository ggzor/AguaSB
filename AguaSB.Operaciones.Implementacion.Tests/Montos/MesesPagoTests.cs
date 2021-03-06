﻿using System;

using NUnit.Framework;
using AutoFixture;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Nucleo.Pagos.Tests
{
    // TODO: Tests mal colocados
    [TestFixture]
    public class MesesPagoTests
    {
        private static readonly Fixture Cualquiera = new Fixture();

        [TestCase(2017, 01, 2017, 01, 1)]
        [TestCase(2017, 01, 2017, 12, 12)]
        [TestCase(2016, 07, 2017, 07, 13)]
        public void DeberiaSerCantidadMeses__CuandoMesesSon(int añoInicio, int mesInicio, int añoFin, int mesFin, int esperado)
        {
            var inicio = new DateTime(añoInicio, mesInicio, 01);
            var fin = new DateTime(añoFin, mesFin, 01);
            var mesPago = new MesesMonto(inicio, fin, Cualquiera.Create<decimal>());

            var resultado = mesPago.CantidadMeses;

            Assert.AreEqual(esperado, resultado);
        }
    }
}
