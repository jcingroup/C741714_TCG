using OutWeb.Enums;
using OutWeb.Modules.Manage;

namespace OutWeb.Service
{
    public class ListFactoryService
    {
        public static ListModuleService Create(ListMethodType methodType)
        {
            ListModuleService listManageModule;
            switch (methodType)
            {
                case ListMethodType.NEWS:
                    listManageModule = new NewsModule();
                    break;

                case ListMethodType.PRODUCT:
                    listManageModule = new ProductModule();
                    break;

                case ListMethodType.PRODUCTKIND:
                    listManageModule = new ProductKindModule();
                    break;

                case ListMethodType.WORKS:
                    listManageModule = new WorksModule();
                    break;

                case ListMethodType.AGENT:
                    listManageModule = new AgentModule();
                    break;

                default:
                    listManageModule = null;
                    break;
            }
            return listManageModule;
        }
    }
}