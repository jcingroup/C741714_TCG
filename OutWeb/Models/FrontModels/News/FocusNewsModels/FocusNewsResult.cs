using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.FocusNewsModels
{
    public class FocusNewsResult : IPaginationModel
    {
        public Dictionary<int,string> FocusTypeInfo { get; set; }

        private List<FocusNewsData> m_data = new List<FocusNewsData>();
        public List<FocusNewsData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}