using ClosedXML.Excel;
using ExcelDatabase.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ExcelDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        private readonly DataContext _context;

        public ExcelUploadController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a file.");
            }

            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var clientsToAdd = new List<Client>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skipping header

                    foreach (var row in rows)
                    {
                        var name = row.Cell(1).GetValue<string>();
                        var surname = row.Cell(2).GetValue<string>();
                        var phoneNumber = row.Cell(3).GetValue<string>();
                        var email = row.Cell(4).GetValue<string>();
                        var country = row.Cell(5).GetValue<string>();
                        var isActiveNowString = row.Cell(6).GetValue<string>().ToLower();
                        bool isActiveNow = isActiveNowString == "yes";

                        bool isNumberValid = false;
                        try
                        {
                            var numberProto = phoneNumberUtil.Parse(phoneNumber, null);
                            isNumberValid = phoneNumberUtil.IsValidNumber(numberProto);

                            // If country information is missing, attempt to determine it from the phone number
                            if (string.IsNullOrEmpty(country))
                            {
                                country = phoneNumberUtil.GetRegionCodeForNumber(numberProto);
                            }
                        }
                        catch (NumberParseException)
                        {
                            // If parsing fails, isNumberValid remains false.
                        }

                        // Check for duplicate phone number in the database
                        var existingClient = await _context.Clients
                                              .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

                        if (existingClient == null) // Only proceed if no duplicate is found
                        {
                            clientsToAdd.Add(new Client
                            {
                                Name = name,
                                Surname = surname,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Country = country,
                                IsActiveNow = isActiveNow,
                                isNumberValid = isNumberValid
                            });
                        }
                    }
                }
            }

            if (clientsToAdd.Any())
            {
                await _context.Clients.AddRangeAsync(clientsToAdd);
                await _context.SaveChangesAsync();
            }

            return Ok($"Successfully processed and added {clientsToAdd.Count} clients to the database.");
        }


    }
}
