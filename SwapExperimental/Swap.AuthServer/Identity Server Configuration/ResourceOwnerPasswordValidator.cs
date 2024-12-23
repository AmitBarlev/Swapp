using IdentityServer4.Models;
using IdentityServer4.Validation;
using Swap.Object.GeneralObjects;
using Swap.WebApi.Repositories;
using Swap.WebApi.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Swap.AuthServer.IdentityServerConfiguration
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserRepository _userRepository;

        public ResourceOwnerPasswordValidator(IUserRepository repository)
        {
            _userRepository = repository as UserRepository;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            User user = _userRepository.Get(u => u.Email == context.UserName && u.Password == context.Password);
            if (user != null)
                context.Result = new GrantValidationResult("Success!", authenticationMethod: "Approved Credintials");
            else
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Credintials");
        }
    }
}
