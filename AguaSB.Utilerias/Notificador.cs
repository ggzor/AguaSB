using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AguaSB.Utilerias
{
    public sealed class Notificador
    {
        private Action<object, PropertyChangedEventArgs> PropiedadCambiada;
        private Action<object, DataErrorsChangedEventArgs> ErroresCambiados;
        private object Objeto;

        public Notificador(object objeto, Action<object, PropertyChangedEventArgs> eventoPropiedadCambiada, Action<object, DataErrorsChangedEventArgs> eventoErroresCambiados)
        {
            Objeto = objeto;
            PropiedadCambiada = eventoPropiedadCambiada;
            ErroresCambiados = eventoErroresCambiados;
        }

        private Dictionary<string, List<string>> errores = new Dictionary<string, List<string>>();

        public bool TieneErrores => errores.Any();

        public IEnumerable Errores(string propiedad)
        {
            if (propiedad != null && errores.ContainsKey(propiedad))
                return errores[propiedad];
            else
                return null;
        }

        public void Change(string propiedad) => PropiedadCambiada?.Invoke(Objeto, new PropertyChangedEventArgs(propiedad));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", 
            Justification = "Permite que el patrón del Notify sea implementado con facilidad.")]
        public void Set<T>(ref T campo, T value, [CallerMemberName] string propiedad = null)
        {
            if (Equals(campo, value))
                return;

            campo = value;

            Change(propiedad);
        }

        public void Validate<T>(ref T campo, T value, [CallerMemberName]string propiedad = null)
        {
            Set(ref campo, value, propiedad);

            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(Objeto) { MemberName = propiedad };

            Validator.TryValidateProperty(value, contexto, resultados);

            if (resultados.Any())
                errores[propiedad] = resultados.Select(c => c.ErrorMessage).ToList();
            else
                errores.Remove(propiedad);

            ErroresCambiados?.Invoke(Objeto, new DataErrorsChangedEventArgs(propiedad));
        }
    }
}
