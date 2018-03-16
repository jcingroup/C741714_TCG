using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models
{
    public interface IPaginationModel
    {
        /// <summary>
        /// 分頁
        /// </summary>
        PaginationResult Pagination { get; set; }
    }
}