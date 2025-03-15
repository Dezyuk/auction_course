using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;
using System.Data;

namespace SothbeysKillerApi.Services
{
    public class DbUserService : IUserService
    {
        private readonly IDbConnection _dbConnection;

        public DbUserService()
        {
            _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=user_db;Username=postgres;Password=123456");
            _dbConnection.Open();
        }

        public void SignupUser(UserCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3 || request.Name.Length > 255)
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrWhiteSpace(request.Email) || _dbConnection.ExecuteScalar<bool>("select exists(select * from users where email = @Email)", new { Email = request.Email }))
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

            var command = $@"insert into users (id, name, email, password) values (@Id, @Name, @Email, @Password);";
            _dbConnection.ExecuteScalar(command, user);
        }

        public UserSigninResponse SigninUser(UserSigninRequest request)
        {
            var select = "select * from users where email = @Email";
            var _user = _dbConnection.QuerySingleOrDefault<User>(select, new { Email = request.Email });

            if (_user is null)
            {
                throw new NullReferenceException();
            }
            if (!(_user.Password.Equals(request.Password)))
            {
                throw new ArgumentException();
            }

            var response = new UserSigninResponse(_user.Id, _user.Name, _user.Email);
            return response;

        }
    }
}
