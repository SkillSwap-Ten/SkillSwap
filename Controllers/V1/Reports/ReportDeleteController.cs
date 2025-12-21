using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Models;
using Microsoft.EntityFrameworkCore;

namespace SkillSwap.Controllers.V1.Reports;

[ApiController]
[Route("api/[controller]")]
public class ReportDeleteController : ControllerBase
{
    private readonly AppDbContext _context;
    public ReportDeleteController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Deletes a report by its ID.
    /// </summary>
    /// <remarks>
    /// This endpoint allows users to delete a report from the database using its unique identifier.
    /// If the report with the specified ID does not exist, a not found error is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the report to be deleted.</param>
    /// <returns>
    /// A 200 OK response with a success message indicating that the report has been deleted.
    /// A 404 Not Found response if the report with the specified ID does not exist, with a message indicating "No encontrado."
    /// </returns>

    [HttpDelete("DeleteReportById/{id}")]
    public async Task<IActionResult> DeleteReportById(int id)
    {
        // Check if the report exists in the database.
        var find = await checkExistence(id);

        if (find == false)
        {

            // Return a 404 status code if the report is not found.
            return StatusCode(404, ManageResponse.ErrorNotFound());

        }

        // Remove the report from the context and save the changes to the database.
        _context.Reports.Remove(await _context.Reports.FindAsync(id));
        await _context.SaveChangesAsync();

        // Return a success response after the report is deleted.
        return StatusCode(200, ManageResponse.Successfull("Reporte eliminado con éxito"));
    }

    //Checks if a report exists in the database.
    private async Task<bool> checkExistence(int id)
    {
        // Determine if any report matches the given ID
        return await _context.Reports.AnyAsync(p => p.Id == id);
    }
}
