using System.Security.Claims;
using MW.Identity.Token.Constants;
using MW.Identity.Token.Extensions;

namespace MW.Identity.Token.Tests;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    #region GetUserId

    [Fact]
    public void GetUserId_WhenUserIdExists_ReturnsUserId()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.UserId, "user-123"));

        Assert.Equal("user-123", principal.GetUserId());
    }

    [Fact]
    public void GetUserId_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.Email, "test@example.com"));

        Assert.Throws<UnauthorizedAccessException>(() => principal.GetUserId());
    }

    #endregion

    #region Get

    [Fact]
    public void Get_WhenClaimExists_ReturnsValue()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.Email, "test@example.com"));

        Assert.Equal("test@example.com", principal.Get(ClaimConstants.Email));
    }

    [Fact]
    public void Get_WhenClaimMissing_ReturnsNull()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.UserId, "123"));

        Assert.Null(principal.Get(ClaimConstants.Email));
    }

    #endregion

    #region Extension Methods

    [Fact]
    public void GetUserName_ReturnsCorrectValue()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.UserName, "johndoe"));

        Assert.Equal("johndoe", principal.GetUserName());
    }

    [Fact]
    public void GetUserEmail_ReturnsCorrectValue()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.Email, "john@example.com"));

        Assert.Equal("john@example.com", principal.GetUserEmail());
    }

    [Fact]
    public void GetUserDisplayName_ReturnsCorrectValue()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.DisplayName, "John Doe"));

        Assert.Equal("John Doe", principal.GetUserDisplayName());
    }

    [Fact]
    public void GetUserPhone_ReturnsCorrectValue()
    {
        var principal = CreatePrincipal(new Claim(ClaimConstants.Phone, "+1234567890"));

        Assert.Equal("+1234567890", principal.GetUserPhone());
    }

    #endregion

    #region Role Checks

    [Fact]
    public void IsSuperAdmin_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.SuperAdmin));

        Assert.True(principal.IsSuperAdmin());
    }

    [Fact]
    public void IsSuperAdmin_WhenDoesNotHaveRole_ReturnsFalse()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.User));

        Assert.False(principal.IsSuperAdmin());
    }

    [Fact]
    public void IsAdminPanelUser_WhenSuperAdmin_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.SuperAdmin));

        Assert.True(principal.IsAdminPanelUser());
    }

    [Fact]
    public void IsAdminPanelUser_WhenProductAdmin_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.ProductAdmin));

        Assert.True(principal.IsAdminPanelUser());
    }

    [Fact]
    public void IsAdminPanelUser_WhenRegularUser_ReturnsFalse()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.User));

        Assert.False(principal.IsAdminPanelUser());
    }

    [Fact]
    public void IsProductAdmin_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.ProductAdmin));

        Assert.True(principal.IsProductAdmin());
    }

    [Fact]
    public void IsMarketPlaceAdmin_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.MarketPlaceAdmin));

        Assert.True(principal.IsMarketPlaceAdmin());
    }

    [Fact]
    public void IsPublicShopAdmin_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.PublicShopAdmin));

        Assert.True(principal.IsPublicShopAdmin());
    }

    [Fact]
    public void IsExternalSystem_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.ExternalSystem));

        Assert.True(principal.IsExternalSystem());
    }

    [Fact]
    public void IsUser_WhenHasRole_ReturnsTrue()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, SystemRole.User));

        Assert.True(principal.IsUser());
    }

    #endregion
}
