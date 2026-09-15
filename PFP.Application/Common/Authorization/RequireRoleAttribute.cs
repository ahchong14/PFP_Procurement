using PFP.Domain.Enums;

namespace PFP.Application.Common.Authorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RequireRoleAttribute(params Role[] allowedRoles) : Attribute
{
    public IReadOnlyList<Role> AllowedRoles { get; } = allowedRoles;
}