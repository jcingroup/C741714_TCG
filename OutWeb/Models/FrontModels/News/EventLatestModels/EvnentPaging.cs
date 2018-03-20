using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventLatestModels
{
    public class EvnentPaging
    {
        public int ID { get; set; }
        public string Current { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public List<PagingImageInfo> ImagesList { get; set; }
        public List<string> InternetSiteList { get; set; }
    }


}