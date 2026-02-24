using Microsoft.Data.SqlClient;
using HelpdeskApp.Models;

namespace HelpdeskApp.Data
{
    public class TicketDb : BaseDb
    {
        public TicketDb(IConfiguration configuration) : base(configuration) { }

        public (List<Ticket> tickets, int totalCount) GetTickets(string? search, string? status, int page, int pageSize)
        {
            var tickets = new List<Ticket>();
            int totalCount = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                var whereClause = "WHERE t.IsDeleted = 0";
                if (!string.IsNullOrEmpty(search))
                    whereClause += " AND (t.Title LIKE @Search OR t.Description LIKE @Search)";
                if (!string.IsNullOrEmpty(status))
                    whereClause += " AND t.Status = @Status";

                var countQuery = $"SELECT COUNT(*) FROM Tickets t {whereClause}";
                using (var countCmd = new SqlCommand(countQuery, connection))
                {
                    if (!string.IsNullOrEmpty(search))
                        countCmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                    if (!string.IsNullOrEmpty(status))
                        countCmd.Parameters.AddWithValue("@Status", status);
                    totalCount = (int)countCmd.ExecuteScalar();
                }

                var offset = (page - 1) * pageSize;
                var query = $@"SELECT t.Id, t.Title, t.Description, t.CategoryId, t.CreatedBy, t.Status, t.CreatedDate,
                               c.Name AS CategoryName, u.FullName AS CreatedByName
                               FROM Tickets t
                               LEFT JOIN Categories c ON t.CategoryId = c.Id
                               LEFT JOIN Users u ON t.CreatedBy = u.Id
                               {whereClause}
                               ORDER BY t.CreatedDate DESC
                               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(search))
                        command.Parameters.AddWithValue("@Search", "%" + search + "%");
                    if (!string.IsNullOrEmpty(status))
                        command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new Ticket
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                CategoryId = reader.GetInt32(3),
                                CreatedBy = reader.GetInt32(4),
                                Status = reader.GetString(5),
                                CreatedDate = reader.GetDateTime(6),
                                CategoryName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                CreatedByName = reader.IsDBNull(8) ? "" : reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return (tickets, totalCount);
        }

        public Ticket? GetTicketById(int id)
        {
            Ticket? ticket = null;
            using (var connection = GetConnection())
            {
                connection.Open();

                var query = @"SELECT t.Id, t.Title, t.Description, t.CategoryId, t.CreatedBy, t.Status, t.CreatedDate, t.IsDeleted,
                              c.Name AS CategoryName, u.FullName AS CreatedByName
                              FROM Tickets t
                              LEFT JOIN Categories c ON t.CategoryId = c.Id
                              LEFT JOIN Users u ON t.CreatedBy = u.Id
                              WHERE t.Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ticket = new Ticket
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                CategoryId = reader.GetInt32(3),
                                CreatedBy = reader.GetInt32(4),
                                Status = reader.GetString(5),
                                CreatedDate = reader.GetDateTime(6),
                                IsDeleted = reader.GetBoolean(7),
                                CategoryName = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                CreatedByName = reader.IsDBNull(9) ? "" : reader.GetString(9)
                            };
                        }
                    }
                }

                if (ticket != null)
                {
                    var commentQuery = @"SELECT tc.Id, tc.TicketId, tc.CommentText, tc.CreatedBy, tc.CreatedDate,
                                         u.FullName AS CreatedByName
                                         FROM TicketComments tc
                                         LEFT JOIN Users u ON tc.CreatedBy = u.Id
                                         WHERE tc.TicketId = @TicketId
                                         ORDER BY tc.CreatedDate ASC";
                    using (var commentCmd = new SqlCommand(commentQuery, connection))
                    {
                        commentCmd.Parameters.AddWithValue("@TicketId", id);
                        using (var reader = commentCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ticket.Comments.Add(new TicketComment
                                {
                                    Id = reader.GetInt32(0),
                                    TicketId = reader.GetInt32(1),
                                    CommentText = reader.GetString(2),
                                    CreatedBy = reader.GetInt32(3),
                                    CreatedDate = reader.GetDateTime(4),
                                    CreatedByName = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                });
                            }
                        }
                    }
                }
            }
            return ticket;
        }

        public void CreateTicket(Ticket ticket)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO Tickets (Title, Description, CategoryId, CreatedBy, Status, CreatedDate, IsDeleted)
                              VALUES (@Title, @Description, @CategoryId, @CreatedBy, @Status, @CreatedDate, 0)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", ticket.Title);
                    command.Parameters.AddWithValue("@Description", ticket.Description);
                    command.Parameters.AddWithValue("@CategoryId", ticket.CategoryId);
                    command.Parameters.AddWithValue("@CreatedBy", ticket.CreatedBy);
                    command.Parameters.AddWithValue("@Status", "Open");
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteTicket(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Tickets SET IsDeleted = 1 WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTicketStatus(int id, string status)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Tickets SET Status = @Status WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddComment(TicketComment comment)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO TicketComments (TicketId, CommentText, CreatedBy, CreatedDate)
                              VALUES (@TicketId, @CommentText, @CreatedBy, @CreatedDate)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TicketId", comment.TicketId);
                    command.Parameters.AddWithValue("@CommentText", comment.CommentText);
                    command.Parameters.AddWithValue("@CreatedBy", comment.CreatedBy);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
