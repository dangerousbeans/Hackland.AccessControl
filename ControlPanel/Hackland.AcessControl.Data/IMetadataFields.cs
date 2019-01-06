using System;

namespace Hackland.AccessControl.Data
{
    public interface IMetadataFields
    {
        DateTime CreatedTimestamp { get; set; }
        Guid? CreatedByUserId { get; set; }
        DateTime? UpdatedTimestamp { get; set; }
        Guid? UpdatedByUserId { get; set; }
    }
}