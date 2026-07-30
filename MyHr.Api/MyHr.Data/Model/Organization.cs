using MyHr.Data.Enum;

namespace MyHr.Data.Model
{
    public class Organization
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public OrganizationType Type { get; set; }
        public String? ParentId { get; set; }
    }
}
