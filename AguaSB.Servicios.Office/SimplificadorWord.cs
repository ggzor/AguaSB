using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

namespace AguaSB.Servicios.Office
{
    public static class SimplificadorWord
    {
        public static void Simplificar(WordprocessingDocument documento)
        {
            var configuracion = new SimplifyMarkupSettings
            {
                RemoveComments = true,
                RemoveContentControls = true,
                RemoveEndAndFootNotes = true,
                RemoveFieldCodes = true,
                RemoveLastRenderedPageBreak = true,
                RemovePermissions = true,
                RemoveProof = true,
                RemoveRsidInfo = true,
                RemoveSmartTags = true,
                RemoveSoftHyphens = true,
                ReplaceTabsWithSpaces = true,
                AcceptRevisions = false,
                RemoveBookmarks = true,
                RemoveGoBackBookmark = true,
                RemoveMarkupForDocumentComparison = true,
                RemoveWebHidden = true,
                RemoveHyperlinks = true,
                NormalizeXml = false
            };

            MarkupSimplifier.SimplifyMarkup(documento, configuracion);
        }
    }
}
