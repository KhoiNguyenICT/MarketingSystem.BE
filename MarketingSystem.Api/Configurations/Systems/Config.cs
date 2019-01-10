using System;
using System.Collections.Generic;
using System.Security.Claims;
using MarketingSystem.Common.Constants;
using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using Google.Common.Constants;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingSystem.Api.Configurations.Systems
{
    public static class Config
    {
        public static void ConfigureIdentityService(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment env)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 0;
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0);

                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            });

            services.AddIdentity<Account, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.IssuerUri = configuration.GetValue<string>(ConfigurationKeys.WebHostUrl);
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients())
                //.AddTestUsers(GetTestUsers())
                .AddAspNetIdentity<Account>()
                .AddProfileService<ProfileService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(options =>
            {
                options.Authority = configuration.GetValue<string>(ConfigurationKeys.IdentityServerUrl);
                options.RequireHttpsMetadata = false;
            });
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client {
                    ClientId = "MarketingSystem-web-app",
                    ClientName = "MarketingSystem Web Application",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    // AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("MarketingSystem-web-app-secret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "MarketingSystemAPI",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "MarketingSystemAPI",
                    DisplayName = "MarketingSystem API",
                    Description = "MarketingSystem API Access",
                    UserClaims = new List<string> { ClaimTypes.Role, JwtClaimTypes.Role },
                    ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
                    Scopes = new List<Scope> {new Scope("MarketingSystemAPI") }
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "62ef6170-2609-408d-8439-0100386a4acc",
                    Username = "test@digimed.vn",
                    Password = "DigiMed@123",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "test@digimed.vn"),
                        new Claim(JwtClaimTypes.Role, Roles.Administrator)
                    },
                    IsActive = true
                }
            };
        }
    }
}
