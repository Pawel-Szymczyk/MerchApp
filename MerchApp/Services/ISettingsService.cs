using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Services
{
    public interface ISettingsService
    {
        AppSettings Settings { get; }
    }
}
