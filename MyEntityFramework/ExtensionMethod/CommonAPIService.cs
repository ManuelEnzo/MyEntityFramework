using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEntityFramework.ExtensionMethod
{
    public static class CommonAPIService
    {
        public static IServiceCollection AddCommonAPIServices<TContext>(this IServiceCollection services, string namespaceLocationDtos)
                    where TContext : DbContext
        {
            // Verifica che il parametro namespaceLocationDtos non sia null o vuoto
            if (string.IsNullOrEmpty(namespaceLocationDtos))
            {
                throw new ArgumentNullException(nameof(namespaceLocationDtos));
            }

            // Itera attraverso tutti gli assembly caricati nell'app domain corrente
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Recupera tutti i tipi da tutti gli assembly che appartengono al namespace specificato
            var dtoTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                                     .Where(t => t.Namespace != null &&
                                                 string.Equals(t.Namespace, namespaceLocationDtos, StringComparison.InvariantCultureIgnoreCase) &&
                                                 !t.IsAbstract &&
                                                 !t.IsInterface);

            // Per ogni tipo di DTO trovato, registra il servizio corrispondente
            foreach (var dtoType in dtoTypes)
            {
                var commonApiType = typeof(ICommonAPI<>).MakeGenericType(dtoType);
                var commonApiImplementationType = typeof(CommonAPI<>).MakeGenericType(dtoType);

                services.AddScoped(commonApiType, serviceProvider =>
                {
                    var dbContext = serviceProvider.GetRequiredService<TContext>();
                    return Activator.CreateInstance(commonApiImplementationType, dbContext);
                });
            }

            return services;
    }
    }
}
