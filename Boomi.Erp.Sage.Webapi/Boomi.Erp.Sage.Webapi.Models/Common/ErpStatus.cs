using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public enum ErpStatus
    {
        [Description("FinalApproval")]
        FinalApproval,
        [Description("Pending")]
        Pending,
        [Description("Complete")]
        Complete,
        [Description("Failure")]
        Failure,
        [Description("Unknown")]
        Unknown,
    }
}
