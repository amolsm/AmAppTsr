using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.ToPdf
{
    public class SavePdf
    {
        public async Task SavePdftoLocationAsync(iTextSharp.text.Document pdfDoc,string filename)
        {
           
            await Task.Run(() => SavePdftoLocation( pdfDoc, filename));
            //return true;
           
        }

        private static void SavePdftoLocation(iTextSharp.text.Document pdfDoc, string filename)
        {
            var output = new FileStream(Path.Combine("D:\\myPDF\\", filename), FileMode.Create);
            var writer1 = PdfWriter.GetInstance(pdfDoc, output);

        }
    }
}
