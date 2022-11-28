using System.Runtime.Serialization;

namespace anoncreds_rs_dotnet.Models
{
    public enum PredicateTypes
    {
        [EnumMember(Value = ">=")]
        GE,
        [EnumMember(Value = "<=")]
        LE,
        [EnumMember(Value = ">")]
        GT,
        [EnumMember(Value = "<")]
        LT
    }
}