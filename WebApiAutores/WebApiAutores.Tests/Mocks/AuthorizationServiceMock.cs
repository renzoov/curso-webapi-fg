using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApiAutores.Tests.Mocks
{
  public class AuthorizationServiceMock : IAuthorizationService
  {
    public AuthorizationResult Resultado { get; set; }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, 
      IEnumerable<IAuthorizationRequirement> requirements)
    {
      return Task.FromResult(Resultado);
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
    {
      return Task.FromResult(Resultado);
    }
  }
}
