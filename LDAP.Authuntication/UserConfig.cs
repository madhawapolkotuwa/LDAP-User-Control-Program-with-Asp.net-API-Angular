namespace LDAP.Authuntication
{
    public class UserConfig
    {
        public string uid { get; set; } = string.Empty;
        public string cn { get; set; } = string.Empty;
        public string sn { get; set; } = string.Empty;
        public string employeeNumber { get; set; } = string.Empty;
        public string employeeType { get; set; } = string.Empty;
        public string CreatedTimestamp { get; set; } = string.Empty;
    }

    public static class LoggedUser 
    {
        public static string uid { get; set; } = string.Empty;
        public static string cn { get; set; } = string.Empty;
        public static string sn { get; set; } = string.Empty;
        public static string employeeNumber { get; set; } = string.Empty;
        public static string employeeType { get; set; } = string.Empty;
    }
    public static class modifiedMember 
    {
        public static string uid { get; set; } = string.Empty;
        public static string password {  get; set; } = string.Empty;
        public static string cn { get; set; } = string.Empty;
        public static string sn { get; set; } = string.Empty;
        public static uint employeeNumber { get; set; } 
        public static string employeeType { get; set; } = string.Empty;
        public static string ModifiedCommonname { get; set; } = string.Empty;
        public static string ModifiedSurename { get; set; } = string.Empty;
        public static string ModifiedEmployeetype { get; set; } = string.Empty;
        public static uint ModifiedEmployeenumber { get; set; }
    }
}
