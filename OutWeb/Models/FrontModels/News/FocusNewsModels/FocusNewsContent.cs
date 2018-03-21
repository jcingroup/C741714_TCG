using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.FocusNewsModels
{
    public class FocusNewsContent
    {
        public int? PagingID { get; set; }
        public Dictionary<int, string> FocusCateInfo { get; set; }

        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public FocusNewsData Data { get; set; }

    }
}