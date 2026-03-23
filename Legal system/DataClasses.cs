public class DataClasses
{
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