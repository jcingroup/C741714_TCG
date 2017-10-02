using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Models;
using OutWeb.Models.Manage.ImgModels;
using OutWeb.Models.Manage.ManageNewsModels;
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
    /// <summary>
    /// 最新消息列表模組
    /// </summary>
    public class NewsModule : ListModuleService
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        public override void DoDeleteByID(int ID)
        {
            var data = this.DB.WBNEWS.Where(s => s.ID == ID).FirstOrDefault();
            if (data == null)
                throw new Exception("[刪除最新消息] 查無此最新消息，可能已被移除");
            try
            {
                this.DB.WBNEWS.Remove(data);
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("[刪除最新消息]" + ex.Message);
            }
        }

        public override object DoGetDetailsByID(int ID)
        {
            NewsDetailsDataModel data = DB.WBNEWS.Select(s => new NewsDetailsDataModel
            {
                ID = s.ID,
                Title = s.NEWS_TITLE,
                PublishDateStr = s.PUB_DT,
                DisplayForFront = (bool)s.DIS_FRONT_ST,
                DisplayForHomePage = (bool)s.DIS_HOME_ST,
                SortIndex = s.SR_SQ,
                NewsContent = s.NEWS_CONTENT
            }).Where(w => w.ID == ID).FirstOrDefault();
            return data;
        }

        public override object DoGetList<TFilter>(TFilter model, Language language)
        {
            NewsListFilterModel filterModel = (model as NewsListFilterModel);
            NewsListResultModel result = new NewsListResultModel();
            List<NewsListDataModel> data = new List<NewsListDataModel>();
            try
            {
                data = DB.WBNEWS.Select(s => new NewsListDataModel()
                {
                    ID = s.ID,
                    Title = s.NEWS_TITLE,
                    PublishDateStr = s.PUB_DT,
                    DisplayForFront = (bool)s.DIS_FRONT_ST,
                    DisplayForHomePage = (bool)s.DIS_HOME_ST,
                    Sort = s.SR_SQ,
                    Language = s.LANG_CD
                })
                 .ToList();
                //語系搜尋
                if (!language.Equals(Language.NotSet))
                {
                    this.NewsListFilterLanguage(language, ref data);
                }
                //關鍵字搜尋
                if (!string.IsNullOrEmpty(filterModel.QueryString))
                {
                    this.NewsListFilter(filterModel.QueryString, ref data);
                }
                //發佈日期搜尋
                if (!string.IsNullOrEmpty(filterModel.PublishDate))
                {
                    this.NewsListDateFilter(filterModel.PublishDate, ref data);
                }

                //前台顯示
                if (!string.IsNullOrEmpty(filterModel.DisplayForFrontEnd))
                {
                    this.NewsListStatusFilter(filterModel.DisplayForFrontEnd, "F", ref data);
                }

                //首頁顯示
                if (!string.IsNullOrEmpty(filterModel.DisplayForHomePage))
                {
                    this.NewsListStatusFilter(filterModel.DisplayForHomePage, "H", ref data);
                }

                //排序
                this.NewsListSort(filterModel.SortColumn, filterModel.Status, ref data);
                PaginationResult pagination;
                //分頁
                this.NewsListPageList(filterModel.CurrentPage, ref data, out pagination);
                result.Pagination = pagination;
                result.Data = data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public override int DoSaveData(FormCollection form, Language language, int? ID = null, List<HttpPostedFileBase> image = null, List<HttpPostedFileBase> images = null)
        {
            WBNEWS saveModel;
            ImageRepository imgepository = new ImageRepository();

            if (!ID.HasValue)
            {
                saveModel = new WBNEWS();
                saveModel.BUD_ID = UserProvider.Instance.User.ID;
                saveModel.BUD_DT = DateTime.UtcNow.AddHours(8);
            }
            else
            {
                saveModel = this.DB.WBNEWS.Where(s => s.ID == ID).FirstOrDefault();
            }

            saveModel.NEWS_TITLE = form["title"];
            saveModel.DIS_FRONT_ST = form["fSt"] == null ? false : true;
            saveModel.DIS_HOME_ST = form["hSt"] == null ? false : true;
            saveModel.SR_SQ = form["sortIndex"] == null ? 1 : form["sortIndex"] == string.Empty ? 1 : Convert.ToInt32(form["sortIndex"]);
            saveModel.NEWS_CONTENT = form["contenttext"];
            saveModel.PUB_DT = form["publishdate"];
            saveModel.UPD_DT = DateTime.UtcNow.AddHours(8);
            saveModel.UPD_ID = UserProvider.Instance.User.ID;
            saveModel.LANG_CD = language.GetCode();

            if (ID.HasValue)
            {
                this.DB.Entry(saveModel).State = EntityState.Modified;
            }
            else
            {
                this.DB.WBNEWS.Add(saveModel);
            }


            try
            {
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            int identityId = saveModel.ID;


            #region 圖片處理

            List<int> oldImgList = new List<int>();

            #region 將原存在的Server圖片保留 記錄圖片ID

            //將原存在的Server圖片保留 記錄圖片ID
            foreach (var f in form.Keys)
            {
                if (f.ToString().StartsWith("MemberData"))
                {
                    var id = Convert.ToInt16(form[f.ToString().Split('.')[0] + ".ID"]);
                    if (!oldImgList.Contains(id))
                        oldImgList.Add(id);
                }
            }

            #endregion 將原存在的Server圖片保留 記錄圖片ID

            #region 建立圖片模型

            ImagesModel imgModel = new ImagesModel()
            {
                ActionName = "News",
                ID = identityId,
                OldImageIds = oldImgList
            };

            #endregion 建立圖片模型

            #region 不知道為什麼 前端由js加入的formdata file類型 會多出null 砍了就對了

            if (image != null)
            {
                if (image.Count > 0)
                    image.RemoveAll(item => item == null);
            }

            #endregion 不知道為什麼 前端由js加入的formdata file類型 會多出null 砍了就對了

            #region img data binding 單筆多筆裝在不同容器

            imgModel.UploadType = "images_s";
            imgepository.UploadPhoto("Post", imgModel, image, "S");
            imgepository.SaveImagesToDB(imgModel);

            #endregion img data binding 單筆多筆裝在不同容器

            #endregion 圖片處理

            return identityId;
        }

        private void NewsListFilterLanguage(Language language, ref List<NewsListDataModel> data)
        {
            var r = data.Where(s => s.Language == language.GetCode()).ToList();
            data = r;
        }

        /// <summary>
        /// 列表關鍵字搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void NewsListFilter(string filterStr, ref List<NewsListDataModel> data)
        {
            var r = data.Where(s => s.Title.Contains(filterStr)).ToList();
            data = r;
        }

        /// <summary>
        /// 日期條件搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void NewsListDateFilter(string publishdate, ref List<NewsListDataModel> data)
        {
            var r = data.Where(s => s.PublishDateStr == publishdate).ToList();
            data = r;
        }

        /// <summary>
        /// 前台顯示搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void NewsListStatusFilter(string status, string displayMode, ref List<NewsListDataModel> data)
        {
            List<NewsListDataModel> result = null;
            if (displayMode == "F")
            {
                if (status == "Y")
                    result = data.Where(s => s.DisplayForFront == true).ToList();
                else
                    result = data.Where(s => s.DisplayForFront == false).ToList();
            }
            else if (displayMode == "H")
                if (status == "Y")
                    result = data.Where(s => s.DisplayForHomePage == true).ToList();
                else
                    result = data.Where(s => s.DisplayForHomePage == false).ToList();
            data = result;
        }

        /// <summary>
        /// 取出分頁資料
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="data"></param>
        private void NewsListPageList(int currentPage, ref List<NewsListDataModel> data, out PaginationResult pagination)
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
        private void NewsListSort(string sortCloumn, string status, ref List<NewsListDataModel> data)
        {
            switch (sortCloumn)
            {
                case "sortPublishDate/asc":
                    data = data.OrderBy(o => o.PublishDateStr).ToList();
                    break;

                case "sortPublishDate/desc":
                    data = data.OrderByDescending(o => o.PublishDateStr).ToList();
                    break;

                case "sortDisplayForFront/asc":
                    data = data.OrderBy(o => o.DisplayForFront).ToList();
                    break;

                case "sortDisplayForFront/desc":
                    data = data.OrderByDescending(o => o.DisplayForFront).ToList();
                    break;

                case "sortDisplayForHome/asc":
                    data = data.OrderBy(o => o.DisplayForHomePage).ThenByDescending(g => g.PublishDateStr).ToList();
                    break;

                case "sortDisplayForHome/desc":
                    data = data.OrderByDescending(o => o.DisplayForHomePage).ThenByDescending(g => g.PublishDateStr).ToList();
                    break;

                case "sortIndex/asc":
                    data = data.OrderBy(o => o.Sort).ThenByDescending(g => g.PublishDateStr).ToList();
                    break;

                case "sortIndex/desc":
                    data = data.OrderByDescending(o => o.Sort).ThenByDescending(g => g.PublishDateStr).ToList();
                    break;

                default:
                    data = data.OrderByDescending(o => o.Sort).ThenByDescending(g => g.PublishDateStr).ToList();
                    break;
            }
        }
    }
}