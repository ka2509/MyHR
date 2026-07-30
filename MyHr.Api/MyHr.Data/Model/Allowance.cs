using MyHr.Data.Enum;

namespace MyHr.Data.Model
{
    public class Allowance
    {
        public String Id { get; set; }
        public AllowanceType Type { get; set; }
        public Int32 Level { get; set; }
        public String Name { get; set; }
        public Decimal Coefficient { get; set; }
    }
}
