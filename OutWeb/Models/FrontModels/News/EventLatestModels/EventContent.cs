using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventLatestModels
{
    public class EventContent
    {
        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public EventLatestData Data { get; set; }

    }
}