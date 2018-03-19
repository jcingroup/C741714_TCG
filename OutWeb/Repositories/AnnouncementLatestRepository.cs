using OutWeb.Entities;
using OutWeb.Models;
using OutWeb.Models.FrontModels.News.AnnouncementLatest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutWeb.Repositories
{
    public class AnnouncementLatestRepository
    {
        private string _connectionString { get; set; }
        private ConnectionRepository conRepo = new ConnectionRepository();

        public AnnouncementLatestRepository()
        {
            _connectionString = conRepo.GetEntityConnctionString();
        }


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
                .Where(s => s.STATUS == "Y" && s.IMG_KIND == "News" &&
                s.IMG_NO == (id.ToString()))
                .Select(s => s.IMG_FILE)
                .FirstOrDefault();
            }
            return imgStr;
        }

        /// <summary>
        /// 取分類
        /// </summary>
        /// <param name="id"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetNewsCateByID(int id, string langCode)
        {
            Dictionary<int, string> cate = new Dictionary<int, string>();
            using (var db = new TCGDB(_connectionString))
            {
                var source = db.NEWS_CATE
                  .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                   s.ID == id && s.STATUS == "Y")
                   .FirstOrDefault();
                if (source == null)
                    throw new Exception("無法取得新聞分類");

                cate.Add(source.ID, source.CATE_NAME);
            }
            return cate;
        }

        /// <summary>
        /// 內容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public AnnouncementLatestContent GetContentByID(int id, int typeID, string langCode)
        {
            AnnouncementLatestContent result = new AnnouncementLatestContent();
            using (var db = new TCGDB(_connectionString))
            {
                var sourceList = db.NEWS
                 .AsEnumerable()
                 .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                 s.STATUS != "D" && s.CATE_ID == typeID)
                 .OrderByDescending(o => o.SORT)
                 .OrderByDescending(s => s.N_DATE)
                 .ToList();

                var source = sourceList.Where(s => s.ID == id).FirstOrDefault();
                if (source == null)
                    throw new Exception("無法取得新聞內容,是否已被移除.");

                result.Data = new AnnouncementLatestData()
                {
                    ID = source.ID,
                    Title = source.N_TITLE,
                    Img = GetMainImg(source.ID),
                    PublishDateString = source.N_DATE.Value.ToString("yyyy-MM-dd"),
                    CateIDInfo = GetNewsCateByID((int)source.CATE_ID, langCode),
                    Content = source.N_DESC
                };
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
        public AnnouncementLatestResult GetList(AnnouncementLatestFilter filter, int? costomPageSize = null, string isIndex = null)
        {
            AnnouncementLatestResult result = new AnnouncementLatestResult();
            List<AnnouncementLatestData> data = new List<AnnouncementLatestData>();
            using (var db = new TCGDB(_connectionString))
            {
                try
                {
                    var source = db.NEWS
                        .AsEnumerable()
                        .Where(s => (string.IsNullOrEmpty(filter.LangCode) ? true : s.LANG_ID == filter.LangCode) &&
                        s.STATUS != "D" && (filter.TypeID == null ? true : s.CATE_ID == (int)filter.TypeID) &&
                         (string.IsNullOrEmpty(isIndex) ? true : s.IS_INDEX == isIndex))
                        .OrderByDescending(o => o.SORT)
                        .OrderByDescending(s => s.N_DATE)
                        .ToList();

                    foreach (var item in source)
                    {
                        AnnouncementLatestData temp = new AnnouncementLatestData()
                        {
                            ID = item.ID,
                            Title = item.N_TITLE,
                            Img = GetMainImg(item.ID),
                            CateIDInfo = GetNewsCateByID((int)item.CATE_ID, filter.LangCode),
                            PublishDateString = item.N_DATE.Value.ToString("yyyy-MM-dd"),
                            Content = item.N_DESC.RemoveHtmlAllTags()
                        };
                        data.Add(temp);
                    }

                    result.Data = data;
                    result = this.ListPagination(ref result, filter.CurrentPage, costomPageSize ?? Convert.ToInt32(PublicMethodRepository.GetConfigAppSetting("DefaultPageSize")));
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
        public AnnouncementLatestResult ListPagination(ref AnnouncementLatestResult model, int page, int pageSize)
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