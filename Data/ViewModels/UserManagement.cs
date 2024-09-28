namespace AuthApp.Data.ViewModels
{
    public class UserManagementVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsBlocked { get; set; }
    }
}
