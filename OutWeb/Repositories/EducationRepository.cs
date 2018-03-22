﻿using OutWeb.Entities;
using OutWeb.Models;
using OutWeb.Models.FrontModels.News;
using OutWeb.Models.FrontModels.News.EventLatestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OutWeb.Models.FrontModels.AboutModels.EducationModels;

namespace OutWeb.Repositories
{
    public class EducationRepository
    {
        private string _connectionString { get; set; }
        private ConnectionRepository conRepo = new ConnectionRepository();

        public EducationRepository()
        {
            _connectionString = conRepo.GetEntityConnctionString();
        }

        /// <summary>
        /// 全德各州所有分類
        /// </summary>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetEducationCate(string langCode)
        {
            Dictionary<int, string> cate = new Dictionary<int, string>();

            using (var db = new TCGDB(_connectionString))
            {
                cate = db.EDU_CATE
                 .Where(s => (string.IsNullOrEmpty(langCode) ? true : s.LANG_ID == langCode) &&
                 s.STATUS == "Y")
                  .ToDictionary(d => d.ID, d => d.CATE_NAME);
                if (cate.Count == 0)
                    throw new Exception("無法取得教育分類");
            }
            return cate;
        }

        /// <summary>
        /// 取得洲別分類
        /// </summary>
        /// <param name="id"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetEduCateByID(int id, string langCode)
        {
            Dictionary<int, string> cate = GetEducationCate(langCode)
                .Where(s => s.Key == id)
                .ToDictionary(d => d.Key, d => d.Value);

            if (cate.Count == 0)
                throw new Exception("無法取得教育分類");

            return cate;
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
                data = db.EDU_DETAIL
                    .AsEnumerable()
                        .Where(o => o.CATE_ID == id.ToString() && o.STATUS == "Y")
                        .OrderByDescending(o => o.SORT)
                        .Select(s => new EvnentPaging()
                        {
                            ID = s.ID,
                            Title = s.C_TITLE,
                            Description = s.C_DESC,
                            //ImagesList = GetPaginImgsListByID(s.ID),
                            //InternetSiteList = GetPagingUrlListByID(s.ID)
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
        /// 取得第一個分頁的描述內容（去除Html Tag）
        /// </summary>
        /// <param name="pagingList"></param>
        /// <returns></returns>
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
        public EducationContent GetContentByID(int statesTypeID, int id, string lagCode)
        {
            EducationContent result = new EducationContent();
            using (var db = new TCGDB(_connectionString))
            {
                var sourceList = db.EDU
                 .AsEnumerable()
                 .Where(s => (string.IsNullOrEmpty(lagCode) ? true : s.LANG_ID == lagCode) &&
                 s.STATUS == "Y" && s.CATE_ID == statesTypeID)
                 .OrderByDescending(o => o.SORT)
                 .OrderByDescending(s => s.C_DATE)
                 .ToList();

                var source = sourceList.Where(s => s.ID == id).FirstOrDefault();
                if (source == null)
                    throw new Exception("無法取得活動內容,是否已被移除.");


                result.Data = new EducationData()
                {
                    ID = source.ID,
                    Title = source.C_TITLE,
                    PagingList = GetPagingListByID(source.ID),
                    PublishDateString = source.C_DATE.Value.ToString("yyyy-MM-dd"),
                };
                result.Data.Remark = GetFirstPagingRemark(result.Data.PagingList);
                int dataIndex = sourceList.IndexOf(source);
                int lastDataIndex = sourceList.Count - 1;
                result.PreviousIDStr = dataIndex == 0 ? "" : sourceList[(dataIndex - 1)].ID.ToString();
                result.NextIDStr = dataIndex == lastDataIndex ? "" : sourceList[(dataIndex + 1)].ID.ToString();
            }
            result.EduCateInfo = GetEduCateByID(statesTypeID, lagCode);
            return result;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public EducationResult GetList(int eduTypeID, EducationListFilter filter, int? coustomPageSize = null, string isIndex = null)
        {
            EducationResult result = new EducationResult();
            List<EducationData> data = new List<EducationData>();
            using (var db = new TCGDB(_connectionString))
            {
                try
                {
                    var source = db.EDU
                        .AsEnumerable()
                        .Where(s => (string.IsNullOrEmpty(filter.LangCode) ? true : s.LANG_ID == filter.LangCode) &&
                        s.STATUS == "Y" && s.CATE_ID == eduTypeID)
                        .OrderByDescending(o => o.SORT)
                        .OrderByDescending(s => s.C_DATE)
                        .ToList();

                    foreach (var item in source)
                    {
                        EducationData temp = new EducationData()
                        {
                            ID = item.ID,
                            Title = item.C_TITLE,
                            PagingList = GetPagingListByID(item.ID),
                            PublishDateString = item.C_DATE.Value.ToString("yyyy-MM-dd"),
                        };
                        temp.Remark = GetFirstPagingRemark(temp.PagingList);
                        data.Add(temp);
                    }

                    result.Data = data;
                    result = this.ListPagination(ref result, filter.CurrentPage, coustomPageSize ?? Convert.ToInt32(PublicMethodRepository.GetConfigAppSetting("DefaultPageSize")));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            result.EduTypeInfo = GetEduCateByID(eduTypeID, filter.LangCode);
            return result;
        }

        /// <summary>
        /// [前台] 列表分頁處理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public EducationResult ListPagination(ref EducationResult model, int page, int pageSize)
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