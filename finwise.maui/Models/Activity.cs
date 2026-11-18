namespace finwise.maui.Models
{
    // Placeholder model: to be replaced by the Phase 1 domain model
    // (decimal amounts, currency, date, payer, splits, category).
    public class Activity
    {
        public string id { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public float amount { get; set; }
        public string activityType { get; set; } = string.Empty;
    }
}
