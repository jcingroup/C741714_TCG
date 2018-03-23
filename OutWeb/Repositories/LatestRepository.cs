using OutWeb.Models.FrontModels.News.EventLatestModels;
using OutWeb.Models.FrontModels.News.EventStatesModels;
using OutWeb.Models.FrontModels.News.FocusNewsModels;
using OutWeb.Models.FrontModels.News.LatestModels;
using System.Collections.Generic;
using System.Linq;

namespace OutWeb.Repositories
{
    public enum ListKind
    {
        中央活動 = 1, 各州活動 = 2,
        中央新聞 = 3, 中央公告 = 4,
        中央聲明 = 5, 焦點專欄 = 6,
    }

    public class LatestRepository
    {
        private string _connectionString { get; set; }
        private ConnectionRepository conRepo = new ConnectionRepository();

        public LatestRepository()
        {
            _connectionString = conRepo.GetEntityConnctionString();
        }

        public List<LatestData> GetLatestList(string langCode)
        {
            List<LatestData> result = new List<LatestData>();
            string isIndex = "Y";
            //新聞 公告 聲明
            AnnouncementLatestRepository aclRepo = new AnnouncementLatestRepository();
            var aclData = aclRepo.GetList(new Models.FrontModels.News.AnnouncementLatest.AnnouncementLatestFilter(), 10000, isIndex);

            foreach (var acl in aclData.Data)
            {
                LatestData temp = new LatestData()
                {
                    ID = acl.ID,
                    Img = acl.Img,
                    PublishDateString = acl.PublishDateString,
                    Remark = acl.Content,
                    Title = acl.Title,
                    TypeInfo = aclRepo.GetNewsCateByID(acl.CateIDInfo.Keys.First(), langCode),
                    ListTitleUrl = @"/News/AnnouncementList?typeID=" + acl.CateIDInfo.Keys.First(),
                    ContentUrl = @"/News/AnnouncementContent?ID=" + acl.ID + "&typeID=" + acl.CateIDInfo.Keys.First()
                };
                switch (acl.CateIDInfo.Keys.First())
                {
                    case 1:
                        temp.DataType = ListKind.中央新聞;
                        break;

                    case 2:
                        temp.DataType = ListKind.中央公告;
                        break;

                    case 4:
                        temp.DataType = ListKind.中央聲明;
                        break;

                    default:
                        break;
                }
                result.Add(temp);
            }

            //各洲活動
            EventStatesRepository statesRepo = new EventStatesRepository();
            var statesType = statesRepo.GetStatesCate(langCode);

            List<EventStatesResult> statesList = new List<EventStatesResult>();
            foreach (var states in statesType)
            {
                var d = statesRepo.GetList(states.Key, new Models.FrontModels.News.EventStatesModels.EventStatesListFilter(), 10000, isIndex);
                statesList.Add(d);
            }

            foreach (var states in statesList)
            {
                foreach (var d in states.Data)
                {
                    LatestData temp = new LatestData()
                    {
                        ID = d.ID,
                        Img = d.Img,
                        PublishDateString = d.PublishDateString,
                        Remark = d.Remark,
                        Title = d.Title,
                        TypeInfo = statesRepo.GetStatesCateByID(states.StatesTypeID, langCode),
                        DataType = ListKind.各州活動,
                        ListTitleUrl = @"/News/EventStatesList?statesTypeID=" + states.StatesTypeID,
                        ContentUrl = @"/News/EventStatesContent?statesTypeID=" + states.StatesTypeID + "&ID=" + d.ID
                    };
                    result.Add(temp);
                }
            }

            //焦點
            FocusRepository focusReps = new FocusRepository();
            List<FocusNewsResult> focusList = new List<FocusNewsResult>();
            var focusType = focusReps.GetFocusCate(langCode);
            foreach (var fcous in focusType)
            {
                var d = focusReps.GetList(fcous.Key, new FocusNewsListFilter(), 10000, isIndex);
                focusList.Add(d);
            }

            foreach (var focus in focusList)
            {
                foreach (var d in focus.Data)
                {
                    LatestData temp = new LatestData()
                    {
                        ID = d.ID,
                        Img = d.Img,
                        PublishDateString = d.PublishDateString,
                        Remark = d.Remark,
                        Title = d.Title,
                        TypeInfo = statesRepo.GetStatesCateByID(focus.FocusTypeInfo.Keys.First(), langCode),
                        DataType = ListKind.焦點專欄,
                        ListTitleUrl = @"/News/FocusList?focusTypeID=" + focus.FocusTypeInfo.Keys.First(),
                        ContentUrl = @"/News/FocusContent?focusTypeID=" + focus.FocusTypeInfo.Keys.First() + "&ID=" + d.ID
                    };

                    result.Add(temp);
                }
            }

            //中央活動
            EventLatestRepository eventRepo = new EventLatestRepository();
            var eventData = eventRepo.GetList(new EventLatestListFilter(), 10000, isIndex);
            foreach (var ev in eventData.Data)
            {
                LatestData temp = new LatestData()
                {
                    ID = ev.ID,
                    Img = ev.Img,
                    PublishDateString = ev.PublishDateString,
                    Remark = ev.Remark,
                    Title = ev.Title,
                    DataType = ListKind.中央活動,
                    ListTitleUrl = @"/News/EventLatest",
                    ContentUrl = @"/News/EventLatestContent?&ID=" + ev.ID
                };
                result.Add(temp);
            }

            if (result.Count > 0)
            {
                result = result.OrderByDescending(s => s.PublishDateString).ToList().Take(10).ToList();
            }
            return result;
        }
    }
}