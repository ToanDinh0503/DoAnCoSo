using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace WebBG.Authorization
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
