using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.Manage.ProductKindModels
{
    public class ProductKindListViewModel
    {
        ProductKindListFilterModel m_filter = new ProductKindListFilterModel();
        ProductKindListResultModel m_result = new ProductKindListResultModel();

        public ProductKindListFilterModel Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public ProductKindListResultModel Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}