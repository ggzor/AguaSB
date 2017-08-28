using System;

namespace AguaSB.Extensiones
{
    /// <summary>
    /// Atributo marcador para la clase que describe la extensión.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DescriptorExtensionAttribute : Attribute { }
}
