using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCConverter.Models
{
    internal class PdfLineRegister
    {
        private string name;

        public bool HasHeader
        {
            get
            {
                return this.HeaderIndex > 0;
            }
        }

        public int HeaderIndex
        {
            get;
            set;
        }

        public PdfLineGroup Group
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
            get
            {
                return this.name;
            }
            set
            {
                this.name = value.Replace(';', ',');
            }
        }

        public string UnitType
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (this.HasHeader)
            {
                for (int i = this.HeaderIndex; i <= this.Group.Level; i++)
                {
                    result += this.Group.GetLevel(i).ToString();
                }
            }
            return result + string.Format(";;{0};{1};{2};{3}\n", new object[]
            {
                this.Code,
                this.Name,
                this.UnitType,
                this.Price
            });
        }
    }
}
