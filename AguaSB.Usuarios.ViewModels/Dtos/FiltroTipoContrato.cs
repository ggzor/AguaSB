using AguaSB.Nucleo;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class FiltroTipoContrato : Filtro
    {
        private ClaseContrato? claseContrato;
        private TipoContrato tipoContrato;

        public ClaseContrato? ClaseContrato
        {
            get { return claseContrato; }
            set { N.Set(ref claseContrato, value); }
        }

        public TipoContrato TipoContrato
        {
            get { return tipoContrato; }
            set { N.Set(ref tipoContrato, value); }
        }
    }
}
