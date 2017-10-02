using System.Collections.Generic;

namespace OutWeb.Models.Manage.ImgModels
{
    public class ImagesModel
    {
        private List<int> m_oldImageIds = new List<int>();
        public List<int> OldImageIds { get { return this.m_oldImageIds; } set { this.m_oldImageIds = value; } }

        public int ID { get; set; }

        /// <summary>
        /// 上傳模式 M:多張 S:單張
        /// </summary>
        public string UploadType { get; set; }

        /// <summary>
        /// Action名稱
        /// </summary>
        public string ActionName { get; set; }

        //public string RedirectToActionUrl { get; set; }

        private List<MemberViewModel> m_memberData = new List<MemberViewModel>();
        public List<MemberViewModel> MemberData { get { return m_memberData; } set { this.m_memberData = value; } }

        private List<MemberViewModel> m_memberDataMultiple = new List<MemberViewModel>();
        public List<MemberViewModel> MemberDataMultiple { get { return m_memberDataMultiple; } set { this.m_memberDataMultiple = value; } }
    }

    public class MemberViewModel
    {
        public int? ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string FileUrl { get; set; }
    }
}