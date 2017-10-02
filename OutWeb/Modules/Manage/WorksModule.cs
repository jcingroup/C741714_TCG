using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Models;
using OutWeb.Models.Manage.ImgModels;
using OutWeb.Models.Manage.WorksModels;
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
    public class WorksModule : ListModuleService
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        public override void DoDeleteByID(int ID)
        {
            var img = this.DB.WBPIC.Where(s => s.MAP_WORKS_ID == ID).FirstOrDefault();
            var data = this.DB.WBWORKS.Where(s => s.ID == ID).FirstOrDefault();
            if (data == null)
                throw new Exception("[刪除案例] 查無此案例，可能已被移除");
            try
            {
                if (img != null)
                    this.DB.WBPIC.Remove(img);
                this.DB.WBWORKS.Remove(data);
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("[刪除案例]" + ex.Message);
            }
        }

        public override object DoGetDetailsByID(int ID)
        {
            WorksDetailsDataModel data = DB.WBWORKS.Select(s => new WorksDetailsDataModel
            {
                ID = s.ID,
                WorksPulishDateStr = s.PUB_DT,
                WorksTitle = s.WKS_TITLE,
                Sort = s.SR_SQ,
                Content = s.WKS_CONTENT,
                DisplayForFrontEnd = (bool)s.DIS_FRONT_ST
            }).Where(w => w.ID == ID).FirstOrDefault();
            return data;
        }

        public override object DoGetList<TFilter>(TFilter model, Language language)
        {
            WorksListFilterModel filterModel = (model as WorksListFilterModel);
            WorksListResultModel result = new WorksListResultModel();
            List<WorksListDataModel> data = new List<WorksListDataModel>();
            try
            {
                data = DB.WBWORKS.Select(s => new WorksListDataModel()
                {
                    ID = s.ID,
                    PublishDateStr = s.PUB_DT,
                    WorksName = s.WKS_TITLE,
                    Sort = s.SR_SQ,
                    Language = s.LANG_CD,
                    DisplayForFront = (bool)s.DIS_FRONT_ST
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

                //發佈日期搜尋
                if (!string.IsNullOrEmpty(filterModel.PublishDate))
                {
                    this.ListDateFilter(filterModel.PublishDate, ref data);
                }

                //前台顯示
                if (!string.IsNullOrEmpty(filterModel.DisplayForFrontEnd))
                {
                    this.ListStatusFilter(filterModel.DisplayForFrontEnd, ref data);
                }

                //前台顯示
                this.ListSort(filterModel.SortColumn, filterModel.DisplayForFrontEnd, ref data);
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
            WBWORKS saveModel;
            ImageRepository imgepository = new ImageRepository();

            if (!ID.HasValue)
            {
                saveModel = new WBWORKS();
                saveModel.BUD_USRID = UserProvider.Instance.User.ID;
                saveModel.BUD_DT = DateTime.UtcNow.AddHours(8);
            }
            else
            {
                saveModel = this.DB.WBWORKS.Where(s => s.ID == ID).FirstOrDefault();
            }
            saveModel.PUB_DT = form["wkDt"] == null ? "" : form["wkDt"];
            saveModel.WKS_TITLE = form["wTitle"] == null ? "" : form["wTitle"];
            saveModel.SR_SQ = form["sortIndex"] == null ? 1 : form["sortIndex"] == string.Empty ? 1 : Convert.ToInt32(form["sortIndex"]);
            saveModel.DIS_FRONT_ST = form["fSt"] == null ? false : true;
            saveModel.WKS_CONTENT = form["contenttext"] == null ? "" : form["contenttext"];
            saveModel.UPD_DT = DateTime.UtcNow.AddHours(8);
            saveModel.UPD_USRID = UserProvider.Instance.User.ID;
            saveModel.LANG_CD = language.GetCode();

            if (ID.HasValue)
                this.DB.Entry(saveModel).State = EntityState.Modified;
            else
                this.DB.WBWORKS.Add(saveModel);


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

            foreach (var f in form.Keys)
            {
                if (f.ToString().StartsWith("OtherImagesData"))
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
                ActionName = "Works",
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

            if (images != null)
            {
                if (images.Count > 0)
                    images.RemoveAll(item => item == null);
            }

            #endregion 不知道為什麼 前端由js加入的formdata file類型 會多出null 砍了就對了

            #region img data binding 單筆多筆裝在不同容器

            imgModel.UploadType = "images_s";
            imgepository.UploadPhoto("Post", imgModel, image, "S");
            imgModel.UploadType = "images_m";
            imgepository.UploadPhoto("Post", imgModel, images, "M");
            imgepository.SaveImagesToDB(imgModel);

            #endregion img data binding 單筆多筆裝在不同容器

            #endregion 圖片處理
            return identityId;
        }

        /// <summary>
        /// 語系過濾條件
        /// </summary>
        /// <param name="language"></param>
        /// <param name="data"></param>
        private void ListFilterLanguage(Language language, ref List<WorksListDataModel> data)
        {
            var r = data.Where(s => s.Language == language.GetCode()).ToList();
            data = r;
        }

        /// <summary>
        /// 列表關鍵字搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void ListFilter(string filterStr, ref List<WorksListDataModel> data)
        {
            var r = data.Where(s => s.WorksName.Contains(filterStr)).ToList();
            data = r;
        }

        /// <summary>
        /// 列表分類搜尋
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="data"></param>

        private void ListTypeFilter(string typeCode, ref List<WorksListDataModel> data)
        {
            var r = data.Where(s => s.TypeID.ToString().Contains(typeCode)).ToList();
            data = r;
        }

        /// <summary>
        /// 日期條件搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void ListDateFilter(string publishdate, ref List<WorksListDataModel> data)
        {
            var r = data.Where(s => s.PublishDateStr == publishdate).ToList();
            data = r;
        }

        /// <summary>
        /// 狀態搜尋
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="data"></param>
        private void ListStatusFilter(string filterStatus, ref List<WorksListDataModel> data)
        {
            bool bDisplayForFront = filterStatus == "Y" ? true : false;
            var r = data.Where(s => s.DisplayForFront == bDisplayForFront).ToList();
            data = r;
        }

        /// <summary>
        /// 取出分頁資料
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="data"></param>
        private void ListPageList(int currentPage, ref List<WorksListDataModel> data, out PaginationResult pagination)
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
        private void ListSort(string sortCloumn, string status, ref List<WorksListDataModel> data)
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
                    data = data.OrderBy(o => o.DisplayForFront).ThenByDescending(g => g.Sort).ToList();
                    break;

                case "sortDisplayForFront/desc":
                    data = data.OrderByDescending(o => o.DisplayForFront).ThenByDescending(g => g.Sort).ToList();
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