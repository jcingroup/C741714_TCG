using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventStatesModels
{
    public class EventStatesContent
    {
        public int? PagingID { get; set; }
        public Dictionary<int, string> StatesCateInfo { get; set; }

        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public EventStatesData Data { get; set; }

    }
}