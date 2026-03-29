using Microsoft.Data.Sqlite;
using System;
using System.IO;

public class DatabaseInitializer
{
    private readonly string _dbPath;

    public DatabaseInitializer(string databaseFileName = "legal.db")
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _dbPath = Path.Combine(desktop, databaseFileName);
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
                name TEXT NOT NULL,
                meaning_text TEXT
            );

                );

                -- ============================
                -- Timeline events
                -- ============================

                CREATE TABLE IF NOT EXISTS CaseEvent (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    year INTEGER NOT NULL,
                    evidence_id INTEGER NOT NULL,

                --Tab system
                -- respondent encoded index (r2,r4,r8)
                    respondents STRING NOT NULL,

                --  a 2D array, each row are respondent parts, each column are legislation
                -- [ [L1,L2,L3], [L1], [L1,L2] ] -> 3  respondents here

                    respondentsLegal STRING NOT NULL,

                    FOREIGN KEY (evidence_id) REFERENCES Evidence(id)
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