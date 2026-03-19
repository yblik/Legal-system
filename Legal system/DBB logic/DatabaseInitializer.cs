using Microsoft.Data.Sqlite;
using System;
using System.IO;

public class DatabaseInitializer
{
    private readonly string _dbPath;

    public DatabaseInitializer(string databaseFileName = "timeline.db")
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, databaseFileName);
    }

    public void Initialize()
    {
        bool createNew = !File.Exists(_dbPath);

        using (var connection = new SqliteConnection("Data Source=" + _dbPath))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                PRAGMA foreign_keys = ON;

                -- ============================
                -- Evidence + Evidence Types
                -- ============================

                CREATE TABLE IF NOT EXISTS EvidenceType (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Evidence (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    point TEXT NOT NULL,
                    type INTEGER NOT NULL,
                    rating INTEGER NOT NULL,
                    file_path TEXT,
                    location_info TEXT,
                    FOREIGN KEY (type) REFERENCES EvidenceType(id)
                );

                -- ============================
                -- Respondents + Legislation
                -- ============================

                CREATE TABLE IF NOT EXISTS Respondent (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Legislation (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL
                );

                -- ============================
                -- Case Events (formerly Timeline)
                -- ============================

                CREATE TABLE IF NOT EXISTS CaseEvent (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    year INTEGER NOT NULL,
                    evidence_id INTEGER NOT NULL,
                    respondent_part INTEGER NOT NULL,
                    FOREIGN KEY (evidence_id) REFERENCES Evidence(id)
                );

                -- ============================
                -- CaseEvent ↔ Respondent (many-to-many)
                -- ============================

                CREATE TABLE IF NOT EXISTS CaseEventRespondent (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    case_event_id INTEGER NOT NULL,
                    respondent_id INTEGER NOT NULL,
                    FOREIGN KEY (case_event_id) REFERENCES CaseEvent(id),
                    FOREIGN KEY (respondent_id) REFERENCES Respondent(id)
                );

                -- ============================
                -- CaseEvent ↔ Legislation (many-to-many)
                -- ============================

                CREATE TABLE IF NOT EXISTS CaseEventLegislation (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    case_event_id INTEGER NOT NULL,
                    legislation_id INTEGER NOT NULL,
                    FOREIGN KEY (case_event_id) REFERENCES CaseEvent(id),
                    FOREIGN KEY (legislation_id) REFERENCES Legislation(id)
                );
";

                cmd.ExecuteNonQuery();
            }
        }
    }
}

//init by
//var db = new DatabaseInitializer();
//db.Initialize();