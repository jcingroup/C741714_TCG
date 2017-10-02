namespace OutWeb.Models.Manage.WorksModels
{
    public class WorksListViewModel
    {
        private WorksListFilterModel m_filter = new WorksListFilterModel();
        private WorksListResultModel m_result = new WorksListResultModel();

        public WorksListFilterModel Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public WorksListResultModel Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}