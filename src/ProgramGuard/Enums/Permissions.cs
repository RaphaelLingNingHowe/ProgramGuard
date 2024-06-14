namespace ProgramGuard.Enums
{
    [Flags]
    public enum Permissions
    {
        None = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8
    }
}
