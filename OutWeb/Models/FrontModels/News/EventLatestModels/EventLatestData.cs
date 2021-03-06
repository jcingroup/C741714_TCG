﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.EventLatestModels
{
    public class EventLatestData
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string PublishDateString { get; set; }
        public string Remark { get; set; }
        public int Sort { get; set; }
        public string BD_DTString { get; set; }
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

        public string Img { get; set; }


        private List<EvnentPaging> m_paging = new List<EvnentPaging>();
        public List<EvnentPaging> PagingList { get { return m_paging; } set { m_paging = value; } }
    }
}