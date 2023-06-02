using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using SafeCity.Api.Controllers;
using SafeCity.Api.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SafeCity.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            _mockSignInManager = new Mock<SignInManager<AppUser>>(_mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);
            _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task SendOtpOnPhone_UserDoesNotExist_CreatesUser()
        {

            var request = new CreateAccountRequest { UserName = "test", FullName = "Test User", Password = "password" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);


            var result = await _controller.SendOtpOnPhone(request);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Login_UserDoesNotExist_ReturnsInternalServerError()
        {
            var request = new LoginRequest { UserName = "test", Password = "password" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);

            var result = await _controller.Login(request);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    }
}
