
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using DACS_WebClothes.Authorization;
namespace DACS_WebClothes.Authorization
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