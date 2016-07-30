using Microsoft.AspNet.SignalR;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Website.Application.Security;

namespace TaxiCameBack.Website.Application.Signalr
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        private readonly IMembershipService _membershipService;

        public CustomUserIdProvider(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public string GetUserId(IRequest request)
        {
            var user = _membershipService.GetById(SessionPersister.UserId);
            return user.Email;
        }
    }
}