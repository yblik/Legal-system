using System.Data.SQLite;
using System;
using System.Collections.Generic;
using Legal_system;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string dbPath)
    {
        _connectionString = "Data Source=" + dbPath;
    }

    //private SqliteConnection GetConnection()
    //{
    //    return new SqliteConnection(_connectionString);
    //}

    private SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }

    // ============================
    // EvidenceType
    // ============================
    public void AddEvidenceType(string name)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO EvidenceType (name) VALUES (@name)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ============================
    // Evidence
    // ============================
    public void AddEvidence(string point, int type, int rating, string filePath, string locationInfo)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Evidence (point, type, rating, file_path, location_info)
                    VALUES (@point, @type, @rating, @file, @loc)";

                cmd.Parameters.AddWithValue("@point", point);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@file", filePath);
                cmd.Parameters.AddWithValue("@loc", locationInfo);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // ============================
    // Respondent
    // ============================
    public void AddRespondent(string name)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Respondent (name) VALUES (@name)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ============================
    // Legislation
    // ============================
    public void AddLegislation(string name, string meaningText)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Legislation (name, meaning_text)
                    VALUES (@name, @meaning)";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@meaning", meaningText);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // ============================
    // CaseEvent
    // ============================
    public int AddCaseEvent(int year, int evidenceId, string respondentsJson, string respondentsLegalJson)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                INSERT INTO CaseEvent (year, evidence_id, respondents, respondentsLegal)
                VALUES (@year, @evidence, @respondents, @respondentsLegal);
                SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@evidence", evidenceId);
                cmd.Parameters.AddWithValue("@respondents", respondentsJson);
                cmd.Parameters.AddWithValue("@respondentsLegal", respondentsLegalJson);

                long id = (long)cmd.ExecuteScalar();
                return (int)id;
            }
        }
    }

    // ============================
    // Load methods for ComboBox population
    // ============================

    public Dictionary<int, string> GetRespondents()
    {
        var map = new Dictionary<int, string>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name FROM Respondent";
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        map[reader.GetInt32(0)] = reader.GetString(1);
            }
        }
        return map;
    }

    public Dictionary<int, string> GetLegislation()
    {
        var map = new Dictionary<int, string>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name FROM Legislation";
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        map[reader.GetInt32(0)] = reader.GetString(1);
            }
        }
        return map;
    }

    public Dictionary<int, string> GetEvidence()
    {
        var map = new Dictionary<int, string>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, point FROM Evidence";
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        map[reader.GetInt32(0)] = reader.GetString(1);
            }
        }
        return map;
    }
    public string GetEvidencePointById(int evidenceId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT point FROM Evidence WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", evidenceId);

                object result = cmd.ExecuteScalar();

                return result?.ToString(); // returns null if not found
            }
        }
    }

    public List<TimelineData> GetTimelineData()
    {
        var list = new List<TimelineData>();

        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT 
                        ce.year,
                        ce.evidence_id,
                        ce.respondents,
                        ce.respondentsLegal,
                        e.type,
                        e.rating,
                        l.meaning_text
                    FROM CaseEvent ce
                    LEFT JOIN Evidence e ON e.id = ce.evidence_id
                    LEFT JOIN Legislation l ON l.name = ce.respondentsLegal
                ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new TimelineData
                        {
                            Year = reader.GetInt32(0).ToString(),

                            Evidence = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),

                            EvidenceType = reader.IsDBNull(4) ? "" : reader.GetInt32(4).ToString(),

                            Respondents = reader.IsDBNull(2) ? Array.Empty<string>() : reader.GetString(2).Split(','),

                            Legislation = reader.IsDBNull(3) ? Array.Empty<string>() : reader.GetString(3).Split(','),

                            LegislationDescription = reader.IsDBNull(6) ? "" : reader.GetString(6),

                            Rating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                        };

                        list.Add(data);
                    }

                }
            }
        }

        return list;
    }

    public class EvidenceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Evidence
    {
        public int Id { get; set; }
        public string Point { get; set; }
        public int Type { get; set; }
        public int Rating { get; set; }
        public string FilePath { get; set; }
        public string LocationInfo { get; set; }
    }

    public class Respondent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Legislation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MeaningText { get; set; }
    }

    public class CaseEvent
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int EvidenceId { get; set; }
        public int RespondentPart { get; set; }
    }

    public class CaseEventRespondent
    {
        public int CaseEventId { get; set; }
        public int RespondentId { get; set; }
    }

    public class CaseEventLegislation
    {
        public int CaseEventId { get; set; }
        public int LegislationId { get; set; }
    }
}
//add by:  
//var helper = new DatabaseHelper("timeline.db");

//// Add lookup data
//helper.AddEvidenceType("Photo");
//helper.AddRespondent("Company A");
//helper.AddLegislation("Health & Safety Act");

//// Add evidence
//helper.AddEvidence("Broken railing", 1, 5, @"C:\images\rail.jpg", "North stairwell");

//// Add timeline entry
//int timelineId = helper.AddTimeline(2021, 1, 3);

//// Link respondents + legislation
//helper.AddTimelineRespondent(timelineId, 1);
//helper.AddTimelineLegislation(timelineId, 1);
