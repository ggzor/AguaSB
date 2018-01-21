using Microsoft.Office.Interop.Word;

using System;
using System.IO;

namespace AguaSB.Servicios.Office
{
    public class ImpresorWord : IImpresor, IDisposable
    {
        public Application Word { get; private set; }

        public ImpresorWord()
        {
            try
            {
                IniciarWord();
            }
            catch (Exception ex)
            {
                throw new Exception("Microsoft Word no está instalado o no se encuentra disponible.", ex);
            }
        }

        public void Imprimir(FileInfo archivo)
        {
            AsegurarIntegridadWord();

            var documento = Word.Documents.Add(archivo.FullName);
            documento.PrintOut();
            documento.Close(SaveChanges: false);
        }

        private void IniciarWord()
        {
            Word = new Application
            {
                Visible = false
            };
        }

        private int conteo = 0;

        private void AsegurarIntegridadWord()
        {
            try
            {
                if (Word == null)
                {
                    IniciarWord();
                }
                else
                {
                    var _ = Word.Documents.Count;
                    conteo++;

                    if (conteo > 20)
                    {
                        Word.Quit(SaveChanges: false);

                        Console.WriteLine("Reiniciando word...");
                        IniciarWord();
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Log
                Console.WriteLine($"Mensaje Word: {ex.Message}");
                Console.WriteLine("Reiniciando word...");
                IniciarWord();
            }
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        Word.Application.Quit(SaveChanges: false);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Log
                        Console.WriteLine($"Mensaje Word: {ex.Message}");
                    }
                }

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
