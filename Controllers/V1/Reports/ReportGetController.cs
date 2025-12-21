using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkillSwap.Models;
using SkillSwap.Dtos.Report;
using Microsoft.EntityFrameworkCore;

namespace SkillSwap.Controllers.V1.Reports;

[ApiController]
[Route("api/[controller]")]
public class ReportGetController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ReportGetController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all reports from the database.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches a list of all reports available in the database.
    /// </remarks>
    /// <returns>
    /// A 200 OK response with a success message and the list of reports.
    /// </returns>

    [HttpGet("GetReportsAll")]
    public async Task<IActionResult> GetReportsAll()
    {
        // Main query to obtain reports from the database.
        var reports = await _dbContext.Reports
            .Select(report => new Report
            {
                Id = report.Id,
                TitleReport = report.TitleReport,
                Description = report.Description,
                DateReport = report.DateReport,
                ActionTaken = report.ActionTaken,
                IdState = report.IdState,
                IdUser = report.IdUser,
                IdReportedUser = report.IdReportedUser
            })
            .ToListAsync();

        // Process the reports through the ReportManager for any additional logic or projection.
        var reportDtos = await ReportManager.ReportProjection(_dbContext, reports);

        // Return a successful response with the list of reports.
        return StatusCode(200, ManageResponse.SuccessfullWithObject("Data encontrada", reportDtos));
    }

    /// <summary>
    /// Retrieves a report by its ID.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches a specific report based on the provided ID.
    /// If no report with the specified ID exists, a not found error is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the report to retrieve.</param>
    /// <returns>
    /// A 200 OK response with a success message and the report data if found.
    /// A 404 Not Found response if the report with the specified ID does not exist, with a message indicating "No encontrado."
    /// </returns>

    [HttpGet("GetReportById/{id}")]
    public async Task<IActionResult> GetReportById(int id)
    {
        // Query to obtain the report from the database based on the provided ID.
        var reports = await _dbContext.Reports.Where(r => r.Id == id)
        .Select(report => new Report
        {
            Id = report.Id,
            TitleReport = report.TitleReport,
            Description = report.Description,
            DateReport = report.DateReport,
            ActionTaken = report.ActionTaken,
            IdState = report.IdState,
            IdUser = report.IdUser,
            IdReportedUser = report.IdReportedUser
        })
        .ToListAsync();

        // Process the retrieved report through the ReportManager for any additional logic.
        var reportDtos = await ReportManager.ReportProjection(_dbContext, reports);

        // Return a successful response with the report data.
        return StatusCode(200, ManageResponse.SuccessfullWithObject("Data encontrada", reportDtos));
    }
}
