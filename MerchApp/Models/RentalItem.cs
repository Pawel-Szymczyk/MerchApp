namespace MerchApp.Models
{
    /// <summary>
    /// Represents one line in a rental request — which item and how many.
    /// Stored in SharePoint 'RentalItems' list.
    /// Linked to RentalRequest by RequestId, and to Item by ItemId.
    /// </summary>
    public class RentalItem
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }
}
