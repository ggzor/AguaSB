using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace AguaSB.Utilerias
{
    public static class NotifyPropertyChangedEx
    {
        public static IObservable<(object Source, PropertyChangedEventArgs Args)> ToObservableProperties(this INotifyPropertyChanged notifier)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => notifier.PropertyChanged += handler,
                handler => notifier.PropertyChanged -= handler)
                .Select(eventPattern => (eventPattern.Sender, eventPattern.EventArgs));
        }
    }
}
