using OutWeb.Entities;
using OutWeb.Models.FrontModels.News.EventLatestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Repositories
{
    public class NewEventLatestRepository
    {
        //public EventLatestResult GetList(int page)
        //{
        //    EventLatestResult result = new EventLatestResult();
        //    using (var db = new TCGDB())
        //    {
        //        try
        //        {
        //            var data = db.DLFILES
        //                                .AsEnumerable()
        //                                .OrderByDescending(o => o.PUB_DT_STR).ThenByDescending(a => a.SQ)
        //                                .Where(o => o.DISABLE == false)
        //                                .Select(o => new DownloadFrontDataModel()
        //                                {
        //                                    ID = o.ID,
        //                                    Title = o.TITLE,
        //                                    PublishDateStr = o.PUB_DT_STR
        //                                })
        //                                .ToList();
        //            result.Data = data;
        //            result = this.ListPagination(ref result, page, Convert.ToInt32(PublicMethodRepository.GetConfigAppSetting("DefaultPageSize")));

        //            foreach (var item in data)
        //                PublicMethodRepository.HtmlDecode(item);


        //            using (var fileModule = new FileModule())
        //            {
        //                foreach (var item in data)
        //                    item.Files = fileModule.GetFiles((int)item.ID, "Download", "F");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    return result;
        //}
    }
}