using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite;
using System;
using System;
using System.Collections.Generic;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string dbPath)
    {
        _connectionString = "Data Source=" + dbPath;
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
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
