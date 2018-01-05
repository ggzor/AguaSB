using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace AguaSB.Utilerias
{
    public static class NotifyPropertyChangedEx
    {
        public static IObservable<(object Source, PropertyChangedEventArgs Args)> ToObservableProperties(this INotifyPropertyChanged notifier)
        {
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => notifier.PropertyChanged += handler,
                handler => notifier.PropertyChanged -= handler)
                .Select(eventPattern => (eventPattern.Sender, eventPattern.EventArgs));
        }

        public static IObservable<TProp> ObservableProperty<TObj, TProp>(this TObj obj, Expression<Func<TObj, TProp>> expression) where TObj : INotifyPropertyChanged
        {
            if (expression.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("The expression must be a Lambda.");

            if (expression.Body is MemberExpression expr && expr.NodeType == ExpressionType.MemberAccess)
            {
                var func = expression.Compile();

                return from e in obj.ToObservableProperties()
                       where e.Args.PropertyName == expr.Member.Name
                       select func(obj);
            }
            else
            {
                throw new ArgumentException("The expression must be a member access.");
            }
        }
    }
}
