using OutWeb.Models.Manage.ImgModels;
using System.Collections.Generic;

namespace OutWeb.Models.Manage.WorksModels
{
    public class WorksDetailsDataModel
    {
        private MemberViewModel m_representImage = new MemberViewModel();
        private List<MemberViewModel> m_otherImage = new List<MemberViewModel>();

        /// <summary>
        /// 圖片
        /// </summary>
        public List<MemberViewModel> OtherImagesData { get { return this.m_otherImage; } set { this.m_otherImage = value; } }

        public MemberViewModel ImagesData { get { return this.m_representImage; } set { this.m_representImage = value; } }

        /// <summary>
        /// 主索引
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        public string WorksPulishDateStr{ get; set; }


        /// <summary>
        /// 案例標題
        /// </summary>
        public string WorksTitle { get; set; }

       
        /// <summary>
        /// 內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 前台顯示
        /// </summary>
        public bool DisplayForFrontEnd { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
    }
}