using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using TaxiCameBack.Core.DomainModel.Membership;

namespace TaxiCameBack.Website.Application.Security
{
    public class CustomPricipal : IPrincipal
    {
        private List<string> Roles;

        public CustomPricipal(List<string> roles)
        {
            Roles = roles;
        }
        public bool IsInRole(string role)
        {
            var roles = role.Split(',');
            return roles.Any(r => Roles.Contains(r));
        }

        public IIdentity Identity { get; set; }
    }
}