using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iText.Layout;
using System.IO;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using iText.Forms;
using iText.Forms.Fields;
using Newtonsoft.Json;
using iText.Kernel.Pdf.Annot;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MPdf.Service.Controllers
{



    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {

        private IHostingEnvironment _env;
        public DocumentsController(IHostingEnvironment env)
        {

            _env = env;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("Content/{id}")]
        public FileStreamResult GetContent(int id)
        {
            string path = GetFilePath();
            return new FileStreamResult(new FileStream(path, FileMode.Open), "application/pdf");
        }

        private string GetFilePath()
        {
            return Path.Combine(_env.WebRootPath, "Documents\\i-134.pdf");
        }


        private string GetTempFilePath()
        {
            string guidFile = Guid.NewGuid().ToString("N");
            return Path.Combine(_env.WebRootPath, $"Output\\{guidFile}.pdf");
        }

        // GET api/values/5
        [HttpGet("{id}/Fields")]
        public IEnumerable<PdfFormField> GetFields(int id)
        {
            List<PdfFormField> fields = new List<PdfFormField>();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(GetFilePath())))
            {
                PdfAcroForm pdfForm = PdfAcroForm.GetAcroForm(pdfDoc, false);

                foreach (var de in pdfForm.GetFormFields())
                {
                    var formField = de.Value;
                    fields.Add(new PdfFormField() { Name = de.Key, Value = formField.GetValueAsString() });
                }

            }

            return fields;
        }

        //// POST api/values
        //[HttpPost("{id}/FillForm")]
        //public void Post([FromBody]string value)
        //{



        //}

        // PUT api/values/5
         [HttpPost("{id}/FillForm")]
        public FileStreamResult FillForm(int id, [FromBody] PdfFormFieldCollection value)
        {
            string tempFile = GetTempFilePath();
            using (PdfReader reader = new PdfReader(GetFilePath()))
            {
                reader.SetUnethicalReading(true);
                using (PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(tempFile)))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    form.SetGenerateAppearance(true);
                    foreach (var field in value)
                    {
                        var pdfField = form.GetField(field.Name);
                        pdfField.SetValue(field.Value);
                    }
                    pdfDoc.Close();
                }
            }

            return new FileStreamResult(new FileStream(tempFile, FileMode.Open), "application/pdf");



        }


        [HttpGet("{id}/Annotate")]
        public FileStreamResult Annotate(int id)
        {
            string tempFile = GetTempFilePath();
            using (PdfReader reader = new PdfReader(GetFilePath()))
            {
                reader.SetUnethicalReading(true);
                using (PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(tempFile)))
                {
                    PdfAcroForm pdfForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    //var list = pdfForm.GetPdfObject().Values();
                    
                    foreach (var de in pdfForm.GetFormFields())
                    {
                        var formField = de.Value;
                        PdfAnnotation text = PdfAnnotation.MakeAnnotation(formField.GetPdfObject()); //, PdfAnnotation.(){ , new Rectangle(200f, 250f, 300f, 350f), "Fox", "The fox is quick", true, "Comment");
                        if(text != null )
                            text.s( new PdfString(de.Key));
                        //fields.Add(new PdfFormField() { Name = de.Key, Value = formField.GetValueAsString() });
                    }
                    pdfDoc.Close();
                }
            }

            return new FileStreamResult(new FileStream(tempFile, FileMode.Open), "application/pdf");



        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
