using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Dtos.User;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.Validations
{
    public class UserPutValidation
    {
        public static async Task<string> GeneralValidationAsync(AppDbContext _dbContext, [FromBody] UserPutDTO userDTO)
        {

            // Check if email is valid
            if (!DataValidator.ValidateEmail(userDTO.Email))
            {
                return "Correo inválido";
            }

            //Check if name is valid
            if (!DataValidator.ValidateLettersOnly(userDTO.Name))
            {
                return "Nombre inválido";
            }

            //Check if lastName is valid
            if (!DataValidator.ValidateLettersOnly(userDTO.LastName))
            {
                return "Apellido inválido";
            }

            //Check if birthday is not null
            if (!DataValidator.ValidateContainNotNull(userDTO.Birthdate))
            {
                return "Fecha de nacimiento es requerida";
            }

            //Check if category is not null
            if (!DataValidator.ValidateContainNotNull(userDTO.Category))
            {
                return "Las categorias son requeridas";
            }

            //Check if skills is not null
            if (!DataValidator.ValidateContainNotNull(userDTO.Abilities))
            {
                return "Las habilidades son requeridas";
            }

            return "correcto";
        }
    }
}