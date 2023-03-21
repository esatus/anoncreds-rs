namespace anoncreds_rs_dotnet.Models
{
    public class NonrevokedIntervalOverride
    {
        public string RevRegDefId { get; set; }
        public int RequestedFromTs { get; set; }
        public int OverrideRevStatusListTs { get; set; }
    }
}
