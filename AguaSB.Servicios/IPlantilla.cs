namespace AguaSB.Servicios
{
    public interface IPlantilla<TPlantilla, TDato>
    {
        TPlantilla Plantilla { get; set; }

        TPlantilla Rellenar(TDato datos);
    }
}
