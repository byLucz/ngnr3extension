using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;
using Microsoft.AspNetCore.Mvc;
using ServerExtension.CardEditor.Models;
using ServerExtension.CardEditor.Services;
using System;

namespace ServerExtension.CardEditor
{
    [Route("api/[controller]/[action]")]
    public class BTController : ControllerBase
    {
        private readonly ICurrentObjectContextProvider contextProvider;
        private readonly IBTService btService;
        public BTController(
            ICurrentObjectContextProvider contextProvider,
            IBTService btService)
        {
            this.contextProvider = contextProvider;
            this.btService = btService;
        }

        [HttpPost]
        public CommonResponse<BTEmployeeInfoModel> GetEmployeeInfo(
            [FromBody] BTEmplInfoRequestModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var sessionContext = contextProvider.GetOrCreateCurrentSessionContext();
            var result = btService.GetEmployeeInfo(sessionContext, model.EmplId);

            return CommonResponse.CreateSuccess(sessionContext, model.CardId, result);
        }

        [HttpPost]
        public CommonResponse<BTPerDiemModel> RecalculatePerDiem([FromBody] BTPerDiemRequestModel model)
        {
            var sessionContext = contextProvider.GetOrCreateCurrentSessionContext();
            var result = btService.RecalculatePerDiem(sessionContext, model);
            return CommonResponse.CreateSuccess(sessionContext, model.CardId, result);
        }

    }
}
