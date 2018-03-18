using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventStatesCategoryModels
{
    public class EventStatesCategoryViewModel
    {
        EventStatesCategoryListFilter m_filter = new EventStatesCategoryListFilter();
        EventStatesCategoryResult m_result = new EventStatesCategoryResult();

        public EventStatesCategoryListFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public EventStatesCategoryResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}