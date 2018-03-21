using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.AboutModels.EducationModels
{
    public class EducationResult : IPaginationModel
    {
        public Dictionary<int,string> EduTypeInfo { get; set; }

        private List<EducationData> m_data = new List<EducationData>();
        public List<EducationData> Data { get { return m_data; } set { m_data = value; } }

        private PaginationResult m_pagination = new PaginationResult();

        public PaginationResult Pagination
        { get { return this.m_pagination; } set { this.m_pagination = value; } }
    }
}