using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GGUtils.MVVM.Async
{
    /// <summary>
    /// Based on https://msdn.microsoft.com/en-us/magazine/dn630647
    /// </summary>
    public class AsyncDelegateCommand<T> : AsyncCommandBase, INotifyPropertyChanged
    {

        private Func<Task<T>> Command { get; }

        public Func<bool> CanExecuteFunction { get; }

        public AsyncProperty<T> Execution { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncDelegateCommand(Func<Task<T>> command, Func<bool> canExecuteFunction)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            CanExecuteFunction = canExecuteFunction;
        }

        public AsyncDelegateCommand(Func<Task<T>> command) : this(command, null) { }

        public AsyncDelegateCommand()
        {

        }

        // Execute if not executing
        public override bool CanExecute(object parameter) =>
            CanExecuteFunction?.Invoke() ?? Execution == null || Execution.IsCompleted;

        public override async Task ExecuteAsync(object parameter)
        {
            Execution = new AsyncProperty<T>(Command());

            RaisePropertiesChanged();

            await Execution.TaskCompletion;

            RaisePropertiesChanged();
        }

        private void RaisePropertiesChanged()
        {
            RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(Execution));
        }

        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
