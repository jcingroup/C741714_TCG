namespace OutWeb.Models.Manage.ProductModels
{
    public class ProductListViewModel
    {
        private ProductListFilterModel m_filter = new ProductListFilterModel();
        private ProductListResultModel m_result = new ProductListResultModel();

        public ProductListFilterModel Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public ProductListResultModel Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}