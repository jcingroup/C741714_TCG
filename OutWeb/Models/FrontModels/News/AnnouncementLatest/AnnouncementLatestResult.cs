using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.AnnouncementLatest
{
    public class AnnouncementLatestResult : IPaginationModel
    {
        private List<AnnouncementLatestData> m_data = new List<AnnouncementLatestData>();
        public List<AnnouncementLatestData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}