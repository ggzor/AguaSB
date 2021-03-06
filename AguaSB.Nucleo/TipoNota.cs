﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("TiposNota")]
    public class TipoNota : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        private string nombre;
        public virtual ICollection<Nota> Notas { get; set; }

        #region Propiedades
        [Required(ErrorMessage = Mensajes.Validacion.CampoRequerido)]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Nombre
        {
            get { return nombre; }
            set { N.Validate(ref nombre, value); }
        }
        #endregion

        public TipoNota()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            Notas = new List<Nota>();
        }

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        [NotMapped]
        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private readonly Lazy<Notificador> notificador;
        [NotMapped]
        protected Notificador N => notificador.Value;
        #endregion
    }
}
