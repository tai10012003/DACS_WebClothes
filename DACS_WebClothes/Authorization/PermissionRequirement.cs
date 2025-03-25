using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace DACS_WebClothes.Authorization
{

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public int RequiredPermission { get; }

        public PermissionRequirement(int requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }
    }
}
