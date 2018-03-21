using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.AboutModels.EducationModels
{
    public class EducationViewModel
    {
        EducationListFilter m_filter = new EducationListFilter();
        EducationResult m_result = new EducationResult();

        public EducationListFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public EducationResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}