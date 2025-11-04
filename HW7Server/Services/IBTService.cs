using DocsVision.Platform.WebClient;
using ServerExtension.CardEditor.Models;
using System;

namespace ServerExtension.CardEditor.Services
{
    public interface IBTService
    {
        BTEmployeeInfoModel GetEmployeeInfo(SessionContext sessionContext, Guid emplId);

        BTPerDiemModel RecalculatePerDiem(SessionContext sessionContext, BTPerDiemRequestModel model);

        void InitializeOnCreate(SessionContext sessionContext, Guid cardId);
    }
}
