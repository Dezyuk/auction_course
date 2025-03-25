using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;
using System.Data;

namespace SothbeysKillerApi.Repository
{
    public class DbUserRepostory : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public DbUserRepostory(ILogger<DbAuctionRepository> logger)
        {
            _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=auction_db;Username=postgres;Password=123456");
            _dbConnection.Open();
        }

        public bool EmailExist(string email)
        {
            var query = "select exists(select * from users where email = @Email)";
            var answer = _dbConnection.ExecuteScalar<bool>(query, new { Email = email });
            return answer;
        }
        public void Create(User entity)
        {
            var command = $@"insert into users (id, name, email, password) values (@Id, @Name, @Email, @Password);";
            _dbConnection.ExecuteScalar(command, entity);
        }

        public User? Signin(string email)
        {
            var query = "select * from users where email = @Email";
            var user = _dbConnection.QuerySingleOrDefault<User>(query, new { Email = email });
            return user;
        }
    }
}
