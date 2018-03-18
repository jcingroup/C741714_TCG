using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.EventStatesCategoryModels
{
    public class EventStatesCategoryResult : IPaginationModel
    {
        private List<EventStatesCategoryData> m_data = new List<EventStatesCategoryData>();
        public List<EventStatesCategoryData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}