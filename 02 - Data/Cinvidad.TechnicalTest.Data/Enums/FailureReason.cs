using System;
using System.Collections.Generic;
using System.Text;

namespace Cinvidad.TechnicalTest.Data.Enums
{
    public enum FailureReason
    {
        AddressNotFound = 0,
        Weather = 1,
        NoAccess = 2,
        DamagedPackage = 3,
        Other = 99
    }
}
