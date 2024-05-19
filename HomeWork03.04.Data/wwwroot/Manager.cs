using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeWork03._04.Data.wwwroot
{
    public class ContributionInclusion
    {
        public bool Include { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
        public string Cell { get; set; }
        public bool Include { get; set; }
    }

    public class SimchaContributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal? Amount { get; set; }
        public decimal Balance { get; set; }

    }

    public class Actions
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }


    public class Contributions
    {
        public int ContributorId { get; set; }
        public int SimchasId { get; set; }
        public decimal Amount { get; set; }
    }

    public class Simcha
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Contributions { get; set; }
        public decimal Total { get; set; }
    }

    public class Manager
    {
        private string _connectionString;

        public Manager (string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddContributor(Contributor contributor, decimal amount, DateTime date)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors
                                VALUES(@firstName, @lastName, @alwaysInclude, @cell)
                                SELECT SCOPE_IDENTITY() AS Id";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if(!reader.Read())
            {
                return;
            }
            int id = (int)(decimal)reader["Id"];
            AddDeposit(id, amount, date);
        }

        public void AddDeposit(int id, decimal amount, DateTime date)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"INSERT INTO Deposits
                                VALUES(@id, @amount, @date)";
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@date", date);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddContributions(List<SimchaContributor> simchaContributors, int id)
        {
            DeleteContributions(id);

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"INSERT INTO Contributions
                                VALUES(@simchasId, @contributorId, @amount)";
            foreach (SimchaContributor sc in simchaContributors)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@simchasId", id);
                cmd.Parameters.AddWithValue("@contributorId", sc.ContributorId);
                cmd.Parameters.AddWithValue("@amount", sc.Amount);
            }
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddSimcha(string title, DateTime date)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"INSERT INTO Simchas
                                VALUES(@title,@date)";
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@date", date);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Contributor> GetContributors()
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"SELECT cp.Id, cp.FirstName, cp.LastName, cp.AlwaysInclude, cp.Cell, SUM(d.Amount) AS 'Deposits', SUM(c.Amount) AS 'Contributions'
                                FROM Contributors cp
                                JOIN Deposits d
                                ON d.ContributorsId = cp.Id
                                LEFT JOIN Contributions c
                                ON c.ContributorsId = cp.Id
                                GROUP BY cp.Id, cp.FirstName, cp.LastName, cp.AlwaysInclude, cp.Cell";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();
            while (reader.Read())
            {
                var value = reader["Contributions"];
                decimal Contributions;
                if (value == DBNull.Value)
                {
                    Contributions = default(decimal);
                }
                else
                {
                    Contributions = (decimal)value;
                }

                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Cell = (string)reader["Cell"],
                    Balance = (decimal)reader["Deposits"] - Contributions
                });
            }

            return contributors;
        }

        public List<Contributor> GetContributions(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"SELECT cp.Id, cp.FirstName, cp.LastName, cp.Cell, cp.AlwaysInclude, c.SimchasId, SUM(d.Amount) AS 'Deposits', SUM(c.Amount) AS 'Contributions'
                                FROM Contributors cp
                                LEFT JOIN Contributions c
                                ON c.ContributorsId = cp.Id
                                JOIN Deposits d
                                ON d.ContributorsId = cp.Id
                                GROUP BY cp.Id, cp.FirstName, cp.LastName, cp.Cell, cp.AlwaysInclude, c.SimchasId";
            connection.Open();
            List<Contributor> contributors = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var valueC = reader["Contributions"];
                decimal contributions;
                if (valueC == DBNull.Value)
                {
                    contributions = default(decimal);
                }
                else
                {
                    contributions = (decimal)valueC;
                }

                var valueCell = reader["Cell"];
                string cell;
                if (valueCell == DBNull.Value)
                {
                    cell = default(string);
                }
                else
                {
                    cell = (string)valueCell;
                }

                var valueS = reader["SimchasId"];
                int include;
                if (valueS == DBNull.Value)
                {
                    include = default(int);
                }
                else
                {
                    include = (int)valueS;
                }

                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Deposits"] - contributions,
                    Cell = cell,
                    Include = id == include

                });


            }

            return contributors;
        }

        public List<Simcha> GetSimchas()
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"SELECT s.Id, s.Title, s.Date, COUNT(c.SimchasId) AS 'Contributors', SUM(c.Amount) 'Total'
                                FROM Simchas s
                                LEFT JOIN Contributions c
                                ON s.Id = c.SimchasId
                                GROUP BY s.Id, s.Title, s.Date";
            connection.Open();
            List<Simcha> simchas = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var value = reader["Total"];
                decimal total;
                if (value == DBNull.Value)
                {
                    total = default(decimal);
                }
                else
                {
                    total = (decimal)value;
                }

                simchas.Add(new()
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Date = (DateTime)reader["Date"],
                    Contributions = (int)reader["Contributors"],
                    Total = total
                });
            }

            return simchas;
        }

        public int TotalContributorsInSystem() //does it work?
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Count(Id) AS 'Count'
                                FROM Contributors";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            return (int)reader["Count"];
        }

        public List<Actions> GetActions(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.Title, s.Date, c.Amount
                                FROM Simchas s
                                JOIN Contributions c
                                ON s.Id = c.SimchasId
                                JOIN Contributors cp
                                ON c.ContributorsId = cp.Id
                                WHERE cp.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            List<Actions> actions = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var title = (string)reader["Title"];

                actions.Add(new()
                {
                    Action = $"Contributed to the {title} Simcha",
                    Date = (DateTime)reader["Date"],
                    Amount = -(decimal)reader["Amount"]
                });
            }

            actions.AddRange(GetDeposits(id));
            actions.Sort((a, b) =>
            {
                if(a.Date > b.Date)
                {
                    return 1;
                }
                else if (a.Date < b.Date)
                {
                    return -1;
                }

                else
                {
                    return 0;
                }

            });
            return actions;
        }

        public List<Actions> GetDeposits(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT d.Date, d.Amount FROM Deposits d
                                JOIN Contributors cp
                                ON cp.Id = d.ContributorsId
                                WHERE cp.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            List<Actions> actions = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                actions.Add(new()
                {
                    Action = "Deposit",
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]
                });
            }

            return actions;

        }

        public Contributor GetContributor(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT cp.FirstName, cp.LastName, SUM(d.Amount) AS 'Deposits', SUM(c.Amount) AS 'Contributions'
                                FROM Contributors cp
                                JOIN Deposits d
                                ON d.ContributorsId = cp.Id
                                LEFT JOIN Contributions c
                                ON c.ContributorsId = cp.Id
                                GROUP BY cp.FirstName, cp.LastName";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var valueC = reader["Contributions"];
            decimal contributions;
            if (valueC == DBNull.Value)
            {
                contributions = default(decimal);
            }
            else
            {
                contributions = (decimal)valueC;
            }

            return new()
            {
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Balance = (decimal)reader["Deposits"] - contributions,
            };


        }

        public Simcha GetSimcha(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Simchas
                                WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new()
            {
                Id = id,
                Title = (string)reader["Title"],
                Date = (DateTime)reader["Date"]
            };


        }

        public decimal GetTotalAvailable()
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SUM(d.Amount) - SUM(c.Amount) as 'Total'
                                FROM Deposits d
                                JOIN Contributors cp
                                ON d.ContributorsId = cp.Id
                                JOIN Contributions c 
                                ON c.ContributorsId = cp.Id ";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            var value = reader["Total"];
            decimal balance;
            if (value == DBNull.Value)
            {
                balance = default(decimal);
            }
            else
            {
                balance = (decimal)value;
            }

            return balance;
        }

        public void DeleteContributions(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @$"DELETE FROM Contributions
                                WHERE SimchasId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateContributor(Contributor contributor, int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors 
                                SET FirstName = @firstName, LastName = @lastName, AlwaysInclude = @alwaysInclude, Cell = @cell
                                WHERE id = @id";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<SimchaContributor> GetSimchaContributors(int simchaId)
        {
            List<Contributor> contributors = GetContributors();

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions WHERE SimchasId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Contributions> contributions = new List<Contributions>();
            while (reader.Read())
            {
                Contributions contribution = new Contributions
                {
                    Amount = (decimal)reader["Amount"],
                    SimchasId = simchaId,
                    ContributorId = (int)reader["ContributorsId"]
                };
                contributions.Add(contribution);
            }

            return contributors.Select(contributor =>
            {
                var sc = new SimchaContributor();
                sc.FirstName = contributor.FirstName;
                sc.LastName = contributor.LastName;
                sc.AlwaysInclude = contributor.AlwaysInclude;
                sc.ContributorId = contributor.Id;
                sc.Balance = contributor.Balance;
                Contributions contribution = contributions.FirstOrDefault(c => c.ContributorId == contributor.Id);
                if (contribution != null)
                {
                    sc.Amount = contribution.Amount;
                }
                return sc;
            }).ToList();
        }

        public void UpdateSimchaContributions(List<ContributionInclusion> contributors, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchasId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);

            connection.Open();
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            cmd.CommandText = @"INSERT INTO Contributions (SimchasId, ContributorsId, Amount)
                                    VALUES (@simchaId, @contributorId, @amount)";
            foreach (var contributor in contributors.Where(c => c.Include))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
                cmd.Parameters.AddWithValue("@amount", contributor.Amount);
                cmd.ExecuteNonQuery();
            }
        }


    }
}
