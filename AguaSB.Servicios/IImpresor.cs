using System.IO;

namespace AguaSB.Servicios
{
    public interface IImpresor
    {
        void Imprimir(FileInfo archivo);
    }
}
