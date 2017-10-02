using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Models;
using OutWeb.Models.FrontEnd.NewsFrontEndModels;
using OutWeb.Models.FrontEnd.ProductFrontEndModels;
using OutWeb.Models.FrontEnd.WorksFrontEndModels;
using OutWeb.Models.Manage.ProductKindModels;
using OutWeb.Modules.Manage;
using OutWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OutWeb.Modules.FontEnd
{
    public class FontEndModule : IDisposable
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        #region 模組化功能

        /// <summary>
        /// [前台] 列表分頁處理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public dynamic ListPageFrontEnd<T>(IEnumerable<T> data, int page, int pageSize, ListMethodType methodType)
        {
            int startRow = 0;
            PaginationResult paginationResult = null;
            if (pageSize > 0)
            {
                //分頁
                startRow = (page - 1) * pageSize;
                paginationResult = new PaginationResult()
                {
                    CurrentPage = page,
                    DataCount = data.Count(),
                    PageSize = pageSize,
                    FirstPage = 1,
                    LastPage = data.Count() == 0 ? 1 : Convert.ToInt32(Math.Ceiling((decimal)data.Count() / pageSize))
                };
            }

            dynamic result = null;
            switch (methodType)
            {
                case ListMethodType.NotSet:
                    break;

                case ListMethodType.NEWS:
                    result = new NewsFrontEndListDataModel();
                    result.Data = data.Skip(startRow).Take(pageSize).ToList();
                    break;

                //case ListMethodType.FEEDBACK:
                //    result = new FeedbackFrontEndListDataModel();
                //    result.Data = data.Skip(startRow).Take(pageSize).ToList();
                //    break;

                case ListMethodType.PRODUCT:
                    result = new ProductFrontEndListDataModel();
                    result.Data = data.ToList();
                    break;

                default:
                    break;
            }
            result.Pagination = paginationResult;
            return result;
        }

        #endregion 模組化功能

        #region 前台

        #region 最新消息

        /// <summary>
        /// [前台]取得最新消息列表
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        public List<NewsFrontEndDataModel> GetNewsListFrontEnd(int dataCount)
        {
            List<NewsFrontEndDataModel> data;
            try
            {
                data = DB.WBNEWS.Select(s => new NewsFrontEndDataModel()
                {
                    ID = s.ID,
                    SortIndex = s.SR_SQ,
                    Title = s.NEWS_TITLE,
                    PartContent = s.NEWS_CONTENT,
                    PublishDateStr = s.PUB_DT,
                    DisplayForFront = (bool)s.DIS_FRONT_ST,
                    DisplayForHomePage = (bool)s.DIS_HOME_ST,
                })
                 .ToList();
                //篩選+排序
                if (dataCount > 0)
                {
                    data = data
                                  .Where(s => s.DisplayForHomePage)
                                  .OrderByDescending(o => o.SortIndex)
                                  .ThenByDescending(o => o.PublishDateStr)
                                   .Take(dataCount)
                                   .ToList();
                }
                else
                {
                    data = data
                         .Where(s => s.DisplayForFront)
                         .OrderByDescending(o => o.SortIndex)
                         .ThenByDescending(o => o.PublishDateStr)
                         .ToList();
                }

                foreach (var d in data)
                {
                    //取圖檔
                    ImgModule imgModule = new ImgModule();
                    d.ImagesData = imgModule.GetImages(d.ID, "News", "S").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }

        /// <summary>
        /// [前台]取得最新消息內文
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public NewsFrontEndDetalisDataModel GetNewsByIDFrontEnd(int ID)
        {
            NewsFrontEndDetalisDataModel data = DB.WBNEWS.Select(s => new NewsFrontEndDetalisDataModel
            {
                ID = s.ID,
                Title = s.NEWS_TITLE,
                PublishDateStr = s.PUB_DT,
                NewsContent = s.NEWS_CONTENT
            }).Where(w => w.ID == ID).FirstOrDefault();
            return data;
        }

        #endregion 最新消息

        #region 產品

        /// <summary>
        /// [前台]取得最新消息列表
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        public List<ProductFrontEndDataModel> GetProductListFrontEnd()
        {
            List<ProductFrontEndDataModel> data;
            try
            {
                data = DB.WBPRODUCT.Select(s => new ProductFrontEndDataModel()
                {
                    ID = s.ID,
                    ProductName = s.PRD_NM,
                    ProductKindID = s.MAP_PRODUCT_TP_ID,
                    Sort = s.SR_SQ,
                    ProductType = s.PRD_TP,
                    ProductFeatures = s.PRD_FEAT.Replace("\r\n", "<br>"),
                    ProductMaterial = s.PRD_MT,
                    ProductSpecification = s.PRD_SPE,
                    Content = s.PRD_CONTENT,
                    DisplayForFrontEnd = (bool)s.DIS_FRONT_ST,
                })
                 .ToList();
                //篩選+排序

                data = data
               .Where(s => s.DisplayForFrontEnd)
               .OrderByDescending(o => o.Sort)
                .ToList();

                foreach (var d in data)
                {
                    var kind = this.DB.WBPRODUCTTYPE.Where(o => o.ID == d.ProductKindID).FirstOrDefault();
                    d.ProductKindName = kind == null ? "" : kind.PRD_TP_NM;
                    //取圖檔
                    ImgModule imgModule = new ImgModule();
                    d.ImagesData = imgModule.GetImages(d.ID, "Product", "S").FirstOrDefault();
                    d.OtherImagesData = imgModule.GetImages(d.ID, "Product", "M");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }

        /// <summary>
        /// [前台]取得產品分類內文
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductFrontEndDataModel GetProductByIDFrontEnd(int ID)
        {
            var details = GetProductListFrontEnd();
            ProductFrontEndDataModel result = details.Where(o => o.ID == ID).First();
            return result;
        }

        #endregion 產品

        #region 案例分享

        /// <summary>
        /// [前台]取得最新消息列表
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        public List<WorksFrontEndDataModel> GetWorksListFrontEnd()
        {
            List<WorksFrontEndDataModel> data;
            try
            {
                data = DB.WBWORKS.Select(s => new WorksFrontEndDataModel()
                {
                    ID = s.ID,
                    WorksPulishDateStr = s.PUB_DT,
                    WorksTitle = s.WKS_TITLE,
                    Sort = s.SR_SQ,
                    Content = s.WKS_CONTENT,
                    DisplayForFrontEnd = (bool)s.DIS_FRONT_ST
                })
                 .ToList();
                //篩選+排序

                data = data
               .Where(s => s.DisplayForFrontEnd)
               .OrderByDescending(o => o.Sort)
                .ToList();

                foreach (var d in data)
                {
                    //取圖檔
                    ImgModule imgModule = new ImgModule();
                    d.ImagesData = imgModule.GetImages(d.ID, "Works", "S").FirstOrDefault();
                    d.OtherImagesData = imgModule.GetImages(d.ID, "Works", "M");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }

        /// <summary>
        /// [前台]取得產品分類內文
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public WorksFrontEndDataModel GetWorksByIDFrontEnd(int ID)
        {
            var details = GetWorksListFrontEnd();
            WorksFrontEndDataModel result = details.Where(o => o.ID == ID).First();
            return result;
        }

        #endregion 產品
        #endregion 前台

        public void Dispose()
        {
            this.Dispose();
        }
    }
}