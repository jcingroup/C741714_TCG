﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.AboutModels.EducationModels
{
    public class EducationListFilter
    {
        /// <summary>
        /// 選取頁面
        /// </summary>
        public int CurrentPage { get; set; }

        public string LangCode { get; set; }
    }
}