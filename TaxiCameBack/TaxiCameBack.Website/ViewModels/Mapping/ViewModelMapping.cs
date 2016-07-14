using System.Linq;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.ViewModels.Mapping
{
    public class ViewModelMapping
    {
        public static SingleMemberListViewModel UserToSingleMemberListViewModel(MembershipUser user)
        {
            var viewModel = new SingleMemberListViewModel
            {
                IsApproved = user.Active,
                Id = user.UserId,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserEmail = user.Email
            };
            return viewModel;
        }

        public static MemberEditViewModel UserToMemberEditViewModel(MembershipUser user)
        {
            var viewModel = new MemberEditViewModel
            {
                IsApproved = user.Active,
                Id = user.UserId,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                Email = user.Email,
            };
            return viewModel;
        }
    }
}