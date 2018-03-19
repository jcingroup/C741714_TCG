using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.FocusNewsModels
{
    public class FocusNewsViewModel
    {
        FocusNewsListFilter m_filter = new FocusNewsListFilter();
        FocusNewsResult m_result = new FocusNewsResult();

        public FocusNewsListFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public FocusNewsResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}