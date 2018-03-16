using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.EventLatestModels
{
    public class EventLatestResult : IPaginationModel
    {
        private List<EventLatestData> m_data = new List<EventLatestData>();
        public List<EventLatestData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}