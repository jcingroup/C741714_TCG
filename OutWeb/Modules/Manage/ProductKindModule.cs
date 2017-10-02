using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Exceptions;
using OutWeb.Models;
using OutWeb.Models.Manage.ProductKindModels;
using OutWeb.Provider;
using OutWeb.Repositories;
using OutWeb.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Modules.Manage
{
    public class ProductKindModule : ListModuleService
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        public override void DoDeleteByID(int ID)
        {
            //取隸屬分類的產品數量
            int typeId = ID;
            var scCount = this.DB.WBPRODUCT.Where(o => o.MAP_PRODUCT_TP_ID == typeId).Count();
            if (scCount > 0)
                throw new ProductKindRelationExcption("「尚有產品被歸類在此分類，故無法刪除。」");

            var data = this.DB.WBPRODUCTTYPE.Where(s => s.ID == ID).FirstOrDefault();
            if (data == null)
                throw new Exception("[刪除產品分類] 查無此產品分類，可能已被移除");
            try
            {
                this.DB.WBPRODUCTTYPE.Remove(data);
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("[刪除最新消息]" + ex.Message);
            }
        }

        public override object DoGetDetailsByID(int ID)
        {
            ProductKindDetailsDataModel data = DB.WBPRODUCTTYPE.Select(s => new ProductKindDetailsDataModel
            {
                ID = s.ID,
                SortIndex = s.SR_SQ,
                TypeName = s.PRD_TP_NM,
                Status = s.PRD_TP_ST,
            }).Where(w => w.ID == ID).FirstOrDefault();
            return data;
        }

        public override object DoGetList<TFilter>(TFilter model, Language language)
        {
            ProductKindListFilterModel filterModel = (model as ProductKindListFilterModel);
            ProductKindListResultModel result = new ProductKindListResultModel();
            List<ProductKindListDataModel> data = new List<ProductKindListDataModel>();
            try
            {
                data = DB.WBPRODUCTTYPE.Select(s => new ProductKindListDataModel()
                {
                    ID = s.ID,
                    ProductKindName = s.PRD_TP_NM,
                    StatusCode = s.PRD_TP_ST,
                    Sort = s.SR_SQ,
                    Language = s.LANG_CD,
                    CreateDate = s.BUD_DT
                })
                 .ToList();

                //語系搜尋
                if (!language.Equals(Language.NotSet))
                {
                    this.ListFilterLanguage(language, ref data);
                }

                //關鍵字搜尋
                if (!string.IsNullOrEmpty(filterModel.QueryString))
                {
                    this.ListFilter(filterModel.QueryString, ref data);
                }

                //狀態搜尋
                if (!string.IsNullOrEmpty(filterModel.Status))
                {
                    this.ListStatusFilter(filterModel.Status, ref data);
                }

                //取隸屬分類的產品數量
                foreach (var d in data)
                {
                    int typeId = d.ID;
                    var scCount = this.DB.WBPRODUCT.Where(o => o.MAP_PRODUCT_TP_ID == typeId).Count();
                    d.ProductCount = scCount;
                }

                //排序
                this.ListSort(filterModel.SortColumn, filterModel.Status, ref data);
                PaginationResult pagination;
                //分頁
                this.ListPageList(filterModel.CurrentPage, ref data, out pagination);
                result.Pagination = pagination;
                result.Data = data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public override int DoSaveData(FormCollection form, Language language, int? ID = default(int?), List<HttpPostedFileBase> image = null, List<HttpPostedFileBase> images = null)
        {
            WBPRODUCTTYPE saveModel;

            if (!ID.HasValue)
            {
                saveModel = new WBPRODUCTTYPE();
                saveModel.BUD_ID = UserProvider.Instance.User.ID;
                saveModel.BUD_DT = DateTime.UtcNow.AddHours(8);
            }
            else
            {
                saveModel = this.DB.WBPRODUCTTYPE.Where(s => s.ID == ID).FirstOrDefault();
            }
            saveModel.PRD_TP_NM = form["typeName"];
            saveModel.PRD_TP_ST = form["status"];
            saveModel.SR_SQ = form["sortIndex"] == null ? 1 : form["sortIndex"] == string.Empty ? 1 : Convert.ToInt32(form["sortIndex"]);
            saveModel.UPD_DT = DateTime.UtcNow.AddHours(8);
            saveModel.UPD_USR_ID = UserProvider.Instance.User.ID;
            saveModel.LANG_CD = language.GetCode();

            if (ID.HasValue)
            {
                this.DB.Entry(saveModel).State = EntityState.Modified;
            }
            else
            {
                this.DB.WBPRODUCTTYPE.Add(saveModel);
            }
            this.DB.SaveChanges();
            int identityId = saveModel.ID;
            return identityId;
        }

        /// <summary>
        /// 語系過濾條件
        /// </summary>
        /// <param name="language"></param>
        /// <param name="data"></param>
        private void ListFilterLanguage(Language language, ref List<ProductKindListDataModel> data)
        {
            var r = data.Where(s => s.Language == language.GetCode()).ToList();
            data = r;
        }

        /// <summary>
        /// 列表關鍵字搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void ListFilter(string filterStr, ref List<ProductKindListDataModel> data)
        {
            var r = data.Where(s => s.ProductKindName.Contains(filterStr)).ToList();
            data = r;
        }

        /// <summary>
        /// 日期條件搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        //private void ListDateFilter(string beginDate, string endDate, ref List<ProductKindListDataModel> data)
        //{
        //    var r = data.Where(s => Convert.ToDateTime(s.PublishDateStr) >= Convert.ToDateTime(beginDate) && Convert.ToDateTime(s.PublishDateStr) <= Convert.ToDateTime(endDate)).ToList();
        //    data = r;
        //}

        /// <summary>
        /// 狀態搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void ListStatusFilter(string filterStatus, ref List<ProductKindListDataModel> data)
        {
            var r = data.Where(s => s.StatusCode == filterStatus).ToList();
            data = r;
        }

        /// <summary>
        /// 取出分頁資料
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="data"></param>
        private void ListPageList(int currentPage, ref List<ProductKindListDataModel> data, out PaginationResult pagination)
        {
            int pageSize = (int)PageSizeConfig.SIZE10;
            int startRow = (currentPage - 1) * pageSize;
            PaginationResult paginationResult = new PaginationResult()
            {
                CurrentPage = currentPage,
                DataCount = data.Count,
                PageSize = pageSize,
                FirstPage = 1,
                LastPage = Convert.ToInt32(Math.Ceiling((decimal)data.Count / pageSize))
            };
            pagination = paginationResult;
            var query = data.Skip(startRow).Take(pageSize).ToList();
            data = query;
        }

        /// <summary>
        /// 列表排序功能
        /// </summary>
        /// <param name="sortCloumn"></param>
        /// <param name="data"></param>
        private void ListSort(string sortCloumn, string status, ref List<ProductKindListDataModel> data)
        {
            switch (sortCloumn)
            {
                case "sortName/asc":
                    data = data.OrderBy(o => o.ProductKindName.ToString()).ThenByDescending(g => g.Sort).ToList();
                    break;

                case "sortName/desc":
                    data = data.OrderByDescending(o => o.ProductKindName.ToString()).ThenByDescending(g => g.Sort).ToList();
                    break;

                case "sortStatus/asc":
                    data = data.OrderBy(o => o.StatusCode).ThenByDescending(g => g.Sort).ToList();
                    break;

                case "sortStatus/desc":
                    data = data.OrderByDescending(o => o.StatusCode).ThenByDescending(g => g.Sort).ToList();
                    break;

                case "sortIndex/asc":
                    data = data.OrderBy(o => o.Sort).ThenByDescending(g => g.CreateDate).ToList();
                    break;

                case "sortIndex/desc":
                    data = data.OrderByDescending(o => o.Sort).ThenByDescending(g => g.CreateDate).ToList();
                    break;

                default:
                    data = data.OrderByDescending(o => o.Sort).ToList();
                    break;
            }
        }

        /// <summary>
        /// 取得產品分類下拉選單資料
        /// </summary>
        /// <param name="defualt"></param>
        /// <param name="isListMode">是否為列表模式</param>
        /// <param name="isDisplayFrontIsFalse">判斷分類是否已被停用</param>
        /// <returns></returns>
        public SelectList CreateProductKindDropList(int? defualt, bool isListMode = true, bool isDisplayFrontIsFalse = true)
        {

            List<SelectListItem> types = DB.WBPRODUCTTYPE.Where(o => isDisplayFrontIsFalse ? o.PRD_TP_ST == "Y" : true)
                .Select(s => new SelectListItem() { Value = s.ID.ToString(), Text = s.PRD_TP_NM })
                .ToList();
            string itemTxt = "";
            if (isListMode)
            {
                itemTxt = "全部";
                if (!defualt.HasValue)
                    defualt = 0;
            }
            else
            {
                itemTxt = "請選擇";
                if (!defualt.HasValue)
                    defualt = 0;
            }
            types.Insert(0, new SelectListItem() { Value = "0", Text = itemTxt });
            types.Where(o => o.Value == defualt.ToString()).FirstOrDefault().Selected = true;
            SelectList typeList = new SelectList(types, "Value", "Text", defualt);
            return typeList;
        }
    }
}