using BasicAuthentication.Enums;

namespace BasicAuthentication.Entities
{
    public class UserPermission
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public Permission Permission { get; set; }
    }
}
