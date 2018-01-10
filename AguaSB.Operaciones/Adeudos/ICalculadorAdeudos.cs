using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Adeudos
{
    public interface ICalculadorAdeudos
    {
        Adeudo ObtenerAdeudo(Contrato contrato);
    }
}
