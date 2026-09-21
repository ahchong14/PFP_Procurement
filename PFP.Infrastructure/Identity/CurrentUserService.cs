using Microsoft.AspNetCore.Http;
using PFP.Application.Abstractions.Services;
using PFP.Domain.Enums;
using System.Security.Claims;

namespace PFP.Infrastructure.Identity;

internal sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            string? value = GetClaimValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out int userId)
                ? userId
                : null;
        }
    }

    public int? SupplierId
    {
        get
        {
            string? value = GetClaimValue("SupplierId");

            return int.TryParse(value, out int supplierId)
                ? supplierId
                : null;
        }
    }

    public Role? Role
    {
        get
        {
            string? value = GetClaimValue(ClaimTypes.Role);

            return Enum.TryParse<Role>(
                value,
                ignoreCase: true,
                out var role)
                ? role
                : null;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .IsAuthenticated == true;

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirstValue(claimType);
    }
}