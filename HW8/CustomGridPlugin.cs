using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.Layout.WebClient.Models;
using DocsVision.Layout.WebClient.Models.TableData;
using DocsVision.Layout.WebClient.Services;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataGridControlServerExtension.DataGridControlPlugins
{

    public class CustomGridPlugin : IDataGridControlPlugin
    {
        const string CurrentCardIdParameterName = "CurrentCardId";
        public string Name => "CustomGridPlugin";

        public TableModel GetTableData(SessionContext sessionContext, List<ParamModel> parameters)
        {
            var table = new TableModel {
                Id = Guid.NewGuid().ToString(),
                Columns = new List<ColumnModel>(),
                Rows = new List<RowModel>()
            };

            table.Columns.Add(new ColumnModel { Id = "RowNum", Name = "N", Type = DocsVision.WebClient.Models.Grid.ColumnType.Integer });
            table.Columns.Add(new ColumnModel { Id = "DateOut", Name = "Дата", Type = DocsVision.WebClient.Models.Grid.ColumnType.String });
            table.Columns.Add(new ColumnModel { Id = "City", Name = "Город", Type = DocsVision.WebClient.Models.Grid.ColumnType.String });
            table.Columns.Add(new ColumnModel { Id = "Reason", Name = "Причина", Type = DocsVision.WebClient.Models.Grid.ColumnType.String });
            table.Columns.Add(new ColumnModel { Id = "State", Name = "Статус", Type = DocsVision.WebClient.Models.Grid.ColumnType.String });

            var cardId = parameters.FirstOrDefault(p => p.Key == CurrentCardIdParameterName);
            if (!Guid.TryParse(cardId.Value, out var cId))
                return table;

            var cardData = sessionContext.AdvancedCardManager.GetCardData(cId, false);
            var mainSection = cardData.Sections[CardDocument.MainInfo.ID];
            var mainRow = mainSection.Rows[0];
            var emplId = (Guid)mainRow["emplOut"];
            var method = sessionContext.Session.ExtensionManager.GetExtensionMethod("CustomGridExtension", "GetInfoFromEmpl");

            method.Parameters.AddNew("emplId", ParameterValueType.Guid).Value = emplId;

            using (InfoRowCollection rows = method.ExecuteReader()) {
                foreach (InfoRow row in rows) {
                    int rowNum = Convert.ToInt32(row["RowNum"]);
                    DateTime dateOut = (DateTime)row["DateOut"];
                    string city = row["City"] as string;
                    string reason = row["Reason"] as string;
                    string state = row["State"] as string;

                    table.Rows.Add(new RowModel {
                        Id = rowNum.ToString(),
                        EntityId = rowNum.ToString(),
                        Cells = new List<CellModel> {
                            new CellModel { ColumnId = "RowNum",  Value = rowNum },
                            new CellModel { ColumnId = "DateOut", Value = dateOut.ToString("dd.MM.yyyy") },
                            new CellModel { ColumnId = "City",    Value = city },
                            new CellModel { ColumnId = "Reason",  Value = reason },
                            new CellModel { ColumnId = "State",   Value = state }
                        }
                    });
                }
            }
            return table;
        }
    }
}
