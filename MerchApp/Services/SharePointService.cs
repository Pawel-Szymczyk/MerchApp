using MerchApp.Models;
using MerchApp.Services.Interfaces;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public class SharePointService : ISharePointService
    {
        private readonly ISettingsService _settingsService;
        private readonly IAuthService _authService;

        private string SiteUrl => _settingsService.Settings.SharePoint.SiteUrl;
        private string ItemsListName => _settingsService.Settings.SharePoint.ItemsListName;
        private string RequestsListName => _settingsService.Settings.SharePoint.RentalRequestsListName;
        private string RentalItemsListName => _settingsService.Settings.SharePoint.RentalItemsListName;

        public SharePointService(ISettingsService settingsService, IAuthService authService)
        {
            _settingsService = settingsService;
            _authService = authService;
        }

        // =========================================================================
        // CONNECTION TEST
        // =========================================================================

        public async Task<int> TestConnectionAsync()
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(ItemsListName);
            ctx.Load(list, l => l.ItemCount);
            await ctx.ExecuteQueryAsync();

            return list.ItemCount;
        }

        // =========================================================================
        // ITEMS
        // =========================================================================
        public async Task<List<Item>> GetItemsAsync()
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(ItemsListName);

            var query = new CamlQuery
            {
                ViewXml = @"
            <View>
              <Query>
                <OrderBy>
                  <FieldRef Name='Title' Ascending='TRUE'/>
                </OrderBy>
              </Query>
              <ViewFields>
                <FieldRef Name='ID'/>
                <FieldRef Name='Title'/>
              </ViewFields>
            </View>"
            };

            var spItems = list.GetItems(query);
            ctx.Load(spItems);
            await ctx.ExecuteQueryAsync();

            var items = new List<Item>();
            foreach (var i in spItems)
            {
                items.Add(new Item
                {
                    Id = i.Id,
                    Title = i["Title"]?.ToString() ?? string.Empty
                });
            }

            return items;
        }

        public async Task<int> AddItemAsync(string title)
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(ItemsListName);
            var itemInfo = new ListItemCreationInformation();
            var item = list.AddItem(itemInfo);

            item["Title"] = title;
            item.Update();

            await ctx.ExecuteQueryAsync();
            return item.Id;
        }

        public async Task DeleteItemAsync(int itemId)
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(ItemsListName);
            var item = list.GetItemById(itemId);
            item.DeleteObject();

            await ctx.ExecuteQueryAsync();
        }

        // =========================================================================
        // RENTAL REQUESTS
        // =========================================================================

        public async Task<int> CreateRentalRequestAsync(
            AppUser user,
            List<CartItem> cartItems,
            DateTime rentalFrom,
            DateTime rentalTo,
            string purpose = "")
        {
            using var ctx = await GetContextAsync();

            var requestList = ctx.Web.Lists.GetByTitle(RequestsListName);
            var requestInfo = new ListItemCreationInformation();
            var requestItem = requestList.AddItem(requestInfo);

            var refNumber = $"REQ-{DateTime.Now:yyyyMMdd-HHmmss}";

            requestItem["Title"] = refNumber;
            requestItem["UserEmail"] = user.Email;
            requestItem["UserDisplayName"] = user.DisplayName;
            requestItem["Status"] = "Pending";
            requestItem["RentalFrom"] = rentalFrom;
            requestItem["RentalTo"] = rentalTo;
            requestItem["Purpose"] = purpose;
            requestItem.Update();

            await ctx.ExecuteQueryAsync();

            int requestId = requestItem.Id;

            var rentalItemsList = ctx.Web.Lists.GetByTitle(RentalItemsListName);

            foreach (var cartItem in cartItems)
            {
                var lineInfo = new ListItemCreationInformation();
                var lineItem = rentalItemsList.AddItem(lineInfo);

                lineItem["Title"] = $"{refNumber}-{cartItem.Item.Title}";
                lineItem["RequestId"] = requestId;
                lineItem["ItemId"] = cartItem.Item.Id;
                lineItem["ItemName"] = cartItem.Item.Title;
                lineItem.Update();
            }

            await ctx.ExecuteQueryAsync();

            return requestId;
        }

        public async Task<List<RentalRequest>> GetAllRentalRequestsAsync()
        {
            return await LoadRentalRequestsAsync(filterByEmail: null);
        }

        public async Task<List<RentalRequest>> GetMyRentalRequestsAsync(string userEmail)
        {
            return await LoadRentalRequestsAsync(filterByEmail: userEmail);
        }

        public async Task ApproveRequestAsync(int requestId, string note = "")
        {
            await UpdateRequestStatusAsync(requestId, "Approved", note);
        }

        public async Task RejectRequestAsync(int requestId, string reason)
        {
            await UpdateRequestStatusAsync(requestId, "Rejected", reason);
        }

        public async Task MarkAsReturnedAsync(int requestId)
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(RequestsListName);
            var item = list.GetItemById(requestId);

            item["Status"] = "Returned";
            item["ReturnedDate"] = DateTime.Today;
            item.Update();

            await ctx.ExecuteQueryAsync();
        }

        public async Task DeleteRentalRequestAsync(int requestId)
        {
            using var ctx = await GetContextAsync();

            // Delete all RentalItems for this request first
            var rentalItemsList = ctx.Web.Lists.GetByTitle(RentalItemsListName);

            var query = new CamlQuery
            {
                ViewXml = $@"
            <View>
              <Query>
                <Where>
                  <Eq>
                    <FieldRef Name='RequestId'/>
                    <Value Type='Number'>{requestId}</Value>
                  </Eq>
                </Where>
              </Query>
            </View>"
            };

            var items = rentalItemsList.GetItems(query);
            ctx.Load(items);
            await ctx.ExecuteQueryAsync();

            foreach (var item in items)
                item.DeleteObject();

            await ctx.ExecuteQueryAsync();

            // Delete the request
            var requestList = ctx.Web.Lists.GetByTitle(RequestsListName);
            var request = requestList.GetItemById(requestId);
            request.DeleteObject();

            await ctx.ExecuteQueryAsync();
        }

        // =========================================================================
        // PRIVATE HELPERS
        // =========================================================================

        private async Task<ClientContext> GetContextAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var ctx = new ClientContext(SiteUrl);

            ctx.ExecutingWebRequest += (sender, e) =>
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + token;
            };

            return ctx;
        }

        private async Task<List<RentalRequest>> LoadRentalRequestsAsync(string? filterByEmail)
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(RequestsListName);

            var whereClause = filterByEmail != null
                ? $@"<Where>
                   <Eq>
                     <FieldRef Name='UserEmail'/>
                     <Value Type='Text'>{filterByEmail}</Value>
                   </Eq>
                 </Where>"
                : string.Empty;

            var query = new CamlQuery
            {
                ViewXml = $@"
                <View>
                  <Query>
                    {whereClause}
                    <OrderBy>
                      <FieldRef Name='Created' Ascending='FALSE'/>
                    </OrderBy>
                  </Query>
                  <ViewFields>
                    <FieldRef Name='ID'/>
                    <FieldRef Name='Title'/>
                    <FieldRef Name='UserEmail'/>
                    <FieldRef Name='UserDisplayName'/>
                    <FieldRef Name='Status'/>
                    <FieldRef Name='RentalFrom'/>
                    <FieldRef Name='RentalTo'/>
                    <FieldRef Name='ReturnedDate'/>
                    <FieldRef Name='ManagerNote'/>
                    <FieldRef Name='Purpose'/>
                    <FieldRef Name='Created'/>
                  </ViewFields>
                </View>"
            };

            var spItems = list.GetItems(query);
            ctx.Load(spItems);
            await ctx.ExecuteQueryAsync();

            var requests = new List<RentalRequest>();
            foreach (var i in spItems)
            {
                requests.Add(new RentalRequest
                {
                    Id = i.Id,
                    UserEmail = i.FieldValues.ContainsKey("UserEmail") && i["UserEmail"] != null
                                          ? i["UserEmail"].ToString()
                                          : string.Empty,
                    UserDisplayName = i.FieldValues.ContainsKey("UserDisplayName") && i["UserDisplayName"] != null
                                          ? i["UserDisplayName"].ToString()
                                          : string.Empty,
                    Status = ParseStatus(i.FieldValues.ContainsKey("Status")
                                          ? i["Status"]?.ToString()
                                          : null),
                    RentalFrom = i.FieldValues.ContainsKey("RentalFrom") && i["RentalFrom"] != null
                                          ? (DateTime)i["RentalFrom"]
                                          : DateTime.Today,
                    RentalTo = i.FieldValues.ContainsKey("RentalTo") && i["RentalTo"] != null
                                          ? (DateTime)i["RentalTo"]
                                          : DateTime.Today,
                    ReturnedDate = i.FieldValues.ContainsKey("ReturnedDate") && i["ReturnedDate"] != null
                                          ? (DateTime?)i["ReturnedDate"]
                                          : null,
                    ManagerNote = i.FieldValues.ContainsKey("ManagerNote") && i["ManagerNote"] != null
                                          ? i["ManagerNote"].ToString()
                                          : string.Empty,
                    Purpose = i.FieldValues.ContainsKey("Purpose") && i["Purpose"] != null
                                          ? i["Purpose"].ToString()
                                          : string.Empty,
                    RequestDate = i.FieldValues.ContainsKey("Created") && i["Created"] != null
                                          ? (DateTime)i["Created"]
                                          : DateTime.Now
                });
            }

            if (requests.Any())
            {
                var allRentalItems = await GetRentalItemsForRequestsAsync(
                    ctx, requests.Select(r => r.Id).ToList());

                foreach (var request in requests)
                {
                    request.Items = allRentalItems
                        .Where(ri => ri.RequestId == request.Id)
                        .ToList();
                }
            }

            return requests;
        }

        private async Task<List<RentalItem>> GetRentalItemsForRequestsAsync(
     ClientContext ctx, List<int> requestIds)
        {
            var list = ctx.Web.Lists.GetByTitle(RentalItemsListName);
            var result = new List<RentalItem>();

            // Build query — use Eq for single ID, In for multiple
            string whereClause;

            if (requestIds.Count == 1)
            {
                whereClause = $@"<Where>
            <Eq>
                <FieldRef Name='RequestId'/>
                <Value Type='Number'>{requestIds[0]}</Value>
            </Eq>
        </Where>";
            }
            else
            {
                var inValues = string.Join("",
                    requestIds.Select(id => $"<Value Type='Number'>{id}</Value>"));

                whereClause = $@"<Where>
            <In>
                <FieldRef Name='RequestId'/>
                <Values>{inValues}</Values>
            </In>
        </Where>";
            }

            var query = new CamlQuery
            {
                ViewXml = $@"
            <View>
              <Query>
                {whereClause}
              </Query>
              <ViewFields>
                <FieldRef Name='ID'/>
                <FieldRef Name='RequestId'/>
                <FieldRef Name='ItemId'/>
                <FieldRef Name='ItemName'/>
              </ViewFields>
            </View>"
            };

            var spItems = list.GetItems(query);
            ctx.Load(spItems);
            await ctx.ExecuteQueryAsync();

            foreach (var i in spItems)
            {
                result.Add(new RentalItem
                {
                    Id = i.Id,
                    RequestId = i.FieldValues.ContainsKey("RequestId") && i["RequestId"] != null
                                    ? Convert.ToInt32(i["RequestId"])
                                    : 0,
                    ItemId = i.FieldValues.ContainsKey("ItemId") && i["ItemId"] != null
                                    ? Convert.ToInt32(i["ItemId"])
                                    : 0,
                    ItemName = i.FieldValues.ContainsKey("ItemName") && i["ItemName"] != null
                                    ? i["ItemName"].ToString()
                                    : string.Empty
                });
            }

            return result;
        }

        private async Task UpdateRequestStatusAsync(int requestId, string status, string note)
        {
            using var ctx = await GetContextAsync();

            var list = ctx.Web.Lists.GetByTitle(RequestsListName);
            var item = list.GetItemById(requestId);

            item["Status"] = status;
            item["ManagerNote"] = note;
            item.Update();

            await ctx.ExecuteQueryAsync();
        }

        private static RentalStatus ParseStatus(string? value) => value switch
        {
            "Approved" => RentalStatus.Approved,
            "Rejected" => RentalStatus.Rejected,
            "Returned" => RentalStatus.Returned,
            _ => RentalStatus.Pending
        };
    }
}
