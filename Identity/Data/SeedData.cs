using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Identity.Config;
using static Shared.Roles.SystemRoles;

namespace Identity.Data
{
    public static class SeedData
    {
        public static void Migrate(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();

                var persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantContext.Database.Migrate();

                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationContext.Database.Migrate();
            }
        }

        public async static void InitializeDatabase(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                foreach (var client in GetClients(configuration))
                {
                    var existingEntity = context.Clients
                        .Include(x => x.RedirectUris)
                        .Include(x => x.AllowedCorsOrigins)
                        .Include(x => x.PostLogoutRedirectUris)
                        .Include(x => x.AllowedScopes)
                        .Include(x => x.ClientSecrets)
                        .Include(x => x.AllowedGrantTypes)
                        .FirstOrDefault(c => c.ClientId == client.ClientId);

                    if (existingEntity == null)
                        context.Clients.Add(client.ToEntity());
                    else
                    {
                        var entityToUpdate = client.ToEntity();
                        entityToUpdate.Id = existingEntity.Id;

                        existingEntity.RedirectUris = entityToUpdate.RedirectUris;
                        existingEntity.AllowedCorsOrigins = entityToUpdate.AllowedCorsOrigins;
                        existingEntity.PostLogoutRedirectUris = entityToUpdate.PostLogoutRedirectUris;
                        existingEntity.AllowedScopes = entityToUpdate.AllowedScopes;
                        existingEntity.ClientSecrets = entityToUpdate.ClientSecrets;
                        existingEntity.AllowedGrantTypes = entityToUpdate.AllowedGrantTypes;

                        context.Entry(existingEntity).CurrentValues.SetValues(entityToUpdate);
                    }

                }
                context.SaveChanges();

                foreach (var identityResource in GetIdentityResources())
                {
                    var existingEntity = context.IdentityResources.FirstOrDefault(ir => ir.Name == identityResource.Name);

                    if (existingEntity == null)
                        context.IdentityResources.Add(identityResource.ToEntity());
                    else
                    {
                        var entityToUpdate = identityResource.ToEntity();
                        entityToUpdate.Id = existingEntity.Id;
                        context.Entry(entityToUpdate).CurrentValues.SetValues(entityToUpdate);
                    }
                }

                context.SaveChanges();

                foreach (var apiResource in GetApiResources())
                {
                    var existingEntity = context.ApiResources.FirstOrDefault(ir => ir.Name == apiResource.Name);

                    if (existingEntity == null)
                        context.ApiResources.Add(apiResource.ToEntity());
                    else
                    {
                        var entityToUpdate = apiResource.ToEntity();
                        entityToUpdate.Id = existingEntity.Id;
                        context.Entry(entityToUpdate).CurrentValues.SetValues(entityToUpdate);
                    }
                }

                context.SaveChanges();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                foreach (var role in GetSystemRolesWithClaims())
                {
                    var existingEntity = roleManager.FindByNameAsync(role.Key).Result;

                    if (existingEntity == null)
                    {
                        existingEntity = new ApplicationRole
                        {
                            Name = role.Key,
                        };

                        await roleManager.CreateAsync(existingEntity);
                        existingEntity = roleManager.FindByNameAsync(role.Key).GetAwaiter().GetResult();

                        foreach (var claim in role.Value)
                        {
                            await roleManager.AddClaimAsync(existingEntity, claim);
                        }
                    }
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                foreach (var pair in GetTestSystemUsersWithRoles())
                {
                    var user = await userManager.FindByEmailAsync(pair.Key.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(pair.Key, CommonTestPassword);
                        user = await userManager.FindByEmailAsync(pair.Key.Email);
                        await userManager.AddToRoleAsync(user, pair.Value);
                    }
                    else
                    {
                        await userManager.UpdateAsync(user);
                    }
                }
            }    
        }
    }
}
