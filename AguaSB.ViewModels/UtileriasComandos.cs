using System.Windows.Input;
using System.Waf.Applications;

using MoreLinq;

using GGUtils.MVVM.Async;

namespace AguaSB.ViewModels
{
    public static class UtileriasComandos
    {
        public static void VerificarPuedeEjecutarEn(params ICommand[] comandos) => comandos.ForEach(comando =>
        {
            switch (comando)
            {
                case DelegateCommand c:
                    c.RaiseCanExecuteChanged();
                    break;
                case AsyncDelegateCommand<int> c:
                    c.RaiseCanExecuteChanged();
                    break;
                default:
                    break;
            };
        });
    }
}
