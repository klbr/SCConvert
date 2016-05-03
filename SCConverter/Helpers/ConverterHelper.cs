using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SCConverter.Converter;
using SCConverter.Models;

namespace SCConverter.Helpers
{
    internal class ConverterHelper
    {
        public event EventHandler<double> Progress;

        internal async Task Convert(string originPath, string targetPath, PdfType type)
        {
            string[] text = await beginConvert(originPath, type);
            await Task.Delay(100);
            await Task.Yield();
            string fileName = createTempFile();
            await assingToFile(text, fileName);
            await Task.Delay(100);
            await Task.Yield();
            File.Copy(fileName, targetPath, true);
            await Task.Delay(100);
            await Task.Yield();
        }

        private async Task<string[]> beginConvert(string originPath, PdfType type)
        {
            var reader = new iTextSharp.text.pdf.PdfReader(originPath);
            var strText = extractTextFromPDF(reader);
            var lines = strText.Split(new char[]
            {
                '\n'
            });
            var converter = IPdfConverter.CreateByCheckingFirstLine(type);
            converter.Progress += Converter_Progress;

            try
            {
                return await converter.ConvertPdf(lines);
            }
            finally
            {
                converter.Progress -= Converter_Progress;
            }
        }

        private void Converter_Progress(object sender, double e)
        {
            if (Progress != null)
            {
                Progress.Invoke(sender, e);
            }
        }

        private string extractTextFromPDF(iTextSharp.text.pdf.PdfReader reader)
        {
            var result = string.Empty;
            try
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    var its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                    string text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, its);
                    text = Encoding.Default.GetString(Encoding.Convert(Encoding.Default, Encoding.Default, Encoding.Default.GetBytes(text)));
                    result += text;
                    if (Progress != null)
                    {
                        Progress.Invoke("Extraindo PDF", page / (double)reader.NumberOfPages);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
            return result;
        }

        private string createTempFile()
        {
            string fileName = Path.GetTempFileName();
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Temporary;
            return fileName;
        }

        private async Task assingToFile(string[] text, string fileName)
        {
            using (var writter = new StreamWriter(fileName, true, Encoding.Default))
            {
                try
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        string line = text[i];
                        writter.WriteLine(line);
                        if (Progress != null)
                        {
                            Progress.Invoke("Gravando dados", (i + 1.0) / (double)text.Length);
                            if (i % 500 == 0)
                            {
                                await Task.Delay(1);
                            }
                        }
                    }
                    writter.Flush();
                }
                finally
                {
                    writter.Close();
                }
            }
        }
    }
}