using AviaServerExtension.ASE.Models;
using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AviaServerExtension.ASE
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ASEController : ControllerBase
    {
        private readonly ISessionContextProvider scProvider;
        private readonly IASEService avService;

        public ASEController(ISessionContextProvider scProvider, IASEService avService)
        {
            this.scProvider = scProvider;
            this.avService = avService;
        }

        [HttpPost]
        public async Task<CommonResponse<List<TicketParams>>> GetTickets([FromBody] AvRequest model)
        {
            var accountName = User.Identity.Name;
            var sessionContext = scProvider.GetOrCreateSessionContext(accountName);
            var tickets = await avService.GetTicketsAsync(sessionContext, model);

            return CommonResponse.CreateSuccess(sessionContext, model.CardId, new List<TicketParams>(tickets));
        }
    }
}
