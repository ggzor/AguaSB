using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

using AguaSB.Utilerias;

namespace AguaSB.ViewModels
{
    public class VerificadorPropiedades
    {
        public INotifyPropertyChanged Padre { get; }

        public Func<IEnumerable<INotifyDataErrorInfo>> PropiedadesError { get; }

        public Func<IEnumerable<INotifyPropertyChanged>> Propiedades { get; }

        public Func<IEnumerable<ICommand>> Comandos { get; }

        public VerificadorPropiedades(INotifyPropertyChanged padre,
            Func<IEnumerable<INotifyDataErrorInfo>> propiedadesError,
            Func<IEnumerable<INotifyPropertyChanged>> propiedades,
            Func<IEnumerable<ICommand>> comandos)
        {
            Padre = padre ?? throw new ArgumentNullException(nameof(padre));
            PropiedadesError = propiedadesError ?? throw new ArgumentNullException(nameof(propiedades));
            Propiedades = propiedades ?? throw new ArgumentNullException(nameof(propiedades));
            Comandos = comandos ?? throw new ArgumentNullException(nameof(comandos));

            Padre.ToObservableProperties()
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher()
                .Subscribe(RegistrarObservadoresDeCambios);
            RegistrarObservadoresDeCambios();
        }

        public void RegistrarObservadoresDeCambios() =>
            RegistrarObservadoresDeCambios((this, new PropertyChangedEventArgs("None")));

        private IDisposable ObservadorDePropiedades;

        private void RegistrarObservadoresDeCambios((object Source, PropertyChangedEventArgs PropiedadCambiada) parametro)
        {
            ObservadorDePropiedades?.Dispose();

            var observables = from obj in Propiedades()
                              let obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => obj.PropertyChanged += h,
                                  h => obj.PropertyChanged -= h)
                              select obs.Select(_ => Unit.Default);

            var observablesError = from obj in PropiedadesError()
                                   let obs = Observable.FromEventPattern<DataErrorsChangedEventArgs>(
                                       h => obj.ErrorsChanged += h,
                                       h => obj.ErrorsChanged -= h)
                                   select obs.Select(_ => Unit.Default);

            var unaVez = Observable.Return(Unit.Default);

            ObservadorDePropiedades = observablesError.Concat(observables)
                .Merge()
                .Merge(unaVez)
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher()
                .Subscribe(_ => VerificarPuedeEjecutar());
        }

        public void VerificarPuedeEjecutar() => UtileriasComandos.VerificarPuedeEjecutarEn(Comandos());
    }
}
