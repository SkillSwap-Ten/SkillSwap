using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Models;

namespace SkillSwap.Seeders
{
    public class RoleSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Administrador", Description = "Responsable de la gestión y configuración del sitio, incluyendo la moderación de contenido, la gestión de usuarios." },
            new Role { Id = 2, Name = "Usuario", Description = "Un profesional que busca oportunidades laborales o empleadores que buscan candidatos." }
            );
        }
    }
}