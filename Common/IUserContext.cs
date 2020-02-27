namespace Common
{
    public interface IUserContext
    {
        UserDto CurrentUser { get; }
    }
}