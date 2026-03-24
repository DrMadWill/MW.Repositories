using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MW.Identity.Token.Constants;
using MW.Identity.Token.Services;

namespace MW.Identity.Token.Tests;

public class CurrentUserTests
{
    private static CurrentUser CreateCurrentUser(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext();
        if (user != null)
            httpContext.User = user;

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        return new CurrentUser(httpContextAccessor.Object);
    }

    private static ClaimsPrincipal CreateAuthenticatedUser(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    #region IsAuthenticated

    [Fact]
    public void IsAuthenticated_WhenUserIsAuthenticated_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_WhenNoUser_ReturnsFalse()
    {
        var currentUser = CreateCurrentUser();

        Assert.False(currentUser.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_WhenHttpContextIsNull_ReturnsFalse()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var currentUser = new CurrentUser(httpContextAccessor.Object);

        Assert.False(currentUser.IsAuthenticated);
    }

    #endregion

    #region UserId

    [Fact]
    public void UserId_WhenAuthenticated_ReturnsUserId()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "user-123"));
        var currentUser = CreateCurrentUser(user);

        Assert.Equal("user-123", currentUser.UserId);
    }

    [Fact]
    public void UserId_WhenNotAuthenticated_ReturnsNull()
    {
        var currentUser = CreateCurrentUser();

        Assert.Null(currentUser.UserId);
    }

    [Fact]
    public void UserId_WhenNoUserIdClaim_ReturnsNull()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.Email, "test@example.com"));
        var currentUser = CreateCurrentUser(user);

        Assert.Null(currentUser.UserId);
    }

    #endregion

    #region IsInRole

    [Fact]
    public void IsInRole_WhenUserHasRole_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.SuperAdmin));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.IsInRole(SystemRole.SuperAdmin));
    }

    [Fact]
    public void IsInRole_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.User));
        var currentUser = CreateCurrentUser(user);

        Assert.False(currentUser.IsInRole(SystemRole.SuperAdmin));
    }

    [Fact]
    public void IsInRole_WhenNotAuthenticated_ReturnsFalse()
    {
        var currentUser = CreateCurrentUser();

        Assert.False(currentUser.IsInRole(SystemRole.SuperAdmin));
    }

    #endregion

    #region Roles

    [Fact]
    public void Roles_WithMultipleRoleClaims_ReturnsAllRoles()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.SuperAdmin),
            new Claim(ClaimTypes.Role, SystemRole.ProductAdmin));
        var currentUser = CreateCurrentUser(user);

        var roles = currentUser.Roles;
        Assert.Equal(2, roles.Count);
        Assert.Contains(SystemRole.SuperAdmin, roles);
        Assert.Contains(SystemRole.ProductAdmin, roles);
    }

    [Fact]
    public void Roles_WithCustomRoleClaimType_ReturnsRoles()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimConstants.Role, SystemRole.User));
        var currentUser = CreateCurrentUser(user);

        var roles = currentUser.Roles;
        Assert.Single(roles);
        Assert.Contains(SystemRole.User, roles);
    }

    [Fact]
    public void Roles_WhenNotAuthenticated_ReturnsEmptyList()
    {
        var currentUser = CreateCurrentUser();

        Assert.Empty(currentUser.Roles);
    }

    [Fact]
    public void Roles_WithDuplicateRoles_ReturnsDistinct()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.SuperAdmin),
            new Claim(ClaimConstants.Role, SystemRole.SuperAdmin));
        var currentUser = CreateCurrentUser(user);

        var roles = currentUser.Roles;
        Assert.Single(roles);
    }

    #endregion

    #region CheckUserAuthorize

    [Fact]
    public void CheckUserAuthorize_WhenSuperAdmin_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "admin-123"),
            new Claim(ClaimTypes.Role, SystemRole.SuperAdmin));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.CheckUserAuthorize("other-user-456"));
    }

    [Fact]
    public void CheckUserAuthorize_WhenSameUser_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "user-123"),
            new Claim(ClaimTypes.Role, SystemRole.User));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.CheckUserAuthorize("user-123"));
    }

    [Fact]
    public void CheckUserAuthorize_WhenDifferentUser_ReturnsFalse()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "user-123"),
            new Claim(ClaimTypes.Role, SystemRole.User));
        var currentUser = CreateCurrentUser(user);

        Assert.False(currentUser.CheckUserAuthorize("other-user-456"));
    }

    [Fact]
    public void CheckUserAuthorize_WhenNotAuthenticated_ReturnsFalse()
    {
        var currentUser = CreateCurrentUser();

        Assert.False(currentUser.CheckUserAuthorize("user-123"));
    }

    #endregion

    #region GetHeaderValue

    [Fact]
    public void GetHeaderValue_WhenHeaderExists_ReturnsValue()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        httpContext.Request.Headers["X-Custom-Header"] = "test-value";

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var currentUser = new CurrentUser(httpContextAccessor.Object);

        Assert.Equal("test-value", currentUser.GetHeaderValue("X-Custom-Header"));
    }

    [Fact]
    public void GetHeaderValue_WhenHeaderMissing_ReturnsNull()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        var currentUser = CreateCurrentUser(user);

        Assert.Null(currentUser.GetHeaderValue("X-Missing-Header"));
    }

    [Fact]
    public void GetHeaderValue_WhenHttpContextIsNull_ReturnsNull()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var currentUser = new CurrentUser(httpContextAccessor.Object);

        Assert.Null(currentUser.GetHeaderValue("X-Custom-Header"));
    }

    #endregion

    #region Get and GetJson

    [Fact]
    public void Get_WhenClaimExists_ReturnsValue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimConstants.Email, "test@example.com"));
        var currentUser = CreateCurrentUser(user);

        Assert.Equal("test@example.com", currentUser.Get(ClaimConstants.Email));
    }

    [Fact]
    public void Get_WhenClaimMissing_ReturnsNull()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        var currentUser = CreateCurrentUser(user);

        Assert.Null(currentUser.Get(ClaimConstants.Email));
    }

    [Fact]
    public void GetJson_WhenClaimExistsWithJson_ReturnsDeserialized()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim("customData", "{\"name\":\"test\",\"value\":42}"));
        var currentUser = CreateCurrentUser(user);

        var result = currentUser.GetJson<TestData>("customData");
        Assert.NotNull(result);
        Assert.Equal("test", result.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void GetJson_WhenClaimMissing_ReturnsDefault()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        var currentUser = CreateCurrentUser(user);

        Assert.Null(currentUser.GetJson<TestData>("customData"));
    }

    private class TestData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    #endregion

    #region HasClaimType

    [Fact]
    public void HasClaimType_WhenClaimExists_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimConstants.Email, "test@example.com"));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.HasClaimType(ClaimConstants.Email));
    }

    [Fact]
    public void HasClaimType_WhenClaimMissing_ReturnsFalse()
    {
        var user = CreateAuthenticatedUser(new Claim(ClaimConstants.UserId, "123"));
        var currentUser = CreateCurrentUser(user);

        Assert.False(currentUser.HasClaimType(ClaimConstants.Email));
    }

    [Fact]
    public void HasClaimType_WhenNotAuthenticated_ReturnsFalse()
    {
        var currentUser = CreateCurrentUser();

        Assert.False(currentUser.HasClaimType(ClaimConstants.UserId));
    }

    #endregion

    #region IsSuperAdmin

    [Fact]
    public void IsSuperAdmin_WhenUserIsSuperAdmin_ReturnsTrue()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.SuperAdmin));
        var currentUser = CreateCurrentUser(user);

        Assert.True(currentUser.IsSuperAdmin);
    }

    [Fact]
    public void IsSuperAdmin_WhenUserIsNotSuperAdmin_ReturnsFalse()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimTypes.Role, SystemRole.User));
        var currentUser = CreateCurrentUser(user);

        Assert.False(currentUser.IsSuperAdmin);
    }

    #endregion

    #region Email

    [Fact]
    public void Email_WhenAuthenticated_ReturnsEmail()
    {
        var user = CreateAuthenticatedUser(
            new Claim(ClaimConstants.UserId, "123"),
            new Claim(ClaimConstants.Email, "test@example.com"));
        var currentUser = CreateCurrentUser(user);

        Assert.Equal("test@example.com", currentUser.Email);
    }

    [Fact]
    public void Email_WhenNotAuthenticated_ReturnsNull()
    {
        var currentUser = CreateCurrentUser();

        Assert.Null(currentUser.Email);
    }

    #endregion
}
