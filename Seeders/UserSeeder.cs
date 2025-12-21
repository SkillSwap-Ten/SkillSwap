using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Models;

namespace SkillSwap.Seeders
{
    public class UserSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<User>();

            // Hash the password and assign it to the user's Password property
            var user1 = new User
            {
                Id = 1,
                Email = "arlex.z96@gmail.com",
                Password = null,
                Name = "arlex",
                LastName = "zapata",
                Birthdate = new DateOnly(2000, 5, 23),
                Description = "Desarrollador full-stack especializado en aplicaciones web.",
                JobTitle = "Desarrollador Web",
                UrlLinkedin = "https://www.linkedin.com/in/arlex-zapata-02aab6243/",
                UrlGithub = "https://github.com/arlexz96",
                UrlBehance = null,
                UrlImage = "https://media.istockphoto.com/id/1200677760/es/foto/retrato-de-apuesto-joven-sonriente-con-los-brazos-cruzados.jpg?s=612x612&w=0&k=20&c=RhKR8pxX3y_YVe5CjrRnTcNFEGDryD2FVOcUT_w3m4w=",
                PhoneNumber = "+1234567891",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 1,
                IdQualification = 1,
                IdAbility = 1,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user2 = new User
            {
                Id = 2,
                Email = "franco@gmail.com",
                Password = null,
                Name = "franco",
                LastName = "mena",
                Birthdate = new DateOnly(1999, 3, 14),
                Description = "Diseñador creativo con pasión por UX/UI.",
                JobTitle = "Diseñador Gráfico",
                UrlLinkedin = "https://www.linkedin.com/in/franccoina/",
                UrlGithub = null,
                UrlBehance = "https://www.behance.net/franccoina",
                UrlImage = "https://media.istockphoto.com/id/1200677760/es/foto/retrato-de-apuesto-joven-sonriente-con-los-brazos-cruzados.jpg?s=612x612&w=0&k=20&c=RhKR8pxX3y_YVe5CjrRnTcNFEGDryD2FVOcUT_w3m4w=",
                PhoneNumber = "+1234567892",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 2,
                IdAbility = 2,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user3 = new User
            {
                Id = 3,
                Email = "jonathan@gmail.com",
                Password = null,
                Name = "jonathan",
                LastName = "escobar",
                Birthdate = new DateOnly(2000, 7, 30),
                Description = "Experto en comunicaciones corporativas y oratoria.",
                JobTitle = "Especialista en Comunicaciones",
                UrlLinkedin = "https://www.linkedin.com/in/jonathanescobarm/",
                UrlGithub = null,
                UrlBehance = null,
                UrlImage = "https://st4allthings4p4ci.blob.core.windows.net/allthingshair/allthingshair/wp-content/uploads/sites/13/2023/04/43848/cortes-de-pelo-para-hombres-jovenes-nueva.jpg",
                PhoneNumber = "+1234567893",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 3,
                IdAbility = 3,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user4 = new User
            {
                Id = 4,
                Email = "joan@ejemplo.com",
                Password = null,
                Name = "joan",
                LastName = "zapata",
                Birthdate = new DateOnly(2005, 11, 11),
                Description = "Estratega de marketing digital enfocado en SEO y marketing de contenidos.",
                JobTitle = "Especialista en Marketing",
                UrlLinkedin = "https://www.linkedin.com/in/davebrown",
                UrlGithub = null,
                UrlBehance = null,
                UrlImage = "https://media.istockphoto.com/id/1319763895/es/foto/sonriente-raza-mixta-hombre-maduro-sobre-fondo-gris.jpg?s=612x612&w=0&k=20&c=sGBwMEZr8RdyFuOF0084teSTc1TwMzdpHjowP9QRWTw=",
                PhoneNumber = "+1234567894",
                CreatedAt = new DateOnly(2024, 2, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 4,
                IdAbility = 4,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user5 = new User
            {
                Id = 5,
                Email = "luisa@gmail.com",
                Password = null,
                Name = "luisa",
                LastName = "ramirez",
                Birthdate = new DateOnly(1999, 6, 20),
                Description = "Productora y editora con experiencia en producción de video y audio.",
                JobTitle = "Productora de Entretenimiento",
                UrlLinkedin = "https://www.linkedin.com/in/luisa-fernanda-ram%C3%ADrez-cardona-0b486565/",
                UrlGithub = null,
                UrlBehance = null,
                UrlImage = "https://media.istockphoto.com/id/1369508766/es/foto/hermosa-mujer-latina-exitosa-sonriendo.jpg?s=612x612&w=0&k=20&c=f-3MdwiVjpE4UWQdqLC3vpWViYMCiGUPr5aKLCmTnDI=",
                PhoneNumber = "+1234567895",
                CreatedAt = new DateOnly(2024, 1, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 5,
                IdAbility = 5,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user6 = new User
            {
                Id = 6,
                Email = "user6@ejemplo.com",
                Password = null,
                Name = "Laura",
                LastName = "Torres",
                Birthdate = new DateOnly(1993, 4, 10),
                Description = "Desarrolladora web apasionada por el frontend.",
                JobTitle = "Desarrolladora Frontend",
                UrlLinkedin = "https://www.linkedin.com/in/lauratorres",
                UrlGithub = "https://github.com/lauratorres",
                UrlBehance = null,
                UrlImage = "https://randomuser.me/api/portraits/women/6.jpg",
                PhoneNumber = "+1234567801",
                CreatedAt = new DateOnly(2024, 2, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 6,
                IdAbility = 6,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user7 = new User
            {
                Id = 7,
                Email = "user7@ejemplo.com",
                Password = null,
                Name = "Javier",
                LastName = "Ramírez",
                Birthdate = new DateOnly(1985, 8, 20),
                Description = "Experto en análisis de datos y visualización.",
                JobTitle = "Analista de Datos",
                UrlLinkedin = "https://www.linkedin.com/in/javierramirez",
                UrlGithub = null,
                UrlBehance = null,
                UrlImage = "https://randomuser.me/api/portraits/men/7.jpg",
                PhoneNumber = "+1234567802",
                CreatedAt = new DateOnly(2024, 2, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 7,
                IdAbility = 7,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user8 = new User
            {
                Id = 8,
                Email = "user8@ejemplo.com",
                Password = null,
                Name = "Sofía",
                LastName = "Gómez",
                Birthdate = new DateOnly(1991, 12, 15),
                Description = "Content creator y especialista en redes sociales.",
                JobTitle = "Gestora de Redes Sociales",
                UrlLinkedin = "https://www.linkedin.com/in/sofiagomez",
                UrlGithub = null,
                UrlBehance = "https://www.behance.net/sofiagomez",
                UrlImage = "https://randomuser.me/api/portraits/women/8.jpg",
                PhoneNumber = "+1234567803",
                CreatedAt = new DateOnly(2024, 3, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 8,
                IdAbility = 8,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user9 = new User
            {
                Id = 9,
                Email = "user9@ejemplo.com",
                Password = null,
                Name = "Fernando",
                LastName = "Martínez",
                Birthdate = new DateOnly(1982, 6, 30),
                Description = "Productor de cine y director.",
                JobTitle = "Productor de Cine",
                UrlLinkedin = "https://www.linkedin.com/in/fernandomartinez",
                UrlGithub = null,
                UrlBehance = null,
                UrlImage = "https://randomuser.me/api/portraits/men/9.jpg",
                PhoneNumber = "+1234567804",
                CreatedAt = new DateOnly(2024, 4, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 9,
                IdAbility = 9,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            var user10 = new User
            {
                Id = 10,
                Email = "user10@ejemplo.com",
                Password = null,
                Name = "Valentina",
                LastName = "Pérez",
                Birthdate = new DateOnly(1994, 11, 5),
                Description = "Desarrolladora de aplicaciones móviles.",
                JobTitle = "Desarrolladora Móvil",
                UrlLinkedin = "https://www.linkedin.com/in/valentinaperez",
                UrlGithub = "https://github.com/valentinaperez",
                UrlBehance = null,
                UrlImage = "https://randomuser.me/api/portraits/women/10.jpg",
                PhoneNumber = "+1234567805",
                CreatedAt = new DateOnly(2024, 5, 1),
                SuspensionDate = null,
                ReactivationDate = null,
                IdState = 1,
                IdRol = 2,
                IdQualification = 10,
                IdAbility = 10,
                PasswordResetToken = null,
                PasswordResetTokenExpiry = null
            };
            user1.Password = passwordHasher.HashPassword(user2, "@A12345");
            user2.Password = passwordHasher.HashPassword(user2, "@A12345");
            user3.Password = passwordHasher.HashPassword(user2, "@A12345");
            user4.Password = passwordHasher.HashPassword(user2, "@A12345");
            user5.Password = passwordHasher.HashPassword(user2, "@A12345");
            user6.Password = passwordHasher.HashPassword(user2, "@A12345");
            user7.Password = passwordHasher.HashPassword(user2, "@A12345");
            user8.Password = passwordHasher.HashPassword(user2, "@A12345");
            user9.Password = passwordHasher.HashPassword(user2, "@A12345");
            user10.Password = passwordHasher.HashPassword(user2, "@A12345");
            modelBuilder.Entity<User>().HasData(user1, user2, user3, user4, user5, user6, user7, user8, user9, user10);
        }
    }
}