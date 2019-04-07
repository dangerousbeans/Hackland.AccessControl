using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Interfaces
{
    public interface IMetadataFields
    {
        DateTime CreatedTimestamp { get; set; }
        int? CreatedByUserId { get; set; }
        DateTime? UpdatedTimestamp { get; set; }
        int? UpdatedByUserId { get; set; }
    }

}
