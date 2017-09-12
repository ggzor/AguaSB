using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GGUtils.MVVM.Async
{
    /// <summary>
    /// Based on https://msdn.microsoft.com/en-us/magazine/dn605875
    /// </summary>
    public class AsyncProperty<T> : INotifyPropertyChanged
    {

        public Task<T> Task { get; }

        public Task TaskCompletion { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncProperty(Task<T> task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));

            TaskCompletion = WatchTaskAsync(task);
        }

        public static readonly IEnumerable<string> OnCompletionChangingProperties =
            new[] { nameof(IsCompleted), nameof(IsNotCompleted) };

        public static readonly IEnumerable<string> OnCanceledChangingProperties =
            new[] { nameof(IsCanceled) };

        public static readonly IEnumerable<string> OnFaultedChangingProperties =
            new[] { nameof(IsFaulted), nameof(Exception), nameof(InnerException), nameof(ExceptionMessage) };

        public static readonly IEnumerable<string> OnSuccessChangingProperties =
            new[] { nameof(Result), nameof(IsSuccessfullyCompleted) };

        private async Task WatchTaskAsync(Task<T> task)
        {
            try
            {
                await task;
            }
            catch { }

            if (PropertyChanged != null)
            {
                var changingProperties = Enumerable.Empty<string>();

                if (task.IsCanceled)
                    changingProperties = OnCanceledChangingProperties;
                else if (task.IsFaulted)
                    changingProperties = OnFaultedChangingProperties;
                else
                    changingProperties = OnSuccessChangingProperties;

                changingProperties = changingProperties.Concat(OnCompletionChangingProperties);

                foreach (var propertyChangedName in changingProperties)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyChangedName));
            }
        }

        public bool IsCompleted => Task.IsCompleted;

        public bool IsNotCompleted => !IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;

        public bool IsFaulted => Task.IsFaulted;

        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ExceptionMessage => InnerException?.Message;

        public T Result => IsSuccessfullyCompleted ? Task.Result : default(T);
    }
}
