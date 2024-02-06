
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static LDAP.Authuntication.RegisterUser;

namespace LDAP.Authuntication
{
    public class LDAPAuth
    {
        public ILDAPConfig ldapConfig;
        public LDAPAuth()
        {
            ldapConfig = new LDAPConfig();
        }

        public bool Login(string username, string password)
        {
            LdapConnection conn = new LdapConnection(new LdapDirectoryIdentifier($"{ldapConfig.Server}", ldapConfig.Port))
            {
                AuthType = AuthType.Basic,
                Credential = new($"uid={username},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}", $"{password}")
            };

            conn.SessionOptions.ProtocolVersion = 3;

            try
            {
                conn.Bind();
                var request = new SearchRequest(
                    $"dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}",
                    $"(&(objectClass=person)(uid={username}))",
                    SearchScope.Subtree,
                    new string[] { "cn", "sn", "employeeNumber", "employeeType" }
                    );
                var searchResponse = (SearchResponse)conn.SendRequest(request);
                var results = searchResponse.Entries.Cast<SearchResultEntry>();

                if( results.Any() )
                {
                    var resultsEntry = results.First();
                    LoggedUser.cn = resultsEntry.Attributes["cn"][0].ToString()!;
                    LoggedUser.sn = resultsEntry.Attributes["sn"][0].ToString()!;
                    LoggedUser.employeeNumber = resultsEntry.Attributes["employeeNumber"][0].ToString()!;
                    LoggedUser.employeeType = resultsEntry.Attributes["employeeType"][0].ToString()!;

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(message: $"Connection error with LDAP :{ex.Message}");
            }


        }

        public bool Register()
        {
            try
            {
                string adminDn = "uid=admin,ou=system";
                string adminPassword = "secret";

                if (Rcn != string.Empty
                    && Rsn != string.Empty
                    && RemployeeType != string.Empty
                    && Ruid != string.Empty
                    && RuserPassword != string.Empty)
                {
                    string newUserDn = $"uid={Ruid},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}";

                    string encodedPassword = PasswordEncode(RuserPassword);

                    // Create LDAP connection
                    using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier($"{ldapConfig.Server}", ldapConfig.Port)))
                    {
                        ldapConnection.Credential = new NetworkCredential(adminDn, adminPassword);
                        ldapConnection.AuthType = AuthType.Basic;

                        ldapConnection.SessionOptions.ProtocolVersion = 3;

                        ldapConnection.Bind();

                        
                        // Create a new user entry
                        var newUserEntry = new AddRequest(newUserDn);
                        newUserEntry.Attributes.Add(new DirectoryAttribute("objectClass", "top"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("objectClass", "person"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("objectClass", "organizationalPerson"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("objectClass", "inetOrgPerson"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("cn", $"{Rcn}"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("sn", $"{Rsn}"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("uid", $"{Ruid}"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("employeeType", $"{RemployeeType}"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("employeeNumber", $"{RemployeeNumber}"));
                        newUserEntry.Attributes.Add(new DirectoryAttribute("userPassword", $"{{SSHA}}{encodedPassword}"));

                        ldapConnection.SendRequest(newUserEntry);

                        // Modify the Group member (Add the new user to a group)
                        AddMemberToGroup(ldapConnection, Ruid, RemployeeType);

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"user register error: {ex.Message}");
            }
        }

        public bool Modify()
        {
            try
            {
                string adminDn = "uid=admin,ou=system";
                string adminPassword = "secret";

                string employee = $"uid={modifiedMember.uid},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}";

                List<DirectoryAttributeModification> modifications = new List<DirectoryAttributeModification>();

                if (modifiedMember.cn != modifiedMember.ModifiedCommonname)
                    AddAttributeModification(modifications, "cn", modifiedMember.ModifiedCommonname);
                if (modifiedMember.sn != modifiedMember.ModifiedSurename)
                    AddAttributeModification(modifications, "sn", modifiedMember.ModifiedSurename);
                if (modifiedMember.employeeType != modifiedMember.ModifiedEmployeetype)
                    AddAttributeModification(modifications, "employeeType", modifiedMember.ModifiedEmployeetype);
                if (modifiedMember.employeeNumber != modifiedMember.ModifiedEmployeenumber)
                    AddAttributeModification(modifications, "employeeNumber", $"{modifiedMember.ModifiedEmployeenumber}");

                string encodedPassword = PasswordEncode(modifiedMember.password);
                AddAttributeModification(modifications, "userPassword", $"{{SSHA}}{encodedPassword}");

                LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier($"{ldapConfig.Server}", ldapConfig.Port));
                ldapConnection.Credential = new NetworkCredential(adminDn, adminPassword);
                ldapConnection.AuthType = AuthType.Basic;

                ldapConnection.SessionOptions.ProtocolVersion = 3;

                ldapConnection.Bind();

                ModifyRequest req = new ModifyRequest(employee);
                foreach (var midification in modifications)
                {
                    req.Modifications.Add(midification);
                }
                ldapConnection.SendRequest(req);

                if (modifiedMember.employeeType != modifiedMember.ModifiedEmployeetype)
                {
                    DeleteGroupMember(ldapConnection, modifiedMember.uid, modifiedMember.employeeType);
                    AddMemberToGroup(ldapConnection, modifiedMember.uid, modifiedMember.ModifiedEmployeetype);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Modify error: {ex.Message}");
            }
        }

        public void AddMemberToGroup(LdapConnection ldapConnection, string uid, string group)
        {
            try
            {
                string groupDn = $"cn={group},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}";
                //DirectoryAttributeModification mod = new DirectoryAttributeModification { Name = "member", Operation = DirectoryAttributeOperation.Add };
                //mod.Add($"uid={uid}");
                ModifyRequest req = new ModifyRequest(groupDn, DirectoryAttributeOperation.Add, "member", $"uid={uid}");
                ldapConnection.SendRequest(req);
            }
            catch(Exception ex)
            {
                throw new Exception($"Adding a member to Group error: {ex.Message}");
            }
            
        }

        public void DeleteGroupMember(LdapConnection ldapConnection,string uid, string group)
        {
            try
            {
                string groupDn = $"cn={group},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}";
                ModifyRequest req = new ModifyRequest(groupDn, DirectoryAttributeOperation.Delete, "member", $"uid={uid}");
                ldapConnection.SendRequest(req);
            }
            catch (Exception ex)
            {
                throw new Exception($"Delete member of Group error: {ex.Message}");
            }
        }

        public void DeleteMember(string uid, string group)
        {
            try
            {
                string adminDn = "uid=admin,ou=system";
                string adminPassword = "secret";

                string userDn = $"uid={uid},dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}";

                using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier($"{ldapConfig.Server}", ldapConfig.Port)))
                {
                    ldapConnection.Credential = new NetworkCredential(adminDn, adminPassword);
                    ldapConnection.AuthType = AuthType.Basic;

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.Bind();

                    DeleteRequest deleteRequest = new DeleteRequest(userDn);
                    ldapConnection.SendRequest(deleteRequest);
                    DeleteGroupMember(ldapConnection,uid,group);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Delete Member error: {ex.Message}");
            }
        }

        public void GetGroups(LdapConnection conn)
        {
            try
            {
                foreach (string group in ldapConfig.Groups)
                {
                    string[] groupArray = new string[] { group };
                    string filter = $"(&(objectClass=groupOfNames)(cn={group}))";
                    string[] attributesToRetrieve = { "member" };
                    SearchRequest searchRequest = new SearchRequest(
                                   $"dc={ldapConfig.Subdomain},dc={ldapConfig.Domain}",
                                   filter,
                                   SearchScope.Subtree,
                                   attributesToRetrieve
                                   );
                    SearchResponse searchResponse = (SearchResponse)conn.SendRequest(searchRequest);
                    // Process the search results
                    foreach (SearchResultEntry entry in searchResponse.Entries)
                    {
                        // Retrieve the 'uniqueMember' attribute values
                        if (entry.Attributes.Contains("member"))
                        {
                            foreach (string value in entry.Attributes["member"].GetValues(typeof(string)))
                            {
                                string cleanedMember = value.Replace("uid=", "");
                                groupArray = groupArray.Concat(new string[] { cleanedMember }).ToArray();
                            }

                            //UserConfig.Groups.Add(groupArray);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"GetGroup Error Adding: {ex.Message}");
            }
        }

        public void GetAllMembers()
        {
            string adminDn = "uid=admin,ou=system";
            string adminPassword = "secret";

            LdapConnection conn = new LdapConnection(new LdapDirectoryIdentifier($"{ldapConfig.Server}", ldapConfig.Port))
            {
                AuthType = AuthType.Basic,
                Credential = new NetworkCredential(adminDn, adminPassword)
            };
            conn.SessionOptions.ProtocolVersion = 3;

            try
            {
                conn.Bind();
                SearchRequest searchRequest = new SearchRequest(
                    "dc=example,dc=com",
                    "(objectClass=inetOrgPerson)",
                    SearchScope.Subtree,
                    new string[] {"uid", "cn", "sn", "employeeNumber", "employeeType", "createTimestamp" }
                );

                SearchResponse searchResponse = (SearchResponse)conn.SendRequest(searchRequest);
                Members.members.Clear();
                foreach (SearchResultEntry entry in searchResponse.Entries)
                {
                    // Access attributes of each user entry
                    string uid = entry.Attributes["uid"]?.GetValues(typeof(string))[0]?.ToString()!;
                    string cn = entry.Attributes["cn"]?.GetValues(typeof(string))[0]?.ToString()!;
                    string sn = entry.Attributes["sn"]?.GetValues(typeof(string))[0]?.ToString()!;
                    string empNum = entry.Attributes["employeeNumber"]?.GetValues(typeof(string))[0]?.ToString()!;
                    string empType = entry.Attributes["employeeType"]?.GetValues(typeof(string))[0]?.ToString()!;
                    string createTimestamp = entry.Attributes["createTimestamp"]?.GetValues(typeof(string))[0]?.ToString()!;
                    DateTimeOffset Timestamp = ParseLdapTimestamp(createTimestamp);
                    

                    UserConfig member = new UserConfig();

                    member.cn = cn;
                    member.sn = sn;
                    member.uid = uid;
                    member.employeeType = empType;
                    member.employeeNumber = empNum;
                    member.CreatedTimestamp = Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    Members.members.Add( member );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"All Members Get Error: {ex.Message}");
            }
        }

        private string PasswordEncode(string password)
        {
            // Generate a random salt
            byte[] salt = GenerateRandomSalt();

            // Combine password and salt
            byte[] combined = CombinePasswordAndSalt(Encoding.UTF8.GetBytes(password), salt);

            // Compute the SHA-1 hash of the combined value
            byte[] hashedPassword = ComputeSHA1Hash(combined);

            // Append salt to the hash
            byte[] saltedHashedPassword = CombineHashAndSalt(hashedPassword, salt);

            // Encode the result in Base64
            return Convert.ToBase64String(saltedHashedPassword);
        }


        // Method to generate a random salt
        private byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[4]; // You can adjust the size of the salt as needed
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Method to combine password and salt
        private byte[] CombinePasswordAndSalt(byte[] password, byte[] salt)
        {
            byte[] combined = new byte[password.Length + salt.Length];
            Buffer.BlockCopy(password, 0, combined, 0, password.Length);
            Buffer.BlockCopy(salt, 0, combined, password.Length, salt.Length);
            return combined;
        }

        // Method to compute the SHA-1 hash of the combined value
        private byte[] ComputeSHA1Hash(byte[] data)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(data);
            }
        }

        // Method to combine hash and salt
        private byte[] CombineHashAndSalt(byte[] hash, byte[] salt)
        {
            byte[] combined = new byte[hash.Length + salt.Length];
            Buffer.BlockCopy(hash, 0, combined, 0, hash.Length);
            Buffer.BlockCopy(salt, 0, combined, hash.Length, salt.Length);
            return combined;
        }

        private DateTimeOffset ParseLdapTimestamp(string ldapTimestamp)
        {
            // Specify the format for LDAP Generalized Time
            string ldapFormat = "yyyyMMddHHmmss.fffZ";

            // Parse the LDAP Generalized Time to DateTimeOffset
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(ldapTimestamp, ldapFormat, null);

            return dateTimeOffset;
        }

        static void AddAttributeModification(List<DirectoryAttributeModification> modifications, string attributeName, string newValue)
        {
            DirectoryAttributeModification mod = new DirectoryAttributeModification
            {
                Operation = DirectoryAttributeOperation.Replace,
                Name = attributeName,
            };

            mod.Add(newValue);

            modifications.Add(mod);
        }

    }
}