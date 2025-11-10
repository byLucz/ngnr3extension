using DocsVision.Platform.StorageServer;
using DocsVision.Platform.StorageServer.Extensibility;
using System;
using System.Data;

namespace SE
{
    public class CustomGridExtension : StorageServerExtension
    {
        public CustomGridExtension() { }

        [ExtensionMethod]
        public CursorInfo GetInfoFromEmpl(Guid emplId)
        {

            using (var cmd = DbRequest.DataLayer.Connection.CreateCommand("getInfoFromEmpl", CommandType.StoredProcedure)) {
                cmd.AddParameter("emplId", DbType.Guid, ParameterDirection.Input, 0, emplId);
                return ExecuteCursorCommand(cmd);
            }
         
        }
    }
}