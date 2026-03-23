using Microsoft.Data.Sqlite;
using System;

using Microsoft.Data.Sqlite;
using System;

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
    public int AddCaseEvent(int year, int evidenceId, int respondentPart)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO CaseEvent (year, evidence_id, respondent_part)
                    VALUES (@year, @evidence, @part);
                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@evidence", evidenceId);
                cmd.Parameters.AddWithValue("@part", respondentPart);

                long id = (long)cmd.ExecuteScalar();
                return (int)id;
            }
        }
    }

    // ============================
    // CaseEventRespondent
    // ============================
    public void AddCaseEventRespondent(int caseEventId, int respondentId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO CaseEventRespondent (case_event_id, respondent_id)
                    VALUES (@event, @respondent)";

                cmd.Parameters.AddWithValue("@event", caseEventId);
                cmd.Parameters.AddWithValue("@respondent", respondentId);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // ============================
    // CaseEventLegislation
    // ============================
    public void AddCaseEventLegislation(int caseEventId, int legislationId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO CaseEventLegislation (case_event_id, legislation_id)
                    VALUES (@event, @legislation)";

                cmd.Parameters.AddWithValue("@event", caseEventId);
                cmd.Parameters.AddWithValue("@legislation", legislationId);

                cmd.ExecuteNonQuery();
            }
        }
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
