using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GGUtils.MVVM.Async
{
    /// <summary>
    /// Based on https://msdn.microsoft.com/en-us/magazine/dn605875
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
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

            var changingProperties = Enumerable.Empty<string>();

            if (task.IsCanceled)
            {
                IsFaulted = false;
                changingProperties = OnCanceledChangingProperties;
            }
            else if (task.IsFaulted)
            {
                IsFaulted = true;
                changingProperties = OnFaultedChangingProperties;
            }
            else
            {
                IsFaulted = false;
                changingProperties = OnSuccessChangingProperties;
            }

            changingProperties = changingProperties.Concat(OnCompletionChangingProperties);

            foreach (var propertyChangedName in changingProperties)
                Raise(propertyChangedName);
        }

        private void Raise([CallerMemberName]string propertyChangedName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChangedName));

        public bool IsCompleted => Task.IsCompleted;

        public bool IsNotCompleted => !IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;

        public bool? IsFaulted { get; private set; }

        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ExceptionMessage => InnerException?.Message;

        public T Result => IsSuccessfullyCompleted ? Task.Result : default;
    }
}
