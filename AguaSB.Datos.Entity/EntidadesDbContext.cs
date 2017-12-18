﻿using AguaSB.Datos.Entity.Migrations;
using AguaSB.Nucleo;
using System.Data.Entity;

namespace AguaSB.Datos.Entity
{
    public class EntidadesDbContext : DbContext
    {
        public EntidadesDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EntidadesDbContext, Configuration>());
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<Contacto> Contactos { get; set; }
        public virtual DbSet<TipoContacto> TiposContacto { get; set; }

        public virtual DbSet<Seccion> Secciones { get; set; }
        public virtual DbSet<Calle> Calles { get; set; }
        public virtual DbSet<Domicilio> Domicilios { get; set; }

        public virtual DbSet<Contrato> Contratos { get; set; }
        public virtual DbSet<TipoContrato> TiposContrato { get; set; }

        public virtual DbSet<Pago> Pagos { get; set; }

        public virtual DbSet<Tarifa> Tarifas { get; set; }
    }
}
