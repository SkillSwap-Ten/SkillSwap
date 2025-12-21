using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Models;

namespace SkillSwap.Seeders
{
    public class QualificationSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Qualification>().HasData(
                new Qualification { Id = 1, Count = 10, AccumulatorAdition = 35 },
                new Qualification { Id = 2, Count = 5, AccumulatorAdition = 15 },
                new Qualification { Id = 3, Count = 15, AccumulatorAdition = 60 },
                new Qualification { Id = 4, Count = 8, AccumulatorAdition = 24 },
                new Qualification { Id = 5, Count = 12, AccumulatorAdition = 45 },
                new Qualification { Id = 6, Count = 10, AccumulatorAdition = 35 },
                new Qualification { Id = 7, Count = 5, AccumulatorAdition = 15 },
                new Qualification { Id = 8, Count = 15, AccumulatorAdition = 60 },
                new Qualification { Id = 9, Count = 8, AccumulatorAdition = 24 },
                new Qualification { Id = 10, Count = 12, AccumulatorAdition = 45 }
            );
        }
    }
}