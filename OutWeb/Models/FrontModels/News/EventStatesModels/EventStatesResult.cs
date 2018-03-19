using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.EventStatesModels
{
    public class EventStatesResult : IPaginationModel
    {
        public int StatesTypeID { get; set; }

        /// <summary>
        /// 直播網址
        /// </summary>
        public string Url { get; set; }
        private List<EventStatesData> m_data = new List<EventStatesData>();
        public List<EventStatesData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}