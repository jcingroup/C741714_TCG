using OutWeb.Entities;
using OutWeb.Models.Manage.ImgModels;
using OutWeb.Provider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutWeb.Modules.Manage
{
    public class ImgModule
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        /// <summary>
        /// 儲存圖片
        /// </summary>
        /// <param name="model"></param>
        public void SaveImgs(ImagesModel model)
        {
            if (model.ID > 0)
            {
                //刪除舊圖
                if (model.ActionName.StartsWith("Product"))
                    this.DB.WBPIC.RemoveRange(this.DB.WBPIC.Where(o => !model.OldImageIds.Contains(o.ID) && o.MAP_PRODUCT_ID == model.ID));
                else if (model.ActionName.StartsWith("News"))
                    this.DB.WBPIC.RemoveRange(this.DB.WBPIC.Where(o => !model.OldImageIds.Contains(o.ID) && o.MAP_NEWS_ID == model.ID));
                else if (model.ActionName.StartsWith("Works"))
                    this.DB.WBPIC.RemoveRange(this.DB.WBPIC.Where(o => !model.OldImageIds.Contains(o.ID) && o.MAP_WORKS_ID == model.ID));
                this.DB.SaveChanges();
            }

            //存檔單筆
            foreach (var img in model.MemberData)
            {
                WBPIC pic = new WBPIC()
                {
                    IMG_NM = img.FileName,
                    MAP_AC_NM = model.ActionName,
                    UP_MODE = "S",
                    IMG_URL = img.FileUrl,
                    IMG_LINK = img.FileUrl,
                    FILE_PATH = img.FilePath,
                    SR_SQ = 1,
                    UP_DT = DateTime.UtcNow.AddHours(8),
                    UP_USR_ID = UserProvider.Instance.User.ID
                };
                if (model.ActionName.StartsWith("Product"))
                    pic.MAP_PRODUCT_ID = model.ID;
                else if (model.ActionName.StartsWith("News"))
                    pic.MAP_NEWS_ID = model.ID;
                else if (model.ActionName.StartsWith("Works"))
                    pic.MAP_WORKS_ID = model.ID;
                this.DB.WBPIC.Add(pic);
                this.DB.SaveChanges();
            }
            //存檔多筆
            foreach (var img in model.MemberDataMultiple)
            {
                int sq = model.MemberDataMultiple.IndexOf(img);
                WBPIC pic = new WBPIC()
                {
                    IMG_NM = img.FileName,
                    MAP_AC_NM = model.ActionName,
                    UP_MODE = "M",
                    IMG_URL = img.FileUrl,
                    IMG_LINK = img.FileUrl,
                    FILE_PATH = img.FilePath,
                    SR_SQ = sq,
                    UP_DT = DateTime.UtcNow.AddHours(8),
                    UP_USR_ID = UserProvider.Instance.User.ID
                };

                if (model.ActionName.StartsWith("Product"))
                    pic.MAP_PRODUCT_ID = model.ID;
                else if (model.ActionName.StartsWith("News"))
                    pic.MAP_NEWS_ID = model.ID;
                else if (model.ActionName.StartsWith("Works"))
                    pic.MAP_WORKS_ID = model.ID;
                this.DB.WBPIC.Add(pic);
                this.DB.SaveChanges();
                img.ID = pic.ID;
            }
        }

        /// <summary>
        /// 取得圖片
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="actionName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<MemberViewModel> GetImages(int ID, string actionName, string actionMode)
        {
            ImagesModel imgModel = new ImagesModel();
            imgModel.ID = ID;
            imgModel.ActionName = actionName;
            imgModel.UploadType = actionMode;

            List<MemberViewModel> imgList = new List<MemberViewModel>();
            if (actionName.StartsWith("Product"))
            {
                imgList = this.DB.WBPIC
                        .Where(o => o.MAP_PRODUCT_ID == ID && o.MAP_AC_NM.StartsWith("Product") && o.UP_MODE == actionMode)
                        .Select(s => new MemberViewModel()
                        {
                            ID = s.ID,
                            FileName = s.IMG_NM,
                            FileUrl = s.IMG_LINK,
                            FilePath = s.FILE_PATH
                        })
                        .ToList();
            }
            else if (actionName.StartsWith("News"))
            {
                imgList = this.DB.WBPIC
                                   .Where(o => o.MAP_NEWS_ID == ID && o.MAP_AC_NM.StartsWith("News"))
                                   .Select(s => new MemberViewModel()
                                   {
                                       ID = s.ID,
                                       FileName = s.IMG_NM,
                                       FileUrl = s.IMG_LINK,
                                       FilePath = s.FILE_PATH
                                   })
                                   .ToList();
            }
            else if (actionName.StartsWith("Works"))
            {
                imgList = this.DB.WBPIC
                                   .Where(o => o.MAP_WORKS_ID == ID && o.MAP_AC_NM.StartsWith("Works") && o.UP_MODE == actionMode)
                                   .Select(s => new MemberViewModel()
                                   {
                                       ID = s.ID,
                                       FileName = s.IMG_NM,
                                       FileUrl = s.IMG_LINK,
                                       FilePath = s.FILE_PATH
                                   })
                                   .ToList();
            }
            return imgList;
        }

        /// <summary>
        /// 刪除圖片
        /// </summary>
        /// <param name="imgID"></param>
        public void DeleteImg(int imgID)
        {
            var img = this.DB.WBPIC.Where(o => o.ID == imgID).First();
            this.DB.WBPIC.Remove(img);
            this.DB.SaveChanges();
        }
    }
}