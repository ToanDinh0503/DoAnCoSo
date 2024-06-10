
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using WebBG.Authorization;
namespace WebBG.Authorization
{

    public static class AuthorizationExtensions
    {
        public static void AddCustomPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminPermission", policy =>
                    policy.Requirements.Add(new PermissionRequirement(1)));
            });
        }
    }
}