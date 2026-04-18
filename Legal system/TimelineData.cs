using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legal_system
{
    internal class TimelineData
    {
        public string Year { get; set; }
        public int EvidenceID { get; set; } //to be encoded to and get point in code (not needed for filtering)

        public string EvidenceType { get; set; } //type from evidence 


        public string[] Respondents { get; set; } //to CSV decoded

        public string[] Legislation { get; set; } //to be | decoded then remain CSV -> fetched in code logic for searching
        //NOTE. respondents searched through and must have legislations flagged via loops

        //public string[] LegislationDescription { get; set; } //to be found through legislation name and fetched in code logic for searching
        ////NOTE. via drop down
        ///
        //public int EvidenceRating { get; set; } //the rating from evidence

        //public string FilePath { get; set; } //the file path from evidence
        //public string FilePathInfo { get; set; } //the location info from evidence

    }
}
//LAypout of timeline tab:
//Showing x amount of results , [x][y][z][v] ,<timeline tab> , [filter tab]
//Year: x year, x evidence type
//Point: x point,
//Respondents: x respondent - x leg - dropdown for leg meaning::, y respondent - y leg, z respondent - z leg
//Evidence rating: x rating
//file path: x file path, x location info

//layou t of filter tab: (considering 2 active filters)
// <Year> , <range>, 2020 - 2023 (as example)
// <Evidence type> , <dropdown>[+], [type1][type2][type3]
// [select filter], [+]


