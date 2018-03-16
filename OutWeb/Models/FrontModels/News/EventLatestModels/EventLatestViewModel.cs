using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventLatestModels
{
    public class EventLatestViewModel
    {
        EventLatestListFilter m_filter = new EventLatestListFilter();
        EventLatestResult m_result = new EventLatestResult();

        public EventLatestListFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public EventLatestResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}