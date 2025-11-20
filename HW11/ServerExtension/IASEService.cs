using AviaServerExtension.ASE.Models;
using DocsVision.Platform.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaServerExtension.ASE
{
    public interface IASEService
    {
        Task<IList<TicketParams>> GetTicketsAsync(SessionContext sessionContext, AvRequest model);
    }
}
