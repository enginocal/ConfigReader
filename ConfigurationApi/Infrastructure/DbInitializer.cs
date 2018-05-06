using ConfigurationApi.Models;
using System;
using System.Linq;

namespace ConfigurationApi.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(ConfigDbContext context)
        {
            context.Database.EnsureCreated();
                       
            if (context.Configurations.Any())
            {
                return;   // DB has been seeded
            }

            var configs = new Config[]
            {
                new Config{ApplicationName="ServiceTraining",Id=Guid.NewGuid(),IsActive=true,Name="Deneme",Type="String",Value="Deneme"}
            };

            foreach (var item in configs)
            {
                context.Configurations.Add(item);
            }
            context.SaveChanges();
        }
    }
}
