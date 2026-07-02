using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MerchApp.Services
{
    [JsonSerializable(typeof(AppSettings))]
    [JsonSerializable(typeof(SharePointSettings))]
    [JsonSerializable(typeof(RolesSettings))]
    internal partial class AppSettingsJsonContext : JsonSerializerContext
    {
    }
}
