using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extenstions
{
    public static class IdentitySeviceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            var builder = services.AddIdentityCore<AppUser>(); // ne pune la dispozitie UserManager<AppUser>
            builder = new IdentityBuilder(builder.UserType, typeof(AppRole), builder.Services);
            builder.AddEntityFrameworkStores<AppIdentityDbContext>(); //da voie lui UserManager sa umble cu IdentityDb a noastra
            builder.AddSignInManager<SignInManager<AppUser>>();
            builder.AddRoleValidator<RoleValidator<AppRole>>();
            builder.AddRoleManager<RoleManager<AppRole>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Token:Issuer"],
                        ValidateIssuer = true,
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}