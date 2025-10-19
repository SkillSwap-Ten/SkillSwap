using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Dtos.User;
using SkillSwap.Models;
using SkillSwap.Validations;

namespace SkillSwap.Controllers.V1.Users;
[ApiController]
[Route("api/[controller]")]
public class UsersPutController : ControllerBase
{

    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    //Constructor
    public UsersPutController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Update a user by their own action
    /// </summary>
    /// <remarks>
    /// This endpoint allows a user to update their own information based on the provided `UserPostDTO` object.
    /// </remarks>
    /// <param name="id">The ID of the user to be updated</param>
    /// <param name="userDTO">The updated user data sent in the request body</param>
    /// <returns>
    /// Returns an HTTP status code:
    /// - 200 OK: If the user information is successfully updated.
    /// - 400 Bad Request: If the user is not found or if the validation fails.
    /// </returns>
    [HttpPut("PutUserByUser")]
    public async Task<IActionResult> PutByUser(int id, UserPostDTO userDTO)
    {

        // Attempt to find the user in the database by ID.
        var userFinded = await _dbContext.Users.Include(e => e.Ability).FirstOrDefaultAsync(i => i.Id == id);

        // Check if the user exists in the database by ID
        if (!await CheckExist(id))
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest("Usuario no encontrado."));
        }

        // Validate the user data provided in the DTO.
         var validation = await UserPutValidation.GeneralValidationAsync(_dbContext, userDTO);

        // If validation fails, return a 400 Bad Request response.
        if (validation != "correcto")
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest(validation));
        }

        userFinded.Ability.Category = userDTO.Category;
        userFinded.Ability.Abilities = userDTO.Abilities;

        // Map the updated data from the DTO to the found user entity.
        _mapper.Map(userDTO, userFinded);

        // FIX: Password hash (only if the user changed their password)
        if (!string.IsNullOrEmpty(userDTO.Password))
        {
            var passwordHasher = new PasswordHasher<User>();
            userFinded.Password = passwordHasher.HashPassword(userFinded, userDTO.Password);
        }

        // Save the changes to the database.
        await _dbContext.SaveChangesAsync();

        // Return a success message indicating that the user has been updated.
        return StatusCode(200, ManageResponse.Successfull("El usuario ha sido actulizado"));
    }

    /// <summary>
    /// Update a user by an administrator
    /// </summary>
    /// <remarks>
    /// This endpoint allows an administrator to update a user's information based on the provided `UserPutAdminDTO` object.
    /// </remarks>
    /// <param name="id">The ID of the user to be updated</param>
    /// <param name="userDTO">The updated user data sent in the request body</param>
    /// <returns>
    /// Returns an HTTP status code:
    /// - 200 OK: If the user information is successfully updated.
    /// - 404 Not Found: If the user is not found in the database.
    /// </returns>
    [HttpPut("PutUserByUserAdmin")]
    public async Task<IActionResult> PutByUserFromAdmin(int id, UserPutAdminDTO userDTO)
    {

        // Attempt to find the user in the database by ID.
        var userFinded = await _dbContext.Users.FindAsync(id);

        // If the user is not found, return a 404 Not Found response.
        if (userFinded == null)
        {
            return StatusCode(404, ManageResponse.ErrorNotFound());
        }

        if (userDTO.IdStateUser == 3)
        {
            userDTO.SuspensionDate = DateOnly.FromDateTime(DateTime.Now);
            userDTO.ReactivationDate = (userFinded.SuspensionDate ?? DateOnly.FromDateTime(DateTime.Now)).AddDays(5);
        }
        

        // Map the updated data from the DTO to the found user entity.
        _mapper.Map(userDTO, userFinded);

        // Save the changes to the database.
        await _dbContext.SaveChangesAsync();

        // Return a success message indicating that the user has been updated.
        return StatusCode(200, ManageResponse.Successfull("El usuario ha sido actualizado"));
    }

    /// <summary>
    /// Update user status based on a specified action
    /// </summary>
    /// <remarks>
    /// Endpoint to either enable or disable a user account based on the action provided ("habilitar" to enable, "deshabilitar" to disable).
    /// </remarks>
    /// <param name="id">The ID of the user to be updated</param>
    /// <param name="action">The action to perform on the user account, either "habilitar" or "deshabilitar"</param>
    /// <returns>
    /// Returns an HTTP status code:
    /// - 200 OK: If the user account status is successfully updated or if the account is already in the desired state.
    /// - 400 Bad Request: If the action is not recognized.
    /// - 404 Not Found: If the user with the specified ID is not found.
    /// The response includes a success message and the updated user information, or an error message in case of failure.
    /// </returns>
    [HttpPut("PutUserByAction")]
    public async Task<IActionResult> PutUserByAction(int id, string action)
    {

        string message = "";

        // Find the user by ID, including the user's state navigation
        var userFind = await _dbContext.Users
            .Include(r => r.IdStateNavigation)
            .FirstOrDefaultAsync(i => i.Id == id);

        // Prepare response object with current user information
        var response = new
        {
            id = userFind.IdState,
            nombre = userFind.Name,
            estado = userFind.IdStateNavigation.Name
        };

        // If the user does not exist, return a 404 status with an error message
        if (userFind == null)
        {
            return StatusCode(404, ManageResponse.ErrorNotFound());
        }

        // Update user state based on the action input ("habilitar" or "deshabilitar")
        if (action.Equals("habilitar", StringComparison.OrdinalIgnoreCase))
        {

            // If the user is already enabled, return a success message without updating
            if (userFind.IdState == 1)
            {
                return StatusCode(200, ManageResponse.SuccessfullWithObject("El usuario reportado ya se encuentra habilitado", response));
            }
            userFind.IdState = 1;
            message = "El estado de la cuenta ha sido habilitada";

        }
        else if (action.Equals("deshabilitar", StringComparison.OrdinalIgnoreCase))
        {
            // If the user is already disabled, return a success message without updating
            if (userFind.IdState == 2)
            {
                return StatusCode(200, ManageResponse.SuccessfullWithObject("El usuario reportado ya se encuentra deshabilitado", response));
            }
            userFind.IdState = 2;
            message = "El estado de la cuenta ha sido deshabilitada";
        }
        else
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest("Acción no reconocida."));
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Retrieve updated user information
        var userFind2 = await _dbContext.Users
            .Include(r => r.IdStateNavigation)
            .FirstOrDefaultAsync(i => i.Id == id);

        // Prepare response object with updated user information
        response = new
        {
            id = userFind2.IdState,
            nombre = userFind2.Name,
            estado = userFind2.IdStateNavigation.Name
        };

        // Return a 200 status with a success message and user details.
        return StatusCode(200, ManageResponse.SuccessfullWithObject("Cuenta suspendida con éxito", response));
    }

    /// Checks if a user exists based on their ID.
    private async Task<bool> CheckExist(int id)
    {
        var response = await _dbContext.Users.FindAsync(id);
        return response != null ? true : false;
    }
}