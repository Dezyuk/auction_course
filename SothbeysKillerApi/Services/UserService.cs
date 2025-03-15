using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services
{
    public class UserService : IUserService
    {
        private static List<User> _users = [];
        public void SignupUser(UserCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3 || request.Name.Length > 255)
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrWhiteSpace(request.Email) || _users.Any(a => a.Email == request.Email))
            {
                throw new ArgumentException();
            }

            int atIndex = request.Email.IndexOf('@');

            if (atIndex <= 0 || atIndex != request.Email.LastIndexOf('@'))
            {
                throw new ArgumentException();
            }

            string local = request.Email[..atIndex];
            string domain = request.Email[(atIndex + 1)..];

            if (domain.Length < 3 || !domain.Contains('.'))
            {
                throw new ArgumentException();
            }

            string topLevelDomain = (domain.Split('.'))[^1];

            if (topLevelDomain.Length < 2 || topLevelDomain.Length > 6)
            {
                throw new ArgumentException();
            }

            if (!(local.All(l => char.IsLetterOrDigit(l) || l is '.' or '-') && domain.All(l => char.IsLetterOrDigit(l) || l is '.' or '-')))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                throw new ArgumentException();
            }

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            };

            _users.Add(user);
        }

        public UserSigninResponse SigninUser(UserSigninRequest request)
        {
            var user = _users.FirstOrDefault(a => a.Email == request.Email);

            if (user is null)
            {
                throw new NullReferenceException();
            }
            if (!(user.Password.Equals(request.Password)))
            {
                throw new ArgumentException();
            }
            var response = new UserSigninResponse(user.Id, user.Name, user.Email);
            return response;
        }
    }

    
}
