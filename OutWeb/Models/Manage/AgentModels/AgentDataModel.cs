using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.Manage.AgentModels
{
    public class AgentDataModel
    {
        public string m_content = string.Empty;
        public string Content
        {
            get { return this.m_content; }
            set { this.m_content = value; }
        }
    }
}