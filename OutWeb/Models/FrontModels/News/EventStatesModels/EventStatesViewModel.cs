using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventStatesModels
{
    public class EventStatesViewModel
    {
        EventStatesListFilter m_filter = new EventStatesListFilter();
        EventStatesResult m_result = new EventStatesResult();

        public EventStatesListFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public EventStatesResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}