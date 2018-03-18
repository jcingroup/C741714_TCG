using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventStatesCategoryModels
{
    public class EventStatesCategoryData
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string PublishDateString { get; set; }
        public string Remark { get; set; }
        public int RemarkLength
        {
            get
            {
                int length = 0;
                if (!string.IsNullOrEmpty(Remark))
                {
                    if (Remark.Length > 145)
                        Remark = Remark.Substring(0, 145);
                    length = Remark.Length;
                }
                return length;
            }
        }


        public List<PagingImageInfo> ImagesList { get; set; }
        public List<string> InternetSiteList { get; set; }
    }
}