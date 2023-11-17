using Handy.PdfProvider.DataModel;

namespace Handy.PdfProvider
{
    public interface IPdfSharpService
    {
        string CreatePdf(PdfData pdfData);
    }
}
