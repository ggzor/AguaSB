using AguaSB.Datos;
using AguaSB.Nucleo;
using Mehdime.Entity;
using System;
using System.Linq;

namespace AguaSB.Inicializadores
{
    public class AgregarTiposContacto : IInicializador
    {
        public AgregarTiposContacto(IDbContextScopeFactory ambito, IRepositorio<TipoContacto> tiposContactoRepo)
        {
            Console.WriteLine("Registrando tipos de contacto...");
            using (var baseDeDatos = ambito.Create())
            {
                if (tiposContactoRepo.Datos.Count() > 0)
                {
                    Console.WriteLine("No se requirió agregar algún tipo de contacto.");
                    return;
                }

                var telefono = new TipoContacto
                {
                    Nombre = "Teléfono",
                    ExpresionRegular = @"^( *\d){10} *$"
                };

                var email = new TipoContacto
                {
                    Nombre = "Email",
                    ExpresionRegular = ".*"
                };

                foreach (var tipoContacto in new[] { telefono, email })
                    tiposContactoRepo.Agregar(tipoContacto);

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");
        }
    }
}
