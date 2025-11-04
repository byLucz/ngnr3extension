using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtension.CardEditor.Models
{
    public class BTPerDiemModel
    {
        public int Days { get; set; }
        public decimal Total { get; set; }
    }

    public class BTEmployeeInfoModel
    {
        public Guid EmplId { get; set; }
        public string EmplName { get; set; }
        public Guid? MngrId { get; set; }
        public string MngrName { get; set; }
        public string Phone { get; set; }
    }

    public class BTEmplInfoRequestModel
    {
        public Guid CardId { get; set; }      
        public Guid EmplId { get; set; } 
    }

    public class BTPerDiemRequestModel
    {
        public Guid CardId { get; set; }
        public Guid CityRowId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
