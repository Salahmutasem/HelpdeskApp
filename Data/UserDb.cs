using Microsoft.Data.SqlClient;
using HelpdeskApp.Models;

namespace HelpdeskApp.Data
{
    public class UserDb : BaseDb
    {
        public UserDb(IConfiguration configuration) : base(configuration) { }

        public User? Login(string email, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, FullName, Email, Password, IsActive FROM Users WHERE Email = @Email AND Password = @Password";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, FullName, Email, IsActive, CreatedDate FROM Users ORDER BY CreatedDate DESC";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2),
                            IsActive = reader.GetBoolean(3),
                            CreatedDate = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return users;
        }

        public bool CreateUser(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Email", user.Email);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0) return false;
                }

                var query = "INSERT INTO Users (FullName, Email, Password, IsActive, CreatedDate) VALUES (@FullName, @Email, @Password, @IsActive, @CreatedDate)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public void ToggleUserActive(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Users SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}