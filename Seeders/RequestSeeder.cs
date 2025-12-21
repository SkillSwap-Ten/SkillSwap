using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Models;

namespace SkillSwap.Seeders
{
    public class RequestSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>().HasData(
                new Request
                {
                    Id = 1,
                    DisponibilitySchedule = "Lunes a Viernes, 9 AM - 5 PM",
                    Description = "Hola, quiero conectar contigo para aprender sobre desarrollo en C#.",
                    IdStateRequest = 2,
                    IdReceivingUser = 1,  // Usuario con ID 1
                    IdRequestingUser = 2   // Usuario con ID 2
                },
                new Request
                {
                    Id = 2,
                    DisponibilitySchedule = "Sábados, 10 AM - 3 PM",
                    Description = "¡Hola! Me gustaría aprender sobre diseño gráfico y creo que podrías ayudarme.",
                    IdStateRequest = 2,
                    IdReceivingUser = 2,  // Usuario con ID 2
                    IdRequestingUser = 3   // Usuario con ID 3
                },
                new Request
                {
                    Id = 3,
                    DisponibilitySchedule = "Lunes, Miércoles y Viernes, 1 PM - 4 PM",
                    Description = "Me encantaría aprender más sobre SEO y marketing digital contigo.",
                    IdStateRequest = 2,
                    IdReceivingUser = 3,  // Usuario con ID 3
                    IdRequestingUser = 4   // Usuario con ID 4
                },
                new Request
                {
                    Id = 4,
                    DisponibilitySchedule = "Martes y Jueves, 3 PM - 6 PM",
                    Description = "Hola, estoy interesado en mejorar mis habilidades de comunicación y creo que tú eres la persona adecuada para ayudarme.",
                    IdStateRequest = 2,
                    IdReceivingUser = 4,  // Usuario con ID 4
                    IdRequestingUser = 5   // Usuario con ID 5
                },
                new Request
                {
                    Id = 5,
                    DisponibilitySchedule = "Todo el día disponible",
                    Description = "¡Hola! Me gustaría aprender sobre producción de video y creo que tu experiencia puede ser valiosa.",
                    IdStateRequest = 2,
                    IdReceivingUser = 5,  // Usuario con ID 5
                    IdRequestingUser = 6   // Usuario con ID 6
                },
                new Request
                {
                    Id = 6,
                    DisponibilitySchedule = "Lunes a Viernes, 10 AM - 5 PM",
                    Description = "Quiero mejorar mis habilidades en JavaScript, ¿podemos conectar?",
                    IdStateRequest = 2,
                    IdReceivingUser = 1,  // Usuario con ID 1
                    IdRequestingUser = 7   // Usuario con ID 7
                },
                new Request
                {
                    Id = 7,
                    DisponibilitySchedule = "Sábados, 11 AM - 2 PM",
                    Description = "Me gustaría aprender más sobre gestión de proyectos, ¿te gustaría ayudarme?",
                    IdStateRequest = 2,
                    IdReceivingUser = 2,  // Usuario con ID 2
                    IdRequestingUser = 8   // Usuario con ID 8
                },
                new Request
                {
                    Id = 8,
                    DisponibilitySchedule = "Martes y Jueves, 1 PM - 4 PM",
                    Description = "Estoy interesado en estrategias de contenido, ¿puedes ayudarme?",
                    IdStateRequest = 2,
                    IdReceivingUser = 3,  // Usuario con ID 3
                    IdRequestingUser = 9   // Usuario con ID 9
                },
                new Request
                {
                    Id = 9,
                    DisponibilitySchedule = "Lunes a Miércoles, 3 PM - 6 PM",
                    Description = "Hola, quiero aprender sobre edición de video y me gustaría tu guía.",
                    IdStateRequest = 2,
                    IdReceivingUser = 4,  // Usuario con ID 4
                    IdRequestingUser = 10  // Usuario con ID 10
                },
                new Request
                {
                    Id = 10,
                    DisponibilitySchedule = "Todo el día disponible",
                    Description = "Quiero explorar más sobre análisis de datos, ¿puedes ayudarme?",
                    IdStateRequest = 2,
                    IdReceivingUser = 5,  // Usuario con ID 5
                    IdRequestingUser = 1   // Usuario con ID 1
                },
                new Request
                {
                    Id = 11,
                    DisponibilitySchedule = "Lunes a Viernes, 9 AM - 5 PM",
                    Description = "Estoy buscando mejorar mis habilidades en UX/UI, ¿puedes asesorarme?",
                    IdStateRequest = 2,
                    IdReceivingUser = 6,  // Usuario con ID 6
                    IdRequestingUser = 2   // Usuario con ID 2
                },
                new Request
                {
                    Id = 12,
                    DisponibilitySchedule = "Sábados, 10 AM - 3 PM",
                    Description = "Me gustaría aprender sobre marketing en redes sociales, ¿puedes ayudarme?",
                    IdStateRequest = 1,
                    IdReceivingUser = 7,  // Usuario con ID 7
                    IdRequestingUser = 3   // Usuario con ID 3
                },
                new Request
                {
                    Id = 13,
                    DisponibilitySchedule = "Lunes, Miércoles y Viernes, 1 PM - 4 PM",
                    Description = "Hola, quiero mejorar mis habilidades de redacción, ¿podemos conectar?",
                    IdStateRequest = 1,
                    IdReceivingUser = 8,  // Usuario con ID 8
                    IdRequestingUser = 4   // Usuario con ID 4
                },
                new Request
                {
                    Id = 14,
                    DisponibilitySchedule = "Martes y Jueves, 3 PM - 6 PM",
                    Description = "Quiero aprender sobre producción de contenido, ¿puedes asesorarme?",
                    IdStateRequest = 1,
                    IdReceivingUser = 9,  // Usuario con ID 9
                    IdRequestingUser = 5   // Usuario con ID 5
                },
                new Request
                {
                    Id = 15,
                    DisponibilitySchedule = "Todo el día disponible",
                    Description = "Hola, me gustaría aprender sobre gestión de tiempo y productividad, ¿puedes ayudarme?",
                    IdStateRequest = 1,
                    IdReceivingUser = 10, // Usuario con ID 10
                    IdRequestingUser = 1   // Usuario con ID 1
                }
            );
        }
    }
}