using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo.Pagos.Tests
{
    [TestFixture]
    public class DetallesPagoTests
    {
        private static Fixture Cualquiera = new Fixture();

        private static readonly TipoContrato TipoContrato = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 0.5m, Nombre = "Básico" };

        [Test]
        public void DeberiaRegresarUnSoloDetalle_CuandoLaTarifaNoCambiaEnElPeriodoDeUnMismoAño()
        {
            var (_, montoTarifa) = RegistrarTarifa(2016, 01);
            var desde = Mes(2017, 01);
            var hasta = Mes(2017, 12);

            var detalles = DetallesPago.Obtener(desde, hasta, TipoContrato, Tarifas).ToArray();

            var esperado = new[]
            {
                new MesesPago(desde, hasta, montoTarifa * TipoContrato.Multiplicador)
            };
            CollectionAssert.AreEqual(esperado, detalles);
        }

        [Test]
        public void DeberiaRegresarDosDetallesYUnCambioDeTarifa_CuandoLaTarifaCambiaEnElPeriodoDeUnMismoAño()
        {
            var (_, montoPrimeraTarifa) = RegistrarTarifa(2016, 12);
            var (inicioNuevaTarifa, montoNuevaTarifa) = RegistrarTarifa(2017, 07);
            var desde = Mes(2017, 01);
            var hasta = Mes(2017, 12);

            var detalles = DetallesPago.Obtener(desde, hasta, TipoContrato, Tarifas).ToArray();

            var esperado = new IDetallePago[]
            {
                new MesesPago(desde, inicioNuevaTarifa.AddMonths(-1), montoPrimeraTarifa * TipoContrato.Multiplicador),
                new CambioTarifa { Mes = inicioNuevaTarifa, Anterior = montoPrimeraTarifa * TipoContrato.Multiplicador, Nueva = montoNuevaTarifa * TipoContrato.Multiplicador },
                new MesesPago(inicioNuevaTarifa, hasta, montoNuevaTarifa * TipoContrato.Multiplicador)
            };
            CollectionAssert.AreEqual(esperado, detalles);
        }

        [Test]
        public void DeberiaRetornarDosDetallesDeDistintosAños_CuandoLaTarifaNoCambia_Y_ElPeriodoEstaEnDistintosAños()
        {
            var desde = Mes(2016, 06);
            var hasta = Mes(2017, 06);
            var (_, montoTarifa) = RegistrarTarifa(2016, 01);

            var detalles = DetallesPago.Obtener(desde, hasta, TipoContrato, Tarifas).ToArray();

            var esperado = new IDetallePago[]
            {
                new MesesPago(desde, Mes(2016, 12), montoTarifa * TipoContrato.Multiplicador),
                new MesesPago(Mes(2017, 01), hasta, montoTarifa * TipoContrato.Multiplicador)
            };
            CollectionAssert.AreEqual(esperado, detalles);
        }

        [Test]
        public void DeberiaRetornarUnDetallePorAñoYPorTarifa_CuandoLaTarifaCambia_Y_ElPeriodoEstaEnDistintosAños()
        {
            var desde = Mes(2016, 01);
            var hasta = Mes(2017, 12);
            var (_, montoPrimeraTarifa) = RegistrarTarifa(2016, 01);
            var (inicioSegundaTarifa, montoSegundaTarifa) = RegistrarTarifa(2016, 08);

            var detalles = DetallesPago.Obtener(desde, hasta, TipoContrato, Tarifas).ToArray();

            var esperado = new IDetallePago[]
            {
                new MesesPago(desde, inicioSegundaTarifa.AddMonths(-1), montoPrimeraTarifa * TipoContrato.Multiplicador),
                new CambioTarifa { Anterior = montoPrimeraTarifa * TipoContrato.Multiplicador, Nueva = montoSegundaTarifa * TipoContrato.Multiplicador, Mes = inicioSegundaTarifa },
                new MesesPago(inicioSegundaTarifa, Mes(2016, 12), montoSegundaTarifa * TipoContrato.Multiplicador),
                new MesesPago(Mes(2017, 01), hasta, montoSegundaTarifa * TipoContrato.Multiplicador)
            };
            CollectionAssert.AreEqual(esperado, detalles);
        }

        [SetUp]
        public void Inicializar()
        {
            tarifas = new List<Tarifa>();
        }

        private List<Tarifa> tarifas;
        private Tarifa[] Tarifas => tarifas.ToArray();

        private (DateTime Inicio, decimal Monto) RegistrarTarifa(int año, int mes)
        {
            var tarifa = new Tarifa
            {
                Inicio = Mes(año, mes),
                Monto = Cualquiera.Create<int>()
            };

            tarifas.Add(tarifa);

            return (tarifa.Inicio, tarifa.Monto);
        }

        private DateTime Mes(int año, int mes) => new DateTime(año, mes, 01);
    }
}