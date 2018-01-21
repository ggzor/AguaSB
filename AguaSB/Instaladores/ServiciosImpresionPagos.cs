using AguaSB.Servicios;
using AguaSB.Servicios.Archivos;
using AguaSB.Servicios.Office;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.IO;
using System.Collections.Generic;
using AguaSB.Utilerias;
using AguaSB.Legado.Nucleo;
using AguaSB.Legado;
using AguaSB.Utilerias.IO;

namespace AguaSB.Instaladores
{
    public class ServiciosImpresionPagos : IWindsorInstaller
    {
        private static readonly string DirectorioAguaSB = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AguaSB");
        private static readonly string DirectorioPlantillas = Path.Combine(DirectorioAguaSB, "Plantillas");
        private static readonly string DirectorioRecibos = Path.Combine(DirectorioAguaSB, "Recibos");
        private static readonly string DirectorioConfiguracion = Path.Combine(DirectorioAguaSB, "Configuracion");

        private const string NombrePlantilla = "Recibo.docx";

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Directory.CreateDirectory(DirectorioPlantillas);
            Directory.CreateDirectory(DirectorioRecibos);
            Directory.CreateDirectory(DirectorioConfiguracion);

            var plantillaReciboPagos = Path.Combine(DirectorioPlantillas, NombrePlantilla);
            var extractorValoresPagos = new ExtractorValoresPagos();
            string rutaArchivoBaseDatos;

            try
            {
                var configuracion = Configuracion.Cargar<ConfiguracionLegado>(subdirectorio: DirectorioConfiguracion);

                if (string.IsNullOrWhiteSpace(configuracion.DirectorioArchivoExcel) || !File.Exists(configuracion.DirectorioArchivoExcel))
                    throw new Exception();
                else
                    rutaArchivoBaseDatos = configuracion.DirectorioArchivoExcel;
            }
            catch
            {
                Configuracion.Guardar(new ConfiguracionLegado(), subdirectorio: DirectorioConfiguracion);
                throw new Exception("Especifique la dirección del archivo de base de datos. Este debe existir.");
            }

            container.Register(
                Component.For<IImpresor>()
                .ImplementedBy<ImpresorWord>());

            container.Register(
                Component.For<IPlantilla<FileInfo, Pago>>()
                .ImplementedBy<RellenadorPlantillaWord<Pago>>()
                .DependsOn(new
                {
                    plantilla = new FileInfo(plantillaReciboPagos),
                    extractorValores = extractorValoresPagos,
                    generadorRutaArchivos = (Func<Pago, string>)extractorValoresPagos.GenerarRuta
                }));

            container.Register(
                Component.For<IInformador<Pago>>()
                .ImplementedBy<GeneradorRecibos<Pago>>());

            container.Register(
                Component.For<EjecutorSolicitudPago>()
                .DependsOn(new
                {
                    archivo = new FileInfo(rutaArchivoBaseDatos)
                }));

            container.Register(
                Component.For<IInformador<Nucleo.Pago>>()
                .ImplementedBy<TransformadorPago>());
        }

        private class ExtractorValoresPagos : IExtractorValores<Pago>
        {
            public string DirectorioPagosHoy { get; } = Path.Combine(DirectorioRecibos, Fecha.Ahora.ToString("yyyy-MM-dd"));

            public ExtractorValoresPagos()
            {
                Directory.CreateDirectory(DirectorioPagosHoy);
            }

            public IReadOnlyDictionary<string, object> Extraer(Pago pago) => new Dictionary<string, object>
            {
                ["%Id%"] = pago.Folio,
                ["%Usuario%"] = pago.Usuario.Numero,
                ["%Contrato%"] = pago.Usuario.Contrato,
                ["%Tipo%"] = pago.Usuario.TipoContrato,
                ["%Nombre%"] = pago.Usuario.Nombre,
                ["%Domicilio%"] = pago.Usuario.Direccion,
                ["%Seccion%"] = pago.Usuario.Seccion,
                ["%Cantidad%"] = pago.Cantidad.ToString("C"),
                ["%CantidadLetra%"] = pago.CantidadLetra.ToUpper(),
                ["%Meses%"] = pago.Meses,
                ["%Dia%"] = pago.FechaPago.ToString("dd"),
                ["%Mes%"] = pago.FechaPago.ToString("MMMM").ToUpper(),
                ["%Año%"] = pago.FechaPago.ToString("yyyy")
            };

            public string GenerarRuta(Pago pago) =>
                Path.Combine(DirectorioPagosHoy, GenerarRutaPago(pago) + ".docx");

            private string GenerarRutaPago(Pago pago)
            {
                return $"{pago.Folio} - {pago.Usuario.Nombre} - {pago.Usuario.Seccion}";
            }
        }
    }
}

