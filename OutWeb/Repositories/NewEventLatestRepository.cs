using OutWeb.Entities;
using OutWeb.Models;
using OutWeb.Models.FrontModels.News;
using OutWeb.Models.FrontModels.News.EventLatestModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutWeb.Repositories
{
    public class NewEventLatestRepository
    {
        private string _connectionString { get; set; }
        private ConnectionRepository conRepo = new ConnectionRepository();

        public NewEventLatestRepository()
        {
            _connectionString = conRepo.GetEntityConnctionString();
        }

        //dbContext.PTSignWorkflow
        //     .Join(dbContext.PTSignWorkflowDetails,
        //     t1 => t1.ID,
        //     t2 => t2.MAP_SIGN_WKF_ID
        //(main, details) => new { Main = main, Details = details })
        //.AsEnumerable()
        //.Where(o => o.Main.MAP_SIGN_ID == signID)
        //.OrderBy(o => o.Details.SIGN_SORT)
        //.Select(s => s.Details.SIGN_DEP_CD)
        //.FirstOrDefault();

        /// <summary>
        /// 取得分頁的網址列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<string> GetPagingUrlListByID(int id)
        {
            List<string> data = new List<string>();

            using (var db = new TCGDB(_connectionString))
            {
                data = db.URL
                    .Where(s => s.STATUS == "Y" && s.URL_KIND == "Activity_Detail" &&
                    s.URL_NO.Contains(id.ToString()))
                    .OrderByDescending(s => s.SORT)
                    .Select(s => s.C_URL)
                    .ToList();
            }

            return data;
        }

        /// <summary>
        /// 取得分頁的圖片列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<PagingImageInfo> GetPaginImgsListByID(int id)
        {
            List<PagingImageInfo> data = new List<PagingImageInfo>();

            using (var db = new TCGDB(_connectionString))
            {
                data = db.IMG
                                .Where(s => s.STATUS == "Y" && s.IMG_STY == "B" && s.IMG_KIND == "Activity_Detail" &&
                                s.IMG_NO.Contains(id.ToString()))
                                .OrderByDescending(s => s.SORT)
                                .Select(s => new PagingImageInfo()
                                {
                                    ImgFileName = s.IMG_FILE,
                                    ImgDescription = s.IMG_DESC
                                })
                                .ToList();
            }
            return data;
        }

        /// <summary>
        /// 取得分頁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        private List<EvnentPaging> GetPagingListByID(int id)
        {
            List<EvnentPaging> data = new List<EvnentPaging>();

            using (var db = new TCGDB(_connectionString))
            {
                data = db.ACTIVITY_DETAIL
                    .AsEnumerable()
                        .Where(o => o.CATE_ID == id.ToString())
                        .OrderBy(o => o.SORT)
                        .Select(s => new EvnentPaging()
                        {
                            Title = s.C_TITLE,
                            Description = s.C_DESC,
                            ImagesList = GetPaginImgsListByID(s.ID),
                            InternetSiteList = GetPagingUrlListByID(s.ID)
                        })
                .ToList();
                //語系join
                //    data = db.ACTIVITY_DETAIL
                //         .Join(db.LANG,
                //     t1 => t1.LANG_ID,
                //     t2 => t2.LANG_ID,
                //(details, lang) => new { Details = details, Lang = lang })
                //.AsEnumerable()
                //.Where(o => o.Details.CATE_ID == id.ToString())
                //.OrderBy(o => o.Details.SORT)
                //.Select(s => new EnentPaging()
                //{
                //    Title = s.Details.C_TITLE,
                //    Description = s.Details.C_DESC,
                //    ImagesList = GetPagingImgOrUrlListByID(s.Details.ID, DetailsContentKind.img),
                //    InternetSiteList = GetPagingImgOrUrlListByID(s.Details.ID, DetailsContentKind.url)
                //})
                //.ToList();
            }

            return data;
        }

        /// <summary>
        /// 文章代表圖
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetMainImg(int id)
        {
            string imgStr = string.Empty;
            using (var db = new TCGDB(_connectionString))
            {
                imgStr = db.IMG
                .Where(s => s.STATUS == "Y" && s.IMG_KIND == "Activity" &&
                s.IMG_NO == (id.ToString()))
                .Select(s => s.IMG_FILE)
                .FirstOrDefault();
            }
            return imgStr;
        }

        private string GetFirstPagingRemark(List<EvnentPaging> pagingList)
        {
            if (pagingList.Count == 0)
                return string.Empty;
            if (string.IsNullOrEmpty(pagingList.First().Description))
                return string.Empty;

            string remark = pagingList.First().Description.RemoveHtmlAllTags();

            return remark;
        }

        /// <summary>
        /// 內容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lagCode"></param>
        /// <returns></returns>
        public EventContent GetContentByID(int id, string lagCode)
        {
            EventContent result = new EventContent();
            using (var db = new TCGDB(_connectionString))
            {
                var sourceList = db.ACTIVITY
                 .AsEnumerable()
                 .Where(s => (string.IsNullOrEmpty(lagCode) ? true : s.LANG_ID == lagCode) &&
                 s.STATUS != "D")
                 .OrderByDescending(o => o.SORT)
                 .OrderByDescending(s => s.C_DATE)
                 .ToList();

                var source = sourceList.Where(s => s.ID == id).FirstOrDefault();
                if (source == null)
                    throw new Exception("無法取得活動內容,是否已被移除.");

                result.Data = new EventLatestData()
                {
                    ID = source.ID,
                    Title = source.C_TITLE,
                    Img = GetMainImg(source.ID),
                    PagingList = GetPagingListByID(source.ID),
                    PublishDateString = source.C_DATE.Value.ToString("yyyy-MM-dd"),
                };
                result.Data.Remark = GetFirstPagingRemark(result.Data.PagingList);
                int dataIndex = sourceList.IndexOf(source);
                int lastDataIndex = sourceList.Count - 1;
                result.PreviousIDStr = dataIndex == 0 ? "" : sourceList[(dataIndex - 1)].ID.ToString();
                result.NextIDStr = dataIndex == lastDataIndex ? "" : sourceList[(dataIndex + 1)].ID.ToString();
            }

            return result;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public EventLatestResult GetList(EventLatestListFilter filter)
        {
            EventLatestResult result = new EventLatestResult();
            List<EventLatestData> data = new List<EventLatestData>();
            using (var db = new TCGDB(_connectionString))
            {
                try
                {
                    var source = db.ACTIVITY
                        .AsEnumerable()
                        .Where(s => (string.IsNullOrEmpty(filter.LangCode) ? true : s.LANG_ID == filter.LangCode) &&
                        s.STATUS != "D")
                        .OrderByDescending(o => o.SORT)
                        .OrderByDescending(s => s.C_DATE)
                        .ToList();

                    foreach (var item in source)
                    {
                        EventLatestData temp = new EventLatestData()
                        {
                            ID = item.ID,
                            Title = item.C_TITLE,
                            Img = GetMainImg(item.ID),
                            PagingList = GetPagingListByID(item.ID),
                            PublishDateString = item.C_DATE.Value.ToString("yyyy-MM-dd"),
                        };
                        temp.Remark = GetFirstPagingRemark(temp.PagingList);
                        data.Add(temp);
                    }

                    result.Data = data;
                    result = this.ListPagination(ref result, filter.CurrentPage, Convert.ToInt32(PublicMethodRepository.GetConfigAppSetting("DefaultPageSize")));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// [前台] 列表分頁處理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public EventLatestResult ListPagination(ref EventLatestResult model, int page, int pageSize)
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
                    DataCount = model.Data.Count(),
                    PageSize = pageSize,
                    FirstPage = 1,
                    LastPage = model.Data.Count() == 0 ? 1 : Convert.ToInt32(Math.Ceiling((decimal)model.Data.Count() / pageSize))
                };
            }
            model.Data = model.Data.Skip(startRow).Take(pageSize).ToList();
            model.Pagination = paginationResult;
            return model;
        }
    }
}