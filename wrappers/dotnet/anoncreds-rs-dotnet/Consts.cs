namespace anoncreds_rs_dotnet
{
    public static class Consts
    {
#if __IOS__
        public const string CREDX_LIB_NAME = "__Internal";
#else
        public const string CREDX_LIB_NAME = "anoncreds";
#endif
    }
}