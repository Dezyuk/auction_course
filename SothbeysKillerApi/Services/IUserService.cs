using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services
{
    public interface IUserService
    {
        void SignupUser(UserCreateRequest request);
        UserSigninResponse SigninUser(UserSigninRequest request);
    }
}
