using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Models.Manage.AgentModels;
using OutWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Modules.Manage
{
    public class AgentModule : ListModuleService
    {
        private WBDBEntities m_DB = new WBDBEntities();

        private WBDBEntities DB
        { get { return this.m_DB; } set { this.m_DB = value; } }

        public override void DoDeleteByID(int ID)
        {
            throw new NotImplementedException();
        }

        public override object DoGetDetailsByID(int ID)
        {
            throw new NotImplementedException();
        }

        public override object DoGetList<TFilter>(TFilter model, Language language)
        {
            AgentDataModel data = new AgentDataModel();
            var agent = this.DB.WBAGENT.FirstOrDefault();

            if (agent != null)
                data.Content = agent.AGENT_CONTENT;
            return data;
        }

        public override int DoSaveData(FormCollection form, Language language, int? ID = default(int?), List<HttpPostedFileBase> image = null, List<HttpPostedFileBase> images = null)
        {
            WBAGENT agent = new WBAGENT();
            this.DB.Database.ExecuteSqlCommand("DELETE WBAGENT");

            agent.AGENT_CONTENT = form["contenttext"];
            this.DB.WBAGENT.Add(agent);

            try
            {
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }
    }
}