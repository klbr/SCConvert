using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCConverter.Models;

namespace SCConverter.Converter
{
    internal abstract class IPdfConverter
    {
        public event EventHandler<double> Progress;

        protected PdfLineGroup lastGroup = null;

        protected bool endLine = false;

        protected abstract bool IsValidLine(string line);

        protected abstract bool Convert(string lPdfLineRegisterine, PdfLineRegister actualRegister, out PdfLineRegister newRegister);

        protected abstract bool IsValid(string[] pdfLines, out string error);

        public virtual async Task<string[]> ConvertPdf(string[] pdfLines)
        {
            string error;
            if (!this.IsValid(pdfLines, out error))
            {
                throw new Exception(error);
            }
            var registers = new List<PdfLineRegister>();
            var sb = new StringBuilder();
            PdfLineRegister reg = null;
            for (int i = 0; i < pdfLines.Length; i++)
            {
                string line = pdfLines[i];
                if (this.IsValidLine(line))
                {
                    PdfLineRegister tmp = null;
                    if (this.Convert(line, reg, out tmp))
                    {
                        registers.Add(reg = tmp);
                    }
                }
                if (Progress != null)
                {
                    Progress.Invoke("Convertendo dados do PDF", (i + 1.0) / (double)pdfLines.Length);
                    if (i % 500 == 0)
                    {
                        await Task.Delay(1);
                    }
                }
            }
            for (int i = 0; i < registers.Count; i++)
            {
                sb.Append(registers[i].ToString());
                if (Progress != null)
                {
                    Progress.Invoke("Registrando dados convertidos", (i + 1.0) / (double)registers.Count);
                    if (i % 500 == 0)
                    {
                        await Task.Delay(1);
                    }
                }
            }
            return sb.ToString().Split(new char[]
            {
                '\n'
            });
        }

        public static IPdfConverter CreateByCheckingFirstLine(PdfType type)
        {
            if (type == PdfType.FeedStock)
            {
                return new SinapiFeedStock();
            }
            return new SinapiComposition();
        }
    }
}
