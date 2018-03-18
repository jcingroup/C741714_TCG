using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventStatesCategoryModels
{
    public class EventStatesCategoryContent
    {
        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public EventStatesCategoryData Data { get; set; }

    }
}