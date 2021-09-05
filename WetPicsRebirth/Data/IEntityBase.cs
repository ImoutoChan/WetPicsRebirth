using System;

namespace WetPicsRebirth.Data
{
    public interface IEntityBase
    {
        DateTimeOffset AddedDate { get; set; }

        DateTimeOffset ModifiedDate { get; set; }
    }
}
