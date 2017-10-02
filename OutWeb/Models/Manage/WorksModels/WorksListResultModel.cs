using OutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.Manage.WorksModels
{
    /// <summary>
    /// 產品列表資回傳模型
    /// </summary>
    public class WorksListResultModel : IPaginationModel
    {
        List<WorksListDataModel> m_data = new List<WorksListDataModel>();
        public List<WorksListDataModel> Data { get { return this.m_data; } set { this.m_data = value; } }

        /// <summary>
        /// 分頁模型
        /// </summary>
        public PaginationResult m_pagination = new PaginationResult();
        public PaginationResult Pagination { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}