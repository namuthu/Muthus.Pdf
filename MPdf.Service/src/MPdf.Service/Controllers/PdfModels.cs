using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPdf.Service.Controllers
{

    public class PdfFormField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PdfFormFieldCollection : List<PdfFormField>
    {

    }
}
