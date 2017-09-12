using System;
using System.Threading.Tasks;

namespace GGUtils.MVVM.Async
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public async void Execute(object parameter) => await ExecuteAsync(parameter);

        public abstract Task ExecuteAsync(object parameter);
        public abstract bool CanExecute(object parameter);
    }
}
