using System;
using NodaTime;

namespace WetPicsRebirth.Data;

public interface IEntityBase
{
    Instant AddedDate { get; set; }

    Instant ModifiedDate { get; set; }
}