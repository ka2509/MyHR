namespace MyHr.Data.Dto
{
    public class OrganizationResponse
    {
        public String Id { get; set; } = string.Empty;
        public String Name { get; set; } = string.Empty;
        public Int32 Type { get; set; }  // OrganizationType as int for sorting
        public String? ParentId { get; set; }
    }
}
