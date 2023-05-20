using Dapper;
using Kasrat;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace YourProject.Controllers
{
    [ApiController]
    public class PdfController : ControllerBase
    {

        private readonly string connectionString = "Data Source=localhost;Initial Catalog=kasrat;Integrated Security=True";

        [EnableCors("AllowOrigin")]
        [HttpPost]
        [Route("api/[controller]/pdf/upload")]
        [Consumes("multipart/form-data")]
        public response UploadPdf(IFormFile file, [FromForm] string category)
        {
            response response = new response();
            if (file == null || file.Length == 0)
            {
                response.message = "No File Uploaded";
                response.isSuccess = false;
                return response;
            }

            try
            {

                var filePath = Path.Combine("C:\\Users\\basne\\source\\repos\\Kasrat\\Kasrat\\workout\\", Guid.NewGuid().ToString() + ".pdf");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Save the file path and category to the database
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var checkquery = "SELECT COUNT(*) FROM PdfFiles WHERE category = @category";
                    var count = connection.ExecuteScalar<int>(checkquery, new { category = category });
                    if (count > 0)
                    {
                        var updatequery = "UPDATE PdfFiles SET pdf_data = @FilePath WHERE category = @Category";
                        connection.Execute(updatequery, new { FilePath = filePath, Category = category });
                    }
                    else
                    {
                        var query = "INSERT INTO PdfFiles (pdf_data, category) VALUES (@FilePath, @Category)";
                        connection.Execute(query, new { FilePath = filePath, Category = category });
                    }
                }
                response.message = "Successful";
                response.isSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.isSuccess = false;
                return response;
            }
        }


        [HttpGet]
        [Route("api/[controller]/pdf/get/{id}")]
        public IActionResult GetPdf(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT pdf_data FROM PdfFiles WHERE Id = @Id";
                var fileLocation = connection.QuerySingleOrDefault<string>(query, new { Id = id });

                if (fileLocation == null)
                {
                    return NotFound();
                }

                var fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "application/pdf");
            }
        }

        //[HttpPost]
        //[Route("api/[controller]/pdf/get")]
        //public IActionResult GetPdf(pdftype category)
        //{
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        var query = "SELECT pdf_data FROM PdfFiles WHERE category = @Category";
        //        var fileLocation = connection.QuerySingleOrDefault<string>(query, new { Category = category.category });

        //        if (fileLocation == null)
        //        {
        //            return NotFound();
        //        }

        //        var fileBytes = System.IO.File.ReadAllBytes(fileLocation);
        //        return File(fileBytes, "application/pdf");
        //    }
        //}











    }
}
