using System;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;

using R5T.Dacia;


namespace R5T.Aestia.Database
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="DatabaseAnomalyRepository{TDbContext}"/> implementation of <see cref="IAnomalyRepository"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseAnomalyRepository<TDbContext>(this IServiceCollection services)
            where TDbContext: DbContext, IAnomalyDbContext
        {
            services.AddSingleton<IAnomalyRepository, DatabaseAnomalyRepository<TDbContext>>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseAnomalyRepository{TDbContext}"/> implementation of <see cref="IAnomalyRepository"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IAnomalyRepository> AddDatabaseAnomalyRepositoryAction<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, IAnomalyDbContext
        {
            var serviceAction = ServiceAction.New<IAnomalyRepository>(() => services.AddDatabaseAnomalyRepository<TDbContext>());
            return serviceAction;
        }
    }
}
