﻿using MediHawker.Repositories.Auth.Implementation;
using MediHawker.Repositories.Auth.Interface;
using MediHawker.Repositories.Consumer.Implementation;
using MediHawker.Repositories.Consumer.Interface;
using MediHawker.Services.Auth.Implemention;
using MediHawker.Services.Auth.Interface;
using MediHawker.Services.Consumer.Implementation;
using MediHawker.Services.Consumer.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medi_hawker_api
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            //Services
            services.AddScoped<IConsumerService, ConsumerService>();
            services.AddScoped<IAuthService, AuthService>();

            //Repositories
            services.AddScoped<IConsumerRepository, ConsumerRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();

            //JWT Token Config
            var key = configuration.GetSection("JWT:Secret").Value;
            var issuer = configuration.GetSection("JWT:ValidIssuer").Value;
            var audience = configuration.GetSection("JWT:ValidAudience").Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))

                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
     }
}
