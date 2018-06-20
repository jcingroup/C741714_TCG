using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Models;
using OutWeb.Models.FrontModels.SearchSiteModels;
using OutWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutWeb.Modules.FrontEnd
{
    public class SearchRepository
    {
        private string _connectionString { get; set; }
        private ConnectionRepository conRepo = new ConnectionRepository();

        public SearchRepository()
        {
            _connectionString = conRepo.GetEntityConnctionString();
        }

        /// <summary>
        /// 關於我們
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchAbout(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                Dictionary<int, List<int>> cateGroup = new Dictionary<int, List<int>>();
                cateGroup.Add(0, new List<int>() { 1, 10, 11 });
                cateGroup.Add(1, new List<int>() { 2, 12, 13 });
                cateGroup.Add(2, new List<int>() { 3, 14, 15 });
                cateGroup.Add(3, new List<int>() { 4, 16, 17 });
                cateGroup.Add(4, new List<int>() { 5, 18, 19 });
                cateGroup.Add(5, new List<int>() { 6, 20, 21 });
                cateGroup.Add(6, new List<int>() { 7, 22, 23 });
                cateGroup.Add(7, new List<int>() { 8, 24, 25 });
                cateGroup.Add(8, new List<int>() { 9, 26, 27 });

                var source = db.ABOUTUS
                .AsEnumerable()
                .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                s.STATUS == "Y" &&
                (s.C_TITLE == qry || s.C_DESC.RemoveHtmlAllTags() == qry ||
                s.C_TITLE.Contains(qry) || s.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                .ToList();

                foreach (var about in source)
                {
                    var cate = cateGroup.Where(s => s.Value.Contains((int)about.CATE_ID)).ToDictionary(d => d.Key, d => d.Value.ToList());
                    if (cate.Count == 0)
                        throw new Exception("無法取得對應ABOUTAS的分類,請聯絡系統管理員");
                    SearchListDataModel temp = new SearchListDataModel();
                    temp.Title = about.C_TITLE;
                    temp.Content = about.C_DESC.RemoveHtmlAllTags();
                    temp.UpDateTime = (DateTime)about.UPD_DT;
                    temp.BD_DTString = (DateTime)about.BD_DT;
                    temp.Sort = (about.SORT) ?? 0;
                    switch (cate.Keys.First())
                    {
                        case 0:
                            temp.LinkAddr = string.Format("/AboutUs/TCG?id={0}", about.ID);
                            break;

                        case 1:
                            temp.LinkAddr = string.Format("/AboutUs/Position?id={0}", about.ID);
                            break;

                        case 2:
                            temp.LinkAddr = string.Format("/AboutUs/Statement?id={0}", about.ID);
                            break;

                        case 3:
                            temp.LinkAddr = string.Format("/AboutUs/Law?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        case 4:
                            temp.LinkAddr = string.Format("/AboutUs/Law?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        case 5:
                            temp.LinkAddr = string.Format("/AboutUs/Law?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        case 6:
                            temp.LinkAddr = string.Format("/AboutUs/Organization?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        case 7:
                            temp.LinkAddr = string.Format("/AboutUs/Organization?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        case 8:
                            temp.LinkAddr = string.Format("/AboutUs/Organization?cate_id={0}&id={1}", about.CATE_ID, about.ID);
                            break;

                        default:
                            break;
                    }
                    model.Add(temp);
                }
            }
        }

        /// <summary>
        /// 教育專欄
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchEducation(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                var query = db.EDU
                .Join(db.EDU_DETAIL,
                m => m.ID.ToString(),
                d => d.CATE_ID,
                (main, details) => new { Main = main, Details = details })
                .AsEnumerable()
                .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.Details.LANG_ID == langCode) &&
                (s.Main.C_TITLE == (qry) || s.Main.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Main.C_TITLE.Contains(qry) || s.Main.C_DESC.RemoveHtmlAllTags().Contains(qry) ||

                s.Details.C_TITLE == (qry) || s.Details.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Details.C_TITLE.Contains(qry) || s.Details.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                .Where(w => w.Details.STATUS == "Y")
                .Where(x=>x.Main.STATUS == "Y")
                .OrderByDescending(d => d.Details.UPD_DT)
                .GroupBy(g => g.Main.C_TITLE)
                .ToList()
                .Select(o => new SearchListDataModel()
                {
                    ID = o.First().Details.ID,
                    Title = o.First().Main.C_TITLE,
                    Type = o.First().Main.ID,
                    Content = o.First().Details.C_DESC.RemoveHtmlAllTags(),
                    UpDateTime = (DateTime)o.First().Details.UPD_DT,
                    LinkAddr = string.Format("/AboutUs/EducationContent?eduTypeID={0}&ID={1}&pagingID={2}", o.First().Main.CATE_ID, o.First().Details.CATE_ID, o.First().Details.ID),
                    BD_DTString = (DateTime)o.First().Details.BD_DT,
                    Sort = o.First().Details.SORT??0,
                })
                .ToList();

                if (query.Count > 0)
                    model.AddRange(query);
            }
        }

        /// <summary>
        /// 加入台灣民政府
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchJoinUs(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                Dictionary<int, List<int>> cateGroup = new Dictionary<int, List<int>>();
                cateGroup.Add(0, new List<int>() { 1, 4, 5 });
                cateGroup.Add(1, new List<int>() { 2, 6, 7 });

                var source = db.JOINUS
                 .AsEnumerable()
                  .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                  s.STATUS == "Y" &&(
                   s.C_TITLE == qry || s.C_DESC.RemoveHtmlAllTags() == qry ||
                  s.C_TITLE.Contains(qry) || s.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                  .ToList();

                foreach (var join in source)
                {
                    var cate = cateGroup.Where(s => s.Value.Contains((int)join.CATE_ID)).ToDictionary(d => d.Key, d => d.Value.ToList());
                    if (cate.Count == 0)
                        throw new Exception("無法取得對應的JOINUS分類,請聯絡系統管理員");
                    SearchListDataModel temp = new SearchListDataModel();
                    temp.Title = join.C_TITLE;
                    temp.Content = join.C_DESC.RemoveHtmlAllTags();
                    temp.UpDateTime = (DateTime)join.UPD_DT;
                    temp.BD_DTString = (DateTime)join.BD_DT;
                    temp.Sort = (join.SORT) ?? 0;
                    switch (cate.Keys.First())
                    {
                        case 0:
                            temp.LinkAddr = string.Format("/JoinUs/Apply");
                            break;

                        case 1:
                            temp.LinkAddr = string.Format("/JoinUs/Consult?id={0}", join.ID);
                            break;

                        default:
                            break;
                    }
                    model.Add(temp);
                }
            }
        }

        /// <summary>
        /// 中央活動
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchEventLatest(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                var query = db.ACTIVITY
               .Join(db.ACTIVITY_DETAIL,
               m => m.ID.ToString(),
               d => d.CATE_ID,
               (main, details) => new { Main = main, Details = details })
               .AsEnumerable()
               .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.Details.LANG_ID == langCode) &&
                (s.Main.C_TITLE == (qry) || s.Main.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Main.C_TITLE.Contains(qry) || s.Main.C_DESC.RemoveHtmlAllTags().Contains(qry) ||

                s.Details.C_TITLE == (qry) || s.Details.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Details.C_TITLE.Contains(qry) || s.Details.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                .Where(w => w.Details.STATUS == "Y")
                .Where(x=>x.Main.STATUS == "Y")
                .OrderByDescending(d => d.Details.UPD_DT)
                .GroupBy(g => g.Main.C_TITLE)
                .ToList()
               .Select(o => new SearchListDataModel()
               {
                   ID = o.First().Details.ID,
                   Title = o.First().Main.C_TITLE,
                   Type = o.First().Main.ID,
                   Content = o.First().Details.C_DESC.RemoveHtmlAllTags(),
                   UpDateTime = (DateTime)o.First().Details.UPD_DT,
                   LinkAddr = string.Format("/News/EventLatestContent?ID={0}&pagingID={1}", o.First().Details.CATE_ID, o.First().Details.ID),
                   BD_DTString = (DateTime)o.First().Details.BD_DT,
                   Sort = o.First().Details.SORT ?? 0,
               })
               .ToList();

                if (query.Count > 0)
                    model.AddRange(query);
            }
        }

        /// <summary>
        /// 各州活動
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchStates(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                var query = db.STATES
                   .Join(db.STATES_DETAIL,
                   m => m.ID.ToString(),
                   d => d.CATE_ID,
                   (main, details) => new { Main = main, Details = details })
                   .AsEnumerable()
                   .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.Details.LANG_ID == langCode) &&
                   (s.Main.C_TITLE == (qry) || s.Main.C_DESC.RemoveHtmlAllTags() == qry ||
                   s.Main.C_TITLE.Contains(qry) || s.Main.C_DESC.RemoveHtmlAllTags().Contains(qry) ||

                   s.Details.C_TITLE == (qry) || s.Details.C_DESC.RemoveHtmlAllTags() == qry ||
                   s.Details.C_TITLE.Contains(qry) || s.Details.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                   .Where(w => w.Details.STATUS == "Y")
                   .Where(x=>x.Main.STATUS == "Y")
                   .OrderByDescending(d => d.Details.UPD_DT)
                   .GroupBy(g => g.Main.C_TITLE)
                   .ToList()
                   .Select(o => new SearchListDataModel()
                   {
                       ID = o.First().Details.ID,
                       Title = o.First().Main.C_TITLE,
                       Type = o.First().Main.ID,
                       Content = o.First().Details.C_DESC.RemoveHtmlAllTags(),
                       UpDateTime = (DateTime)o.First().Details.UPD_DT,
                       LinkAddr = string.Format("/News/EventStatesContent?statesTypeID={0}&ID=1&pagingID={1}", o.First().Details.CATE_ID, o.First().Details.ID),
                       BD_DTString = (DateTime)o.First().Details.BD_DT,
                       Sort = o.First().Details.SORT ?? 0,
                   })
                   .ToList();

                if (query.Count > 0)
                    model.AddRange(query);
            }
        }

        /// <summary>
        /// 新聞公告 聲明
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchAnnouncement(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                var source = db.NEWS
                 .AsEnumerable()
                    .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                  s.STATUS == "Y" &&
                   (s.N_TITLE == qry || s.N_DESC.RemoveHtmlAllTags() == qry ||
                  s.N_TITLE.Contains(qry) || s.N_DESC.RemoveHtmlAllTags().Contains(qry)))
                  .ToList();

                foreach (var news in source)
                {
                    SearchListDataModel temp = new SearchListDataModel();
                    temp.Title = news.N_TITLE;
                    temp.Content = news.N_DESC.RemoveHtmlAllTags();
                    temp.UpDateTime = (DateTime)news.UPD_DT;
                    temp.LinkAddr = string.Format("/News/AnnouncementContent?ID={0}&typeID={1}", news.ID, news.CATE_ID);
                    temp.BD_DTString = (DateTime)news.BD_DT;
                    temp.Sort = (news.SORT) ?? 0;
                    model.Add(temp);
                }
            }
        }

        /// <summary>
        /// 焦點專欄
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="langCode"></param>
        /// <param name="model"></param>
        private void SearchFocus(string qry, string langCode, ref List<SearchListDataModel> model)
        {
            using (var db = new TCGDB(_connectionString))
            {
                var query = db.FOCUS
                .Join(db.FOCUS_DETAIL,
                m => m.ID.ToString(),
                d => d.CATE_ID,
                (main, details) => new { Main = main, Details = details })
                .AsEnumerable()
                .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.Details.LANG_ID == langCode) &&
                (s.Main.C_TITLE == (qry) || s.Main.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Main.C_TITLE.Contains(qry) || s.Main.C_DESC.RemoveHtmlAllTags().Contains(qry) ||

                s.Details.C_TITLE == (qry) || s.Details.C_DESC.RemoveHtmlAllTags() == qry ||
                s.Details.C_TITLE.Contains(qry) || s.Details.C_DESC.RemoveHtmlAllTags().Contains(qry)))
                .Where(w => w.Details.STATUS == "Y")
                .Where(x=>x.Main.STATUS == "Y")
                .OrderByDescending(d => d.Details.UPD_DT)
                .GroupBy(g => g.Main.C_TITLE)
                .ToList()
                .Select(o => new SearchListDataModel()
                {
                    ID = o.First().Details.ID,
                    Title = o.First().Main.C_TITLE,
                    Type = o.First().Main.ID,
                    Content = o.First().Details.C_DESC.RemoveHtmlAllTags(),
                    UpDateTime = (DateTime)o.First().Details.UPD_DT,
                    LinkAddr = string.Format("/News/FocusContent?focusTypeID={0}&ID={1}&pagingID={2}", o.First().Main.CATE_ID, o.First().Details.CATE_ID, o.First().Details.ID),
                    BD_DTString = (DateTime)o.First().Details.BD_DT,
                    Sort = o.First().Details.SORT ?? 0,
                })
                .ToList();

                if (query.Count > 0)
                    model.AddRange(query);
            }
        }

        public SearchListResultModel SearchSite(SearchListFilterModel filter)
        {
            PublicMethodRepository.FilterXss(filter);

            string str = filter.QueryString;
            SearchListResultModel result = new SearchListResultModel();
            List<SearchListDataModel> data = new List<SearchListDataModel>();

            SearchAbout(filter.QueryString, filter.LangCode, ref data);
            SearchEducation(filter.QueryString, filter.LangCode, ref data);
            SearchJoinUs(filter.QueryString, filter.LangCode, ref data);
            SearchEventLatest(filter.QueryString, filter.LangCode, ref data);
            SearchStates(filter.QueryString, filter.LangCode, ref data);
            SearchAnnouncement(filter.QueryString, filter.LangCode, ref data);
            SearchFocus(filter.QueryString, filter.LangCode, ref data);

            //result.Data = data.OrderByDescending(o => o.UpDateTime).ToList();
            result.Data = data.OrderByDescending(o => o.Sort).ThenByDescending(x => x.BD_DTString).ToList();
            result = ListPagination(result, (int)filter.CurrentPage, (int)PageSizeConfig.SIZE10);

            return result;
        }

        public SearchListResultModel ListPagination(SearchListResultModel model, int page, int pageSize)
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
                //若搜尋頁數超過最大頁數，則強制變為搜索最大頁數
                if (page > paginationResult.LastPage)
                {
                    paginationResult.CurrentPage = paginationResult.LastPage;
                    startRow = (paginationResult.CurrentPage - 1) * pageSize;
                }
            }
            model.Data = model.Data.Skip(startRow).Take(pageSize).ToList();
            model.Pagination = paginationResult;
            return model;
        }
    }
}