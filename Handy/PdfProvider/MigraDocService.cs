/*using OnlineShopFinal.PdfProvider.DataModel;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.XPath;
using System.Xml;

namespace OnlineShopFinal.PdfProvider
{
    public class MigraDocService : IMigraDocService
    {
        private readonly string _createdDocsPath = ".\\PdfProvider\\Created";
        private readonly string _imagesPath = ".\\PdfProvider\\Images";



        private readonly string _fillerTextShortText = "fdvsvsvsfv fsfdsfs fdfs fdfdsfsd fdsfsdf fdf";
        private readonly string _fillerTextMediumText = "fdvsvsvsfv fdsfsdfsf fdsfs fsdf fdsfsdfsd fdsfdsf f dsfdsfs  deded deded dedede dedede deded deded deded defdsfsdf fdfsdfsd  fdfdsfsdf fdfsdfs fdfdsfsd fdsfsdf fdf";
        private readonly string _fillerTextText = "fdvsvsvsfv fdsfsdfsf fdsfs fsdf fdsfsdfsd fdsfdsf f dsfdsfs fdsfsdf fdfsdfsd  fdfdsfsdf fdfsdfs fdfdsfsd fdsfsdf fdf";

        public string CreateMigraDocPdf(PdfData pdfData)
        {
            // Create a MigraDoc document
            Document document = CreateDocument(pdfData);
            string mdddlName = $"{_createdDocsPath}/{pdfData.DocumentName}-{DateTime.UtcNow.ToOADate()}.mdddl";
            string docName = $"{_createdDocsPath}/{pdfData.DocumentName}-{DateTime.UtcNow.ToOADate()}.pdf";

            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, mdddlName);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(docName);

            return docName;
        }

        private Document CreateDocument(PdfData pdfData)
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = pdfData.DocumentTitle;
            document.Info.Subject = pdfData.Description;
            document.Info.Author = pdfData.CreatedBy;
            DefineStyles(document);

            CreatePage(document);
            DefineTables(document);
            //FillContent(document);

            return document;
        }
        private void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
        private void CreatePage(Document document)
        {
            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();

            // Put a logo in the header
            Image image = section.AddImage($"{_imagesPath}\\logo.jpg");
            //Image image = section.Headers.Primary.AddImage("~/images/logo.jpg");
            image.Height = "2.5cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();
            
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "5.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Cologne, ");
            paragraph.AddDateField("dd.MM.yyyy");

           
        }
        private void DefineTables(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.AddBookmark("Tables");

            DemonstrateSimpleTable(document);
            DemonstrateTableAlignment(document);


        }

        private void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph();

            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Borders.Visible = true;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.VerticalAlignment = VerticalAlignment.Top;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Items");
            cell = row.Cells[1];
            cell.AddParagraph("Product Name");
            cell = row.Cells[2];
            cell.AddParagraph("Price");
            cell = row.Cells[3];
            cell.AddParagraph("Quantity");
            cell = row.Cells[4];
            cell.AddParagraph("Total");

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph("Apple Phone");
            cell = row.Cells[2];
            cell.AddParagraph("123455");
            cell = row.Cells[3];
            cell.AddParagraph("13");
            cell = row.Cells[4];
            cell.AddParagraph("13536732");

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph("Laptop");
            cell = row.Cells[2];
            cell.AddParagraph("123455");
            cell = row.Cells[3];
            cell.AddParagraph("11");
            cell = row.Cells[4];
            cell.AddParagraph("13536732");

            document.LastSection.Add(table);
        }

        private void DemonstrateTableAlignment(Document document)
        {
            document.LastSection.AddParagraph();

            Table table = new Table();
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Borders.Width = 0.75;
            table.Borders.Visible = false;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;
            

            table.AddColumn(Unit.FromCentimeter(5));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(3));
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.VerticalAlignment = VerticalAlignment.Top;
            Cell cell = row.Cells[3];
            cell.AddParagraph("Total  Price :");
            cell = row.Cells[4];
            cell.AddParagraph("1689765");          

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
            cell.AddParagraph("Vat 19% :");
            cell = row.Cells[4];
            cell.AddParagraph("123456");
   
            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
            cell.AddParagraph("Shipping Cost:");
            cell = row.Cells[4];
            cell.AddParagraph("456644");        
            document.LastSection.Add(table);

        }








    }
}
*/