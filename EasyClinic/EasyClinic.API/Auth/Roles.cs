namespace EasyClinic.API.Auth
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Receptionist = "Receptionist";
        public const string User = "User";
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string StaffOnly = "StaffOnly";
    }
}