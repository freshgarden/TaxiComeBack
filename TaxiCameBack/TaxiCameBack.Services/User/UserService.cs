using TaxiCameBack.Core;
using TaxiCameBack.Services.Common;

namespace TaxiCameBack.Services.User
{
    public class UserService : EntityService<Core.DomainModel.User.User>, IUserService
    {
        private IRepository<Core.DomainModel.User.User> _repository; 
        public UserService(IRepository<Core.DomainModel.User.User> repository) 
            : base(repository)
        {
            _repository = repository;
        }
    }
}
