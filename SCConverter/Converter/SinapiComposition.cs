using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCConverter.Models;

namespace SCConverter.Converter
{
    internal class SinapiComposition : IPdfConverter
    {
        public const string FIRST_LINE = "SINAPI - SISTEMA NACIONAL DE PESQUISA DE CUSTOS E ÍNDICES DA CONSTRUÇÃO CIVIL";

        private const string INIT_REGISTER = "VÍNCULO.....: CAIXA REFERENCIAL";

        private int GroupHeaderIndex = 0;

        private bool nextLineValid = false;

        protected override bool IsValidLine(string line)
        {
            bool result;
            if (line == "VÍNCULO.....: CAIXA REFERENCIAL")
            {
                this.nextLineValid = true;
                result = false;
            }
            else
            {
                if (this.nextLineValid && (line.Length < 2 || line[0] != ' ' || line[1] == '-'))
                {
                    this.nextLineValid = false;
                }
                result = this.nextLineValid;
            }
            return result;
        }

        protected override bool IsValid(string[] pdfLines, out string error)
        {
            bool result;
            if (!pdfLines.Any((string x) => x.Contains("SINAPI - SISTEMA NACIONAL DE PESQUISA DE CUSTOS E ÍNDICES DA CONSTRUÇÃO CIVIL")))
            {
                error = "Arquivo não é do SINAPI - Serviços";
                result = false;
            }
            else
            {
                error = "";
                result = true;
            }
            return result;
        }

        protected override bool Convert(string line, PdfLineRegister actualRegister, out PdfLineRegister newRegister)
        {
            PdfLineGroup tempGroup = null;
            newRegister = null;
            if (line[2] != ' ')
            {
                if (this.lastGroup == null || !char.IsDigit(line[2]))
                {
                    if (this.GroupHeaderIndex == 0)
                    {
                        this.GroupHeaderIndex = 1;
                    }
                    tempGroup = new PdfLineGroup();
                    tempGroup.Level = 1;
                    tempGroup.Code = line.Substring(2, 4);
                    tempGroup.Name = line.Substring(15);
                    tempGroup.BaseCode = tempGroup.Code;
                    tempGroup.Parent = null;
                }
                else
                {
                    if (this.GroupHeaderIndex == 0)
                    {
                        this.GroupHeaderIndex = 2;
                    }
                    tempGroup = new PdfLineGroup();
                    tempGroup.Parent = this.lastGroup.GetLevel(1);
                    tempGroup.Level = 2;
                    tempGroup.Code = line.Substring(2, 4);
                    tempGroup.Name = line.Substring(15);
                    tempGroup.BaseCode = tempGroup.Parent.BaseCode;
                }
            }
            else if (line[7] != ' ')
            {
                if (line.Length < 88)
                {
                    if (this.GroupHeaderIndex == 0)
                    {
                        this.GroupHeaderIndex = 3;
                    }
                    tempGroup = new PdfLineGroup();
                    tempGroup.Parent = this.lastGroup.GetLevel(2);
                    tempGroup.Level = 3;
                    tempGroup.Code = line.Substring(5, 5);
                    tempGroup.Name = line.Substring(15);
                    tempGroup.BaseCode = tempGroup.Parent.BaseCode;
                }
                else
                {
                    newRegister = new PdfLineRegister();
                    newRegister.Group = this.lastGroup;
                    newRegister.HeaderIndex = this.GroupHeaderIndex;
                    newRegister.Name = line.Substring(15, 70);
                    newRegister.Code = line.Substring(5, 9).Trim();
                    if (line.Length > 122)
                    {
                        newRegister.UnitType = line.Substring(87, 10).Trim();
                        newRegister.Price = decimal.Parse(line.Substring(110).Trim());
                    }
                    else
                    {
                        newRegister.UnitType = line.Substring(79, 10).Trim();
                        newRegister.Price = decimal.Parse(line.Substring(100).Trim());
                    }
                    this.GroupHeaderIndex = 0;
                }
            }
            else if (actualRegister != null && !string.IsNullOrEmpty(line.Substring(15).Trim()))
            {
                actualRegister.Name += line.Substring(15);
            }
            if (tempGroup != null)
            {
                this.lastGroup = tempGroup;
            }
            return newRegister != null;
        }
    }
}
