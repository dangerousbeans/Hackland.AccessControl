using System;
using System.Collections.Generic;
using System.Text;

namespace Hackland.AccessControl.Data.Enums
{
    public enum DoorStatus
    {
        Unknown,
        Open, //!MagneticBond && !Reed
        Locking, //!MagneticBond && !Reed && (LockTrigger || Button)
        Closed, //!MagneticBond && Reed
        Locked, //MagneticBond && Reed
        Fault, //MagneticBond && !Reed
        UnlockRequested //Button
    }
}
