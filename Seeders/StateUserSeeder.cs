using Microsoft.EntityFrameworkCore;
using SkillSwap.Models;

namespace SkillSwap.Seeders
{
    public class StateUserSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StateUser>().HasData(
            new StateUser { Id = 1, Name = "Activo", Description = "La cuenta está completamente operativa y en uso.", DurationSuspension = 0 },
            new StateUser { Id = 2, Name = "Inactivo", Description = " La cuenta ha sido revisada y está en un estado de pausa o desactivación temporal. ", DurationSuspension = 0 },
            new StateUser { Id = 3, Name = "Suspendido", Description = "La cuenta ha sido suspendida durante 5 dias", DurationSuspension = 5 }
            );
        }
    }
}