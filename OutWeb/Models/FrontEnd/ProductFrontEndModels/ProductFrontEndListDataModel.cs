using OutWeb.Models.Manage.ImgModels;
using OutWeb.Models.Manage.ProductKindModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OutWeb.Models.FrontEnd.ProductFrontEndModels
{
    public class ProductFrontEndListDataModel : IPaginationModel
    {
        private IEnumerable<ProductFrontEndDataModel> m_data;

        public IEnumerable<ProductFrontEndDataModel> Data { get { return this.m_data; } set { this.m_data = value; } }
        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }

    public class ProductFrontEndDataModel
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
        /// 產品名稱
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 產品型號
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 產品規格
        /// </summary>
        public string ProductSpecification { get; set; }

        /// <summary>
        /// 產品材質
        /// </summary>
        public string ProductMaterial { get; set; }

        /// <summary>
        /// 產品特色
        /// </summary>
        public string ProductFeatures { get; set; }

        /// <summary>
        /// 所屬分類索引
        /// </summary>
        public int ProductKindID { get; set; }

        
        /// <summary>
        /// 所屬分類名稱
        /// </summary>
        public string ProductKindName { get; set; }


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