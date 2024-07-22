using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyEntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonAPIServices<TContext>(
        this IServiceCollection services,
        string namespaceLocationDtos)
        where TContext : DbContext
    {
        if (string.IsNullOrEmpty(namespaceLocationDtos))
        {
            throw new ArgumentNullException(nameof(namespaceLocationDtos));
        }

        // Ottieni il percorso della cartella di output dell'applicazione
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        // Ottieni tutti i file DLL nella cartella di output
        var assemblyFiles = Directory.GetFiles(basePath, "*.dll");

        foreach (var assemblyFile in assemblyFiles)
        {
            try
            {
                // Carica l'assembly
                Assembly assembly = Assembly.LoadFrom(assemblyFile);

                // Trova i tipi nel namespace specificato
                var dtoTypes = assembly.GetTypes()
                    .Where(t => t.Namespace != null &&
                                t.Namespace.Equals(namespaceLocationDtos, StringComparison.InvariantCultureIgnoreCase) &&
                                !t.IsAbstract &&
                                !t.IsInterface)
                    .ToList();

                // Aggiungi i servizi per ogni tipo trovato
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel caricamento dell'assembly {assemblyFile}: {ex.Message}");
            }
        }

        return services;
    }
}
