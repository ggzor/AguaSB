using Microsoft.Office.Interop.Word;
using System;
using System.IO;

namespace AguaSB.Servicios.Office
{
    public class ImpresorWord : IImpresor, IDisposable
    {
        public Application Word { get; }

        public ImpresorWord()
        {
            Word = new Application
            {
                Visible = false
            };
        }

        public void Imprimir(FileInfo archivo)
        {
            var documento = Word.Documents.Add(archivo.FullName);
            documento.PrintOut();
            documento.Close(SaveChanges: false);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    Word.Application.Quit(SaveChanges: false);

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
