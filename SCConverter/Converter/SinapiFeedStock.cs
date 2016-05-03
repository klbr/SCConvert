using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCConverter.Models;

namespace SCConverter.Converter
{
    internal class SinapiFeedStock : IPdfConverter
    {
        public const string FIRST_LINE = "PREÇOS DE INSUMOS";

        private const string INIT_REGISTER = "Mediano (R$)";

        private bool nextLineValid = false;

        protected override bool IsValidLine(string line)
        {
            bool result;
            if (line.Contains(INIT_REGISTER))
            {
                this.nextLineValid = true;
                result = false;
            }
            else
            {
                if (this.nextLineValid && line.Contains("Página:"))
                {
                    this.nextLineValid = false;
                }
                result = this.nextLineValid;
            }
            return result;
        }

        protected override bool Convert(string line, PdfLineRegister actualRegister, out PdfLineRegister newRegister)
        {
            newRegister = null;
            string[] regs = line.Split(new char[]
            {
                ' '
            });
            int ret;
            if (char.IsDigit(line[0]) && regs.Length >= 4 && regs[0].Length == 8 && int.TryParse(regs[0], out ret))
            {
                newRegister = new PdfLineRegister();
                newRegister.Group = this.lastGroup;
                newRegister.HeaderIndex = 0;
                newRegister.Name = string.Join(" ", regs.Skip(1).Take(regs.Length - 3));
                newRegister.Code = regs[0];
                newRegister.UnitType = regs[regs.Length - 2];
                newRegister.Price = decimal.Parse(regs[regs.Length - 1]);
            }
            else
            {
                actualRegister.Name = actualRegister.Name + " " + line;
            }
            return newRegister != null;
        }

        protected override bool IsValid(string[] pdfLines, out string error)
        {
            bool result;
            if (!pdfLines.Any((string x) => x.Contains(FIRST_LINE)))
            {
                error = "Arquivo não é do SINAPI - Insumos";
                result = false;
            }
            else
            {
                error = "";
                result = true;
            }
            return result;
        }
    }
}
