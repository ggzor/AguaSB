﻿using System;
using System.Collections.Generic;
using System.Linq;

using MoreLinq;

namespace AguaSB.Navegacion
{
    public static class Direccion
    {
        public static ColaNavegacion Descomponer(string direccion)
        {
            if (direccion == null)
                throw new ArgumentNullException(nameof(direccion));

            if (direccion == string.Empty)
            {
                return ColaNavegacion.Vacia;
            }
            else
            {
                var direcciones = direccion.Split('/');

                var ultimaDireccion = direcciones.Last();

                IEnumerable<string> resultadoFinal;

                if (ultimaDireccion.Contains('?'))
                {
                    var parametro = ultimaDireccion.SkipWhile(c => c != '?').Skip(1).ToDelimitedString("");
                    ultimaDireccion = ultimaDireccion.TakeWhile(c => c != '?').ToDelimitedString("");

                    var ultimoIndice = direcciones.Length - 1;
                    direcciones[ultimoIndice] = ultimaDireccion;

                    resultadoFinal = direcciones.Concat(parametro);
                }
                else
                {
                    resultadoFinal = direcciones;
                }

                return new ColaNavegacion(resultadoFinal.Select(s => s.Trim()));
            }
        }
    }
}
