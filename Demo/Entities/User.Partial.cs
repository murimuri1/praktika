using System.Data;

namespace Demo.Entities
{
    public partial class User
    {
        public bool IsAdmin
        {
            get
            {
                return Role != null && Role.Name_Role == "Admin";
            }
        }
    }
}