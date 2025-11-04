using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectModel.Search;
using DocsVision.Platform.WebClient;
using ServerExtension.CardEditor.Models;
using System;
using System.Linq;

namespace ServerExtension.CardEditor.Services
{
    public sealed class BTService : IBTService
    {
        public BTEmployeeInfoModel GetEmployeeInfo(SessionContext sessionContext, Guid employeeId)
        {
            if (sessionContext == null) throw new ArgumentNullException(nameof(sessionContext));
            if (employeeId == Guid.Empty) return null;

            var objectContext = sessionContext.ObjectContext;
            var employee = objectContext.GetObject<StaffEmployee>(employeeId);
            if (employee == null) return null;

            var manager = GetMngrForEmployee(employee);

            return new BTEmployeeInfoModel
            {
                EmplId = employeeId,
                EmplName = employee.DisplayName,
                MngrId = objectContext.GetObjectRef(manager).Id,
                MngrName = manager?.DisplayName,
                Phone = employee.Phone
            };
        }


        public BTPerDiemModel RecalculatePerDiem(SessionContext sessionContext, BTPerDiemRequestModel model)
        {
            if (sessionContext == null) throw new ArgumentNullException(nameof(sessionContext));

            ObjectContext objectContext = sessionContext.ObjectContext;

            var dateFrom = model.DateFrom;
            var dateTo = model.DateTo;
            decimal sum = 0m;
            int days = 0;
            decimal total = 0m;

            if (model.CityRowId != Guid.Empty)
            {
                var cityItem = objectContext.GetObject<BaseUniversalItem>(model.CityRowId);
                if (cityItem != null && cityItem.ItemCard != null)
                {
                    var sumObj = cityItem.ItemCard.MainInfo["perDiem"]; 
                    if (sumObj != null)
                        sum = Convert.ToDecimal(sumObj);
                }
            }

            if (dateFrom.HasValue && dateTo.HasValue && dateTo.Value.Date >= dateFrom.Value.Date)
            {
                days = (int)(dateTo.Value.Date - dateFrom.Value.Date).TotalDays + 1;
                total = sum * days;
            }

            var document = objectContext.GetObject<Document>(model.CardId);
            var main = document.MainInfo;
            main["sumOut"] = total;

            objectContext.SaveObject(document);
            objectContext.AcceptChanges();

            return new BTPerDiemModel
            {
                Days = days,
                Total = total
            };
        }

        public void InitializeOnCreate(SessionContext sessionContext, Guid cardId)
        {
            if (sessionContext == null) throw new ArgumentNullException(nameof(sessionContext));

            var objectContext = sessionContext.ObjectContext;
            var staffService = objectContext.GetService<IStaffService>();

            var document = objectContext.GetObject<Document>(cardId);
            if (document == null)
                throw new InvalidOperationException($"Document {cardId} not found.");

            var main = document.MainInfo;

            var currentEmployee = staffService.GetCurrentEmployee();
            if (currentEmployee != null)
            {
                var currentId = objectContext.GetObjectRef(currentEmployee).Id;
                main["emplOut"] = currentId;

                var info = GetEmployeeInfo(sessionContext, currentId);
                if (info != null)
                {
                    if (info.MngrId.HasValue)
                        main["mngrOut"] = info.MngrId.Value;

                    if (!string.IsNullOrEmpty(info.Phone))
                        main["phoneOut"] = info.Phone;
                }
            }
            StaffEmployee reg = null;

            var secretaryGroup = objectContext.FindObject<StaffGroup>(
                new QueryObject(StaffGroup.NameProperty.Name, "Секретарь"));

            if (secretaryGroup != null)
            {
                reg = secretaryGroup.Employees
                    .FirstOrDefault(e => e.Status == StaffEmployeeStatus.Active);
            }

            if (reg != null)
                main["regOut"] = objectContext.GetObjectRef(reg).Id;

            var checker = GetMngrForEmployee(currentEmployee);
            if (checker != null)
                main["checkEmplOut"] = objectContext.GetObjectRef(checker).Id;

            objectContext.SaveObject(document);
            objectContext.AcceptChanges();
        }

        private StaffEmployee GetMngrForEmployee(StaffEmployee employee)
        {
            if (employee == null)
                return null;

            var unit = employee.Unit;
            if (unit == null)
                return null;

            var unitMngr = unit.Manager;

            var root = unit;
            while (root.ParentUnit != null)
                root = root.ParentUnit;

            var rootMngr = root.Manager;

            bool isUnitMngr = unitMngr != null && ReferenceEquals(unitMngr, employee);

            if (isUnitMngr)
                return rootMngr;

            return unitMngr ?? rootMngr;
        }
    }
}
