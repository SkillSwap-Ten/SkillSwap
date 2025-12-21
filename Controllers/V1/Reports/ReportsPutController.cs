using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Dtos.Report;
using SkillSwap.Models;

namespace SkillSwap.Controllers.V1.Reports;

[ApiController]
[Route("api/[controller]")]
public class ReportPutController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ReportPutController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Executes an action on a report, such as suspending or enabling a user.
    /// </summary>
    /// <remarks>
    /// This endpoint allows users to take actions on a report based on the specified action type. 
    /// Actions include suspender (suspending), (enabling), or disabling a user associated with the report.
    /// </remarks>
    /// <param name="reportDTO">The report action data transfer object containing the report ID, action taken, and user IDs.</param>
    /// <returns>
    /// A 200 OK response with a success message and updated report information if the action is successful.
    /// A 400 Bad Request response if any required fields are missing or if an unrecognized action is provided.
    /// A 404 Not Found response if the report does not exist or if associated users are not found.
    /// </returns>

    [HttpPut("PutReportByAction")]
    public async Task<IActionResult> PutReportByAction([FromBody] ReportActionDTO reportDTO)
    {
        string message = "";
        if (reportDTO.Id == default(int) || reportDTO.ActionTaken == null || reportDTO.IdReportedUser == default(int) || reportDTO.IdUser == default(int))
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest("Error, ninguno de los datos puede estar vacío."));
        }

        // Search for the report in the database, including related entities
        var report = await _dbContext.Reports
            .Include(r => r.User)
            .Include(r => r.UserReported)
            .Include(r => r.UserReported.IdStateNavigation)
            .Include(r => r.StateReport)
            // Incluir el usuario relacionado
            .FirstOrDefaultAsync(r => r.Id == reportDTO.Id && r.IdReportedUser == reportDTO.IdReportedUser);

        // If the report is not found, return a 404 Not Found response
        if (report == null)
        {
            return StatusCode(400, ManageResponse.ErrorNotFound());
        }

        // Ensure the user is not null before accessing their properties
        if (report.User == null)
        {
            return StatusCode(400, ManageResponse.ErrorNotFound());
        }

        // Ensure the UserReported is not null before accessing their properties
        if (report.UserReported == null)
        {
            return StatusCode(400, ManageResponse.ErrorNotFound());
        }

        // Update the report and user's state based on the action
        if (reportDTO.ActionTaken.Equals("suspender", StringComparison.OrdinalIgnoreCase))
        {
            if (report.UserReported.IdState == 3)
            {
                message = ", El usuario reportado ya se encuentra suspendido";
            }
            else
            {
                report.IdState = 2;
                report.UserReported.IdState = 3;
                report.ActionTaken = "usuario suspendido";
                report.UserReported.SuspensionDate = DateOnly.FromDateTime(DateTime.Now);
                report.UserReported.ReactivationDate = (report.UserReported.SuspensionDate ?? DateOnly.FromDateTime(DateTime.Now)).AddDays(5);
            }

        }
        else if (reportDTO.ActionTaken.Equals("habilitar", StringComparison.OrdinalIgnoreCase))
        {
            if (report.UserReported.IdState == 1)
            {
                message = ", El usuario reportado ya se encuentra habilitado";
            }
            else
            {
                if (DateOnly.FromDateTime(DateTime.Now) > report.UserReported.ReactivationDate)
                {
                    report.IdState = 3;
                    report.UserReported.IdState = 1;
                    report.ActionTaken = "usuario habilitado";
                    report.UserReported.SuspensionDate = null;
                    report.UserReported.ReactivationDate = null;
                }
                else
                {
                    return StatusCode(200, ManageResponse.SuccessfullWithObject("Lo sentimos pero la cuenta sigue suspendida", new
                    {
                        Id_del_reporte = report.Id,
                        Estado = report.StateReport.Name,
                        AccionTomada = report.ActionTaken,
                        id_del_usuario_reportante = report.IdUser,
                        Id_del_usuario_reportado = report.IdReportedUser,
                        nombre = report.User.Name,
                        estado_de_Cuenta_del_usuario_reportado = report.UserReported.IdStateNavigation.Name,
                        fecha_de_suspension = report.UserReported.SuspensionDate,
                        fecha_de_reactivacion = report.UserReported.ReactivationDate
                    }));
                }


            }

        }
        else if (reportDTO.ActionTaken.Equals("deshabilitar", StringComparison.OrdinalIgnoreCase))
        {
            if (report.UserReported.IdState == 2)
            {
                message = ", El usuario reportado ya se encuentra deshabilitado";
            }
            else
            {
                report.IdState = 3;
                report.UserReported.IdState = 2;
                report.ActionTaken = "usuario deshabilitado";
                report.UserReported.SuspensionDate = null;
                report.UserReported.ReactivationDate = null;
            }
        }
        else
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest("Acción no reconocida."));
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        report = await _dbContext.Reports
           .Include(r => r.User)
           .Include(r => r.UserReported)
           .Include(r => r.UserReported.IdStateNavigation)
           .Include(r => r.StateReport)
           // Incluir el usuario relacionado
           .FirstOrDefaultAsync(r => r.Id == reportDTO.Id && r.IdReportedUser == reportDTO.IdReportedUser);

        var response = new
        {
            Id_del_reporte = report.Id,
            Estado = report.StateReport.Name,
            AccionTomada = report.ActionTaken,
            id_del_usuario_reportante = report.IdUser,
            Id_del_usuario_reportado = report.IdReportedUser,
            nombre = report.User.Name,
            estado_de_Cuenta_del_usuario_reportado = report.UserReported.IdStateNavigation.Name,
            fecha_de_suspension = report.UserReported.SuspensionDate,
            fecha_de_reactivacion = report.UserReported.ReactivationDate
        };

        // Return a success response with the updated report information
        return StatusCode(200, ManageResponse.SuccessfullWithObject("Data actualizada " + message, response));
    }
}
