using System.Collections.Generic;

namespace OutWeb.Models.Manage.ProductKindModels
{
    /// <summary>
    /// 最新消息列表資回傳模型
    /// </summary>
    public class ProductKindListResultModel : IPaginationModel
    {
        private List<ProductKindListDataModel> m_data = new List<ProductKindListDataModel>();
        public List<ProductKindListDataModel> Data { get { return this.m_data; } set { this.m_data = value; } }

        /// <summary>
        /// 分頁模型
        /// </summary>
        public PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}