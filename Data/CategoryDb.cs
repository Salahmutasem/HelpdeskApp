using Microsoft.Data.SqlClient;
using HelpdeskApp.Models;

namespace HelpdeskApp.Data
{
    public class CategoryDb : BaseDb
    {
        public CategoryDb(IConfiguration configuration) : base(configuration) { }

        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, Name, IsActive, CreatedDate FROM Categories ORDER BY CreatedDate DESC";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IsActive = reader.GetBoolean(2),
                            CreatedDate = reader.GetDateTime(3)
                        });
                    }
                }
            }
            return categories;
        }

        public List<Category> GetActiveCategories()
        {
            var categories = new List<Category>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, Name FROM Categories WHERE IsActive = 1 ORDER BY Name";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }

        public bool CreateCategory(Category category)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var checkQuery = "SELECT COUNT(*) FROM Categories WHERE Name = @Name";
                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Name", category.Name);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0) return false;
                }

                var query = "INSERT INTO Categories (Name, IsActive, CreatedDate) VALUES (@Name, @IsActive, @CreatedDate)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@IsActive", category.IsActive);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public void ToggleCategoryActive(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Categories SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}