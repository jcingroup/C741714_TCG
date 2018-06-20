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

        News = 7, //中央新聞(En)
        ニュース = 8, //中央新聞(JP)

        Announcement = 9, //中央公告(En)
        お知らせ = 10, //中央公告(JP)

        Statement = 11, //中央聲明(En)
        掲示される = 12, //中央聲明(JP)

        HQ_Events = 13, //各州活動(En)
        各州の活動 = 14, //各州活動(JP)

        HighlightsColumn = 15,//焦點專欄(En)
        フォーカスコラム = 16, //焦點專欄(JP)

        State_Events = 17, //中央活動(En)
        中央の活動 = 18, //中央活動(JP)

        DefaultValue = 999,

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

            var aclData = aclRepo.GetList(new Models.FrontModels.News.AnnouncementLatest.AnnouncementLatestFilter() { LangCode = langCode }, 10000, isIndex);

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
                    ContentUrl = @"/News/AnnouncementContent?ID=" + acl.ID + "&typeID=" + acl.CateIDInfo.Keys.First(),
                    BD_DTString = acl.BD_DTString,
                    Sort = acl.Sort,
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

                    case 5:
                        temp.DataType = ListKind.News;
                        break;

                    case 6:
                        temp.DataType = ListKind.ニュース;
                        break;

                    case 7:
                        temp.DataType = ListKind.Announcement;
                        break;

                    case 8:
                        temp.DataType = ListKind.お知らせ;
                        break;

                    case 9:
                        temp.DataType = ListKind.Statement;
                        break;

                    case 10:
                        temp.DataType = ListKind.掲示される;
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
                var d = statesRepo.GetList(states.Key, new Models.FrontModels.News.EventStatesModels.EventStatesListFilter{ LangCode = langCode }, 10000, isIndex);
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
                        //DataType = ListKind.各州活動,
                        ListTitleUrl = @"/News/EventStatesList?statesTypeID=" + states.StatesTypeID,
                        ContentUrl = @"/News/EventStatesContent?statesTypeID=" + states.StatesTypeID + "&ID=" + d.ID,
                        BD_DTString = d.BD_DTString,
                        Sort = d.Sort,
                    };

                    switch (langCode)
                    {
                        case  "en":
                            temp.DataType = ListKind.HQ_Events;
                            break;

                        case "JPN":
                            temp.DataType = ListKind.各州の活動;
                            break;

                        case "zh-tw":
                        default:
                            temp.DataType = ListKind.各州活動;
                            break;
                       
                    }

                    result.Add(temp);
                }
            }

            //焦點
            FocusRepository focusReps = new FocusRepository();
            List<FocusNewsResult> focusList = new List<FocusNewsResult>();
            var focusType = focusReps.GetFocusCate(langCode);
            foreach (var fcous in focusType)
            {
                var d = focusReps.GetList(fcous.Key, new FocusNewsListFilter { LangCode = langCode }, 10000, isIndex);
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
                        //DataType = ListKind.焦點專欄,
                        ListTitleUrl = @"/News/FocusList?focusTypeID=" + focus.FocusTypeInfo.Keys.First(),
                        ContentUrl = @"/News/FocusContent?focusTypeID=" + focus.FocusTypeInfo.Keys.First() + "&ID=" + d.ID,
                        BD_DTString = d.BD_DTString,
                        Sort = d.Sort,
                    };

                    switch (langCode)
                    {
                        case "en":
                            temp.DataType = ListKind.HighlightsColumn;
                            break;

                        case "JPN":
                            temp.DataType = ListKind.フォーカスコラム;
                            break;

                        case "zh-tw":
                        default:
                            temp.DataType = ListKind.焦點專欄;
                            break;

                    }

                    result.Add(temp);
                }
            }

            //中央活動
            EventLatestRepository eventRepo = new EventLatestRepository();
            var eventData = eventRepo.GetList(new EventLatestListFilter { LangCode = langCode}, 10000, isIndex);
            foreach (var ev in eventData.Data)
            {
                LatestData temp = new LatestData()
                {
                    ID = ev.ID,
                    Img = ev.Img,
                    PublishDateString = ev.PublishDateString,
                    Remark = ev.Remark,
                    Title = ev.Title,
                    //DataType = ListKind.中央活動,
                    ListTitleUrl = @"/News/EventLatest",
                    ContentUrl = @"/News/EventLatestContent?&ID=" + ev.ID,
                    BD_DTString = ev.BD_DTString,
                    Sort = ev.Sort,
                };

                switch (langCode)
                {
                    case "en":
                        temp.DataType = ListKind.State_Events;
                        break;

                    case "JPN":
                        temp.DataType = ListKind.中央の活動;
                        break;

                    case "zh-tw":
                    default:
                        temp.DataType = ListKind.中央活動;
                        break;

                }
                result.Add(temp);
            }

            if (result.Count > 0)
            {
                //result = result.OrderByDescending(s => s.PublishDateString).ToList().Take(20).ToList();
                result = result.OrderByDescending(s => s.Sort)
                                .ThenByDescending(x=>x.PublishDateString)
                                .ThenByDescending(y=>y.BD_DTString).ToList().Take(20).ToList();
            }
            return result;
        }

      
    }
}