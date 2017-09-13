using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GGUtils.MVVM.Async
{
    /// <summary>
    /// Based on https://msdn.microsoft.com/en-us/magazine/dn630647
    /// </summary>
    public class AsyncDelegateCommand<T> : AsyncCommandBase, INotifyPropertyChanged
    {

        private Func<Task<T>> Command { get; }

        private Func<CancellationToken, Task<T>> CancelableCommand;

        private bool cancellationSupported;

        private CancelAsyncCommand cancelCommand;

        public ICommand CancelCommand { get; } = new EmptyCommand();


        public Func<bool> CanExecuteFunction { get; }


        public AsyncProperty<T> Execution { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncDelegateCommand(Func<Task<T>> command, Func<bool> canExecuteFunction = null)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            CanExecuteFunction = canExecuteFunction;
        }

        public AsyncDelegateCommand(Func<CancellationToken, Task<T>> cancelableCommand, Func<bool> canExecuteFunction = null)
        {
            CancelableCommand = cancelableCommand ?? throw new ArgumentNullException(nameof(cancelableCommand));
            CancelCommand = cancelCommand = new CancelAsyncCommand();
            cancellationSupported = true;
        }

        // Only execute if not executing
        public override bool CanExecute(object parameter) =>
            CanExecuteFunction?.Invoke() ?? Execution == null || Execution.IsCompleted;

        public override async Task ExecuteAsync(object parameter)
        {
            if (cancellationSupported)
                cancelCommand.NotifyCommandStarting();

            if (cancellationSupported)
                Execution = new AsyncProperty<T>(CancelableCommand(cancelCommand.Token));
            else
                Execution = new AsyncProperty<T>(Command());

            RaisePropertiesChanged();

            await Execution.TaskCompletion;

            if (cancellationSupported)
                cancelCommand.NotifyCommandFinished();

            RaisePropertiesChanged();
        }

        private void RaisePropertiesChanged()
        {
            RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(Execution));
        }

        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class EmptyCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => false;

            public void Execute(object parameter) { CanExecuteChanged?.Invoke(null, null); } // Just to supress the warning
        }

        private class CancelAsyncCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private bool commandExecuting;

            private bool CommandExecuting
            {
                get => commandExecuting;

                set
                {
                    commandExecuting = value;
                    RaiseCanExecuteChanged();
                }
            }

            private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public CancellationToken Token => cancellationTokenSource.Token;

            public bool CanExecute(object parameter) => CommandExecuting && !cancellationTokenSource.IsCancellationRequested;

            public void Execute(object parameter)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            internal void NotifyCommandStarting() => CommandExecuting = true;

            internal void NotifyCommandFinished() => CommandExecuting = false;

            private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
