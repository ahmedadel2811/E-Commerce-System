using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        string FullName { get; }
        List<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
