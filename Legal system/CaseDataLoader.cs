//using System;
//using System.Collections.Generic;

//public class CaseDataLoader
//{
//    private readonly DatabaseHelper _db;

//    public List<EvidenceType> EvidenceTypes { get; private set; }
//    public List<Evidence> Evidence { get; private set; }
//    public List<Respondent> Respondents { get; private set; }
//    public List<Legislation> Legislation { get; private set; }
//    public List<CaseEvent> CaseEvents { get; private set; }
//    public List<CaseEventRespondent> CaseEventRespondents { get; private set; }
//    public List<CaseEventLegislation> CaseEventLegislation { get; private set; }

//    public CaseDataLoader(DatabaseHelper db)
//    {
//        _db = db;
//    }

//    public void LoadAll()
//    {
//        EvidenceTypes = _db.LoadEvidenceTypes();
//        Evidence = _db.LoadEvidence();
//        Respondents = _db.LoadRespondents();
//        Legislation = _db.LoadLegislation();
//        CaseEvents = _db.LoadCaseEvents();
//        CaseEventRespondents = _db.LoadCaseEventRespondents();
//        CaseEventLegislation = _db.LoadCaseEventLegislation();
//    }
//}