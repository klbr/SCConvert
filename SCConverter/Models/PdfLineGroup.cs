using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCConverter.Models
{
    internal class PdfLineGroup
    {
        public string BaseCode
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public PdfLineGroup Parent
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public PdfLineGroup GetLevel(int level)
        {
            PdfLineGroup result;
            if (level <= 0)
            {
                result = null;
            }
            else if (this.Level == level)
            {
                result = this;
            }
            else if (this.Parent != null)
            {
                result = this.Parent.GetLevel(level);
            }
            else
            {
                result = null;
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}{1};{2};{3}\n", new object[]
            {
                (this.Level > 1) ? ";" : "",
                this.Code,
                (this.Level == 1) ? ";" : "",
                this.Name
            });
        }
    }
}
