using OfficeOpenXml;

using AguaSB.Legado.Nucleo;
using AguaSB.Servicios.Office;

namespace AguaSB.Legado
{
    public static class LecturaUsuario
    {
        public static Usuario Leer(ExcelWorksheet hoja, int fila)
        {
            var filaUsuario = new EditorFilaExcel(hoja, fila);

            return new Usuario
            {

                Numero = filaUsuario.Obtener<int>(2),
                Contrato = filaUsuario.Obtener<string>(3),
                Nombre = filaUsuario.Obtener<string>(6),
                Seccion = filaUsuario.Obtener<string>(7),
                Direccion = filaUsuario.Obtener<string>(8),
                TipoContrato = filaUsuario.Obtener<string>(11),
            };
        }
    }
}
