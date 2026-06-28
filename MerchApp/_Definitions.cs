using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp
{
    public enum UserRole
    {
        User,
        Manager
    }

    public enum RentalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Returned = 3,
    }

    public enum NotificationKind
    {
        Info,
        Success,
        Warning,
        Error
    }
}
