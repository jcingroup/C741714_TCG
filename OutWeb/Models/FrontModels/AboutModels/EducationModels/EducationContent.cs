using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.AboutModels.EducationModels
{
    public class EducationContent
    {
        public int? PagingID { get; set; }
        public Dictionary<int, string> EduCateInfo { get; set; }

        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public EducationData Data { get; set; }

    }
}