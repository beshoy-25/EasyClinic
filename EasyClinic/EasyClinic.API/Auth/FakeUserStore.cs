namespace EasyClinic.API.Auth
{
    public record AuthUser(int Id, string Email, string Password, string Role);

    public static class FakeUserStore
    {
        private static readonly List<AuthUser> Users =
        [
            new(1, "admin@easyclinic.com", "Admin@123", Roles.Admin),
            new(2, "reception@easyclinic.com", "Reception@123", Roles.Receptionist),
            new(2, "user@easyclinic.com", "User@123", Roles.User),
        ];

        public static AuthUser? Validate(string email, string password) =>
            Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
    }
}