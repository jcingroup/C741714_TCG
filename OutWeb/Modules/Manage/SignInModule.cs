
using OutWeb.Entities;
using OutWeb.Models.UserInfo;
using OutWeb.Provider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Modules.Manage
{

    public class SignInModule
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }
        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public LoginUserInfoModel GetUserBySignID(SignInModel userModel)
        {
            LoginUserInfoModel userInfo =
            this.DB.WBUSR
                .Where(s => s.SIGNIN_ID == userModel.AccountName && s.SIGNIN_PWD == userModel.Password)
                         .Select(s => new LoginUserInfoModel()
                         {
                             ID = s.ID,
                             UserAccountName = s.SIGNIN_ID,
                             UserName = s.USR_NM,
                             UserEngName = s.USR_ENM,
                             Email = s.USR_EML,
                             CreateDate = s.BUD_DTM,
                             GUID = s.USR_GUID
                         })
                         .FirstOrDefault();
            return userInfo;
        }


        public bool ChangePassword(FormCollection form)
        {
            if (UserProvider.Instance.User == null)
            {
                throw new Exception("請先登入!");
            }
            var oldPwd = form["oldPw"];
            var newPwd = form["newPw"];
            var rePwd = form["rePw"];
            var entityUser = this.DB.WBUSR.Where(o => o.SIGNIN_ID == UserProvider.Instance.User.UserAccountName).First();
            bool isTruePw = (oldPwd + UserProvider.Instance.User.GUID == entityUser.SIGNIN_PWD + UserProvider.Instance.User.GUID);
            if (isTruePw)
            {
                if (newPwd.Equals(rePwd))
                {
                    entityUser.SIGNIN_PWD = rePwd;
                    this.DB.Entry(entityUser).State = EntityState.Modified;
                    this.DB.SaveChanges();
                }
                else
                    throw new Exception("新密碼兩次輸入密碼不同.");
            }
            else
                throw new Exception("原密碼輸入錯誤.");
            return true;
        }
    }
}