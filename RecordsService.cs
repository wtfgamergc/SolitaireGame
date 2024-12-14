using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data.SQLite;

namespace Solitaire.Records
{
    public class Record
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }

    public class RecordsService
    {
        private string ConnectionString = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "RecordsSolitaire.db") + ";";

        public void SaveRecord(string playerName, int score)
        {
            var connection = new SQLiteConnection(ConnectionString);
            using (connection)
            {
                var command = new SQLiteCommand("INSERT INTO Records (PlayerName, Score) VALUES (@PlayerName, @Score)", connection);
                command.Parameters.AddWithValue("@PlayerName", playerName);
                command.Parameters.AddWithValue("@Score", score);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Record> GetRecords()
        {
            var records = new List<Record>();
            var connection = new SQLiteConnection(ConnectionString);
            using (connection)
            {
                var command = new SQLiteCommand("SELECT PlayerName, Score FROM Records ORDER BY Score DESC", connection);
                connection.Open();
                var reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        records.Add(new Record
                        {
                            PlayerName = reader.GetString(0),
                            Score = reader.GetInt32(1)
                        });
                    }
                }
                return records;
            }
        }

    }

}

