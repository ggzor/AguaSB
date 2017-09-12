using System.Threading.Tasks;
using System.Windows.Input;

namespace GGUtils.MVVM.Async
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
