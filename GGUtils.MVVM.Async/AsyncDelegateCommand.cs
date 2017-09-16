using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using MoreLinq;

namespace GGUtils.MVVM.Async
{
    /// <summary>
    /// Based on https://msdn.microsoft.com/en-us/magazine/dn630647
    /// </summary>
    public class AsyncDelegateCommand<T> : AsyncCommandBase, INotifyPropertyChanged
    {

        private readonly Func<Task<T>> Command;

        private readonly Func<CancellationToken, Task<T>> CancelableCommand;

        private readonly Func<IProgress<(double Progress, string ProgressMessage)>, Task<T>> ProgressCommand;

        private readonly Func<CancellationToken, IProgress<(double, string)>, Task<T>> ProgressAndCancellationCommand;


        public bool CancellationSupported { get; }

        private CancelAsyncCommand cancelCommand;

        public ICommand CancelCommand { get; } = new EmptyCommand();


        public bool ProgressReportingSupported { get; }

        public double Progress { get; private set; }

        public string ProgressMessage { get; private set; }

        private IProgress<(double, string)> progressListener;


        public Func<bool> CanExecuteFunction { get; }

        private bool MultipleExecutionSupported { get; }

        public AsyncProperty<T> Execution { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;


        private AsyncDelegateCommand(Func<bool> canExecuteFunction, bool multipleExecutionSupported, bool cancellationSupported, bool progressReportingSupported)
        {
            CanExecuteFunction = canExecuteFunction;
            MultipleExecutionSupported = multipleExecutionSupported;
            CancellationSupported = cancellationSupported;
            ProgressReportingSupported = progressReportingSupported;

            if (cancellationSupported)
                CancelCommand = cancelCommand = new CancelAsyncCommand();

            if (progressReportingSupported)
                progressListener = new Progress<(double, string)>(ProgressChanged);
        }

        public AsyncDelegateCommand(Func<Task<T>> command, Func<bool> canExecuteFunction = null, bool multipleExecutionSupported = false)
            : this(canExecuteFunction, multipleExecutionSupported, false, false)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public AsyncDelegateCommand(Func<CancellationToken, Task<T>> command, Func<bool> canExecuteFunction = null, bool multipleExecutionSupported = false)
            : this(canExecuteFunction, multipleExecutionSupported, true, false)
        {
            CancelableCommand = command ?? throw new ArgumentNullException(nameof(command));
        }

        public AsyncDelegateCommand(Func<IProgress<(double Progress, string ProgressMessage)>, Task<T>> command, Func<bool> canExecuteFunction = null,
            bool multipleExecutionSupported = false) : this(canExecuteFunction, multipleExecutionSupported, false, true)
        {
            ProgressCommand = command ?? throw new ArgumentNullException(nameof(command));
        }

        public AsyncDelegateCommand(Func<CancellationToken, IProgress<(double Progress, string ProgressMessage)>, Task<T>> command, Func<bool> canExecuteFunction = null,
            bool multipleExecutionSupported = false) : this(canExecuteFunction, multipleExecutionSupported, true, true)
        {
            ProgressAndCancellationCommand = command ?? throw new ArgumentNullException(nameof(command));
        }


        // Only execute if not executing
        public override bool CanExecute(object parameter)
        {
            if (MultipleExecutionSupported)
            {
                if (CanExecuteFunction != null)
                    return CanExecuteFunction();
                else
                    return true;
            }
            else
            {
                var canMakeSingleExecution = Execution == null || Execution.IsCompleted;

                if (CanExecuteFunction != null)
                    return CanExecuteFunction() && canMakeSingleExecution;
                else
                    return canMakeSingleExecution;
            }
        }

        public override async Task ExecuteAsync(object parameter)
        {
            if (CancellationSupported)
                cancelCommand.NotifyCommandStarting();

            if (ProgressReportingSupported && CancellationSupported)
                Execution = new AsyncProperty<T>(ProgressAndCancellationCommand(cancelCommand.Token, progressListener));
            else if (CancellationSupported)
                Execution = new AsyncProperty<T>(CancelableCommand(cancelCommand.Token));
            else if (ProgressReportingSupported)
                Execution = new AsyncProperty<T>(ProgressCommand(progressListener));
            else
                Execution = new AsyncProperty<T>(Command());

            RaisePropertiesChanged();

            await Execution.TaskCompletion;

            if (CancellationSupported)
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

        private void ProgressChanged((double Progress, string ProgressMessage) progressTuple)
        {
            Progress = progressTuple.Progress;
            ProgressMessage = progressTuple.ProgressMessage;

            new[] { nameof(Progress), nameof(ProgressMessage) }.ForEach(RaisePropertyChanged);
        }

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
