using AguaSB.Utilerias;
using MoreLinq;
using System.Linq;

namespace AguaSB.Nucleo.Utilerias
{
    public static class CantidadALetra
    {
        private const string NombresUnidades = "un,dos,tres,cuatro,cinco,seis,siete,ocho,nueve";
        private const string NombresMenores20 = "once,doce,trece,catorce,quince,dieciséis,diecisiete,dieciocho,diecinueve";
        private const string NombresDecenas = "diez,veinte,treinta,cuarenta,cincuenta,sesenta,setenta,ochenta,noventa";
        private const string NombresCentenas = "ciento,doscientos,trescientos,cuatrocientos,quinientos,seiscientos,setecientos,ochocientos,novecientos";


        private static string[] ConvertirValoresArreglo(string valores) =>
            valores.Split(',')
            .Select(s => s.Trim())
            .Prepend("")
            .ToArray();

        private static readonly string[] Unidades = ConvertirValoresArreglo(NombresUnidades);
        private static readonly string[] Menores20 = ConvertirValoresArreglo(NombresMenores20);
        private static readonly string[] Decenas = ConvertirValoresArreglo(NombresDecenas);
        private static readonly string[] Centenas = ConvertirValoresArreglo(NombresCentenas);

        public static string Convertir(int numero)
        {
            if (numero == 0)
                return "Cero pesos";
            if (numero == 1)
                return "Un peso";

            string resultado = ConvertirMenorMilMillones(numero);

            return $"{resultado} pesos".Capitalizar();
        }

        private const int UnMillon = 1_000_000;
        private static string ConvertirMenorMilMillones(int numero)
        {
            if (numero < UnMillon)
                return ConvertirMenorUnMillon(numero);

            var millones = numero / UnMillon;
            var parteMenorMillon = numero % UnMillon;

            string resultado;

            if (millones == 1)
                resultado = "Un millón";
            else
                resultado = $"{ConvertirMenorMil(millones)} millones";

            if (parteMenorMillon == 0)
                resultado += " de";
            else
                resultado += $" {ConvertirMenorUnMillon(parteMenorMillon)}";

            return resultado;
        }

        private const int Mil = 1000;
        private static string ConvertirMenorUnMillon(int numero)
        {
            if (numero < Mil)
                return ConvertirMenorMil(numero);

            var millares = numero / Mil;
            var parteMenorMil = numero % Mil;

            var resultado = $"{ConvertirMenorMil(millares)} mil";

            if (parteMenorMil != 0)
                resultado += $" {ConvertirMenorMil(parteMenorMil)}";

            return resultado;
        }

        private const int Cien = 100;

        private static string ConvertirMenorMil(int numero)
        {
            if (numero < Cien)
                return ConvertirMenorCien(numero);

            if (numero == Cien)
                return "cien";

            var centena = numero / Cien;
            var decenaYUnidad = numero % Cien;

            var resultado = Centenas[centena];

            if (decenaYUnidad != 0)
                resultado += $" {ConvertirMenorCien(decenaYUnidad)}";

            return resultado;
        }

        private static string ConvertirMenorCien(int numero)
        {
            if (numero < 20)
                return ConvertirMenorVeinte(numero);

            var decena = numero / 10;
            var unidad = numero % 10;

            var resultado = Decenas[decena];

            if (unidad != 0)
                resultado += $" y {Unidades[unidad]}";

            return resultado;
        }


        private static string ConvertirMenorVeinte(int numero)
        {
            if (numero < 10)
                return Unidades[numero];
            else if (numero == 10)
                return "diez";
            else
                return Menores20[numero - 10];
        }
    }
}
