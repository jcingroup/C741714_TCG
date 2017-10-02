using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OutWeb.Repositories;
using OutWeb.Models.Manage.ImgModels;

namespace OutWeb.Models.Manage.ManageNewsModels
{
    public class NewsDetailsDataModel
    {
        MemberViewModel m_imagesData = new MemberViewModel();
        public MemberViewModel ImagesData { get { return this.m_imagesData; } set { this.m_imagesData = value; } }

        public int ID { get; set; }
        public string Title { get; set; }
        public string PublishDateStr { get; set; }


        /// <summary>
        /// 前台顯示
        /// </summary>
        public bool DisplayForFront { get; set; }

        public string DisplayForFrontStr
        {
            get
            {
                string str = string.Empty;
                if (this.DisplayForFront)
                    str = "顯示";
                else
                    str = "隱藏";
                return str;
            }
        }

        /// <summary>
        /// 首頁顯示
        /// </summary>
        public bool DisplayForHomePage { get; set; }

        public string DisplayForHomePageStr
        {
            get
            {
                string str = string.Empty;
                if (this.DisplayForHomePage)
                    str = "顯示";
                else
                    str = "隱藏";
                return str;
            }
        }
        public int? SortIndex { get; set; }
        public string NewsContent { get; set; }
    }
}