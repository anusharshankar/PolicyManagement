﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using PolicyLDAP.Identity;
using PolicyLDAP.Settings;

namespace PolicyLDAP.Services
{
    public class LdapService : ILdapService
    {
        private readonly string _searchBase;

        private readonly LdapSettings _ldapSettings;

        private readonly string[] _attributes =
        {
            "objectSid", "objectGUID", "objectCategory", "objectClass", "memberOf", "name", "cn", "distinguishedName",
            "sAMAccountName", "sAMAccountName", "userPrincipalName", "displayName", "givenName", "sn", "description",
            "telephoneNumber", "mail", "streetAddress", "postalCode", "l", "st", "co", "c"
        };

        public LdapService(IOptions<LdapSettings> ldapSettingsOptions)
        {
            this._ldapSettings = ldapSettingsOptions.Value;
            this._searchBase = this._ldapSettings.SearchBase;
        }

        private ILdapConnection GetConnection()
        {
            var ldapConnection = new LdapConnection() { SecureSocketLayer = this._ldapSettings.UseSSL };

            //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
            ldapConnection.Connect(this._ldapSettings.ServerName, this._ldapSettings.ServerPort);
            //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
            ldapConnection.Bind(this._ldapSettings.Credentials.DomainUserName, this._ldapSettings.Credentials.Password);

            return ldapConnection;
        }

        public ICollection<Identity.LdapEntry> GetGroups(string groupName, bool getChildGroups = false)
        {
            var groups = new List<Identity.LdapEntry>();

            var filter = $"(&(objectClass=group)(cn={groupName}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    this._attributes,
                    false,
                    null,
                    null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    var entry = searchResultMessage.Entry;

                    groups.Add(this.CreateEntryFromAttributes(entry.DN, entry.getAttributeSet()));

                    if (!getChildGroups)
                    {
                        continue;
                    }

                    foreach (var child in this.GetChildren<Identity.LdapEntry>(string.Empty, entry.DN))
                    {
                        groups.Add(child);
                    }
                }
            }

            return groups;
        }

        public ICollection<LdapUser> GetAllUsers()
        {
            return this.GetUsersInGroups(null);
        }

        public ICollection<LdapUser> GetUsersInGroup(string group)
        {
            return this.GetUsersInGroups(this.GetGroups(group));
        }

        public ICollection<LdapUser> GetUsersInGroups(ICollection<Identity.LdapEntry> groups)
        {
            var users = new List<LdapUser>();

            if (groups == null || !groups.Any())
            {
                users.AddRange(this.GetChildren<LdapUser>(this._searchBase));
            }
            else
            {
                foreach (var group in groups)
                {
                    users.AddRange(this.GetChildren<LdapUser>(this._searchBase, @group.DistinguishedName));
                }
            }

            return users;
        }

        public ICollection<LdapUser> GetUsersByEmailAddress(string emailAddress)
        {
            var users = new Collection<LdapUser>();

            var filter = $"(&(objectClass=user)(mail={emailAddress}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    this._attributes,
                    false, null, null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    users.Add(this.CreateUserFromAttributes(this._searchBase,
                        searchResultMessage.Entry.getAttributeSet()));
                }
            }

            return users;
        }

        public LdapUser GetUserByUserName(string userName)
        {
            LdapUser user = null;

            var filter = $"(&(objectClass=user)(name={userName}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    this._attributes,
                    false,
                    null,
                    null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    user = this.CreateUserFromAttributes(this._searchBase, searchResultMessage.Entry.getAttributeSet());
                }
            }

            return user;
        }

        public LdapUser GetAdministrator()
        {
            var name = this._ldapSettings.Credentials.DomainUserName.Substring(
                this._ldapSettings.Credentials.DomainUserName.IndexOf("\\", StringComparison.Ordinal) != -1
                    ? this._ldapSettings.Credentials.DomainUserName.IndexOf("\\", StringComparison.Ordinal) + 1
                    : 0);

            return this.GetUserByUserName(name);
        }

        public void AddUser(LdapUser user, string password)
        {
            var dn = $"CN={user.FirstName} {user.LastName},{this._ldapSettings.ContainerName}";

            var attributeSet = new LdapAttributeSet
            {
                new LdapAttribute("instanceType", "4"),
                new LdapAttribute("objectCategory", $"CN=Person,CN=Schema,CN=Configuration,{this._ldapSettings.DomainDistinguishedName}"),
                new LdapAttribute("objectClass", new[] {"top", "person", "organizationalPerson", "user"}),
                //new LdapAttribute("name", user.UserName),
                new LdapAttribute("cn", $"{user.FirstName} {user.LastName}"),
                //new LdapAttribute("sAMAccountName", user.UserName),
                //new LdapAttribute("userPrincipalName", user.UserName),
                new LdapAttribute("unicodePwd", Convert.ToBase64String(Encoding.Unicode.GetBytes($"\"{user.Password}\""))),
                new LdapAttribute("userAccountControl", user.MustChangePasswordOnNextLogon ? "544" : "512"),
                new LdapAttribute("givenName", user.FirstName),
                new LdapAttribute("sn", user.LastName),
                new LdapAttribute("mail", user.EmailAddress)
            };

            if (user.DisplayName != null)
            {
                attributeSet.Add(new LdapAttribute("displayName", user.DisplayName));
            }

            if (user.Description != null)
            {
                attributeSet.Add(new LdapAttribute("description", user.Description));
            }
            if (user.Phone != null)
            {
                attributeSet.Add(new LdapAttribute("telephoneNumber", user.Phone));
            }
            

            var newEntry = new Novell.Directory.Ldap.LdapEntry(dn, attributeSet);

            using (var ldapConnection = this.GetConnection())
            {
                ldapConnection.Add(newEntry);
            }
        }

        public void DeleteUser(string distinguishedName)
        {
            using (var ldapConnection = this.GetConnection())
            {
                ldapConnection.Delete(distinguishedName);
            }
        }

        public bool Authenticate(string distinguishedName, string password)
        {
            using (var ldapConnection = new LdapConnection() { SecureSocketLayer = true })
            {
                ldapConnection.Connect(this._ldapSettings.ServerName, this._ldapSettings.ServerPort);

                try
                {
                    ldapConnection.Bind(distinguishedName, password);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private IList<T> GetChildren<T>(string searchBase, string groupDistinguishedName = null)
            where T : ILdapEntry, new()
        {
            var entries = new List<T>();

            var objectCategory = "*";
            var objectClass = "*";

            if (typeof(T) == typeof(Identity.LdapEntry))
            {
                objectClass = "group";
                objectCategory = "group";

                entries = GetChildren(_searchBase, groupDistinguishedName, objectCategory, objectClass)
                    .Cast<T>().ToList();

            }

            if (typeof(T) == typeof(LdapUser))
            {
                objectCategory = "person";
                objectClass = "user";

                entries = GetChildren(_searchBase, null, objectCategory, objectClass).Cast<T>()
                    .ToList();

            }

            return entries;
        }

        private ICollection<ILdapEntry> GetChildren(string searchBase, string groupDistinguishedName = null,
            string objectCategory = "*", string objectClass = "*")
        {
            var allChildren = new Collection<ILdapEntry>();

            var filter = string.IsNullOrEmpty(groupDistinguishedName)
                ? $"(&(objectCategory={objectCategory})(objectClass={objectClass}))"
                : $"(&(objectCategory={objectCategory})(objectClass={objectClass})(memberOf={groupDistinguishedName}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    this._attributes,
                    false,
                    null,
                    null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    var entry = searchResultMessage.Entry;

                    if (objectClass == "group")
                    {
                        allChildren.Add(this.CreateEntryFromAttributes(entry.DN, entry.getAttributeSet()));

                        foreach (var child in this.GetChildren(string.Empty, entry.DN, objectCategory, objectClass))
                        {
                            allChildren.Add(child);
                        }
                    }

                    if (objectClass == "user")
                    {
                        allChildren.Add(this.CreateUserFromAttributes(entry.DN, entry.getAttributeSet()));
                    }

                    ;
                }
            }

            return allChildren;
        }

        private LdapUser CreateUserFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            var ldapUser = new LdapUser
            {
                ObjectSid = attributeSet.getAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.getAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.getAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.getAttribute("objectClass")?.StringValue,
                IsDomainAdmin = attributeSet.getAttribute("memberOf") != null && attributeSet.getAttribute("memberOf").StringValueArray.Contains("CN=Domain Admins," + this._ldapSettings.SearchBase),
                MemberOf = attributeSet.getAttribute("memberOf")?.StringValueArray,
                CommonName = attributeSet.getAttribute("cn")?.StringValue,
                //UserName = attributeSet.getAttribute("name")?.StringValue,
                SamAccountName = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                UserPrincipalName = attributeSet.getAttribute("userPrincipalName")?.StringValue,
                Name = attributeSet.getAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.getAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                DisplayName = attributeSet.getAttribute("displayName")?.StringValue,
                FirstName = attributeSet.getAttribute("givenName")?.StringValue,
                LastName = attributeSet.getAttribute("sn")?.StringValue,
                Description = attributeSet.getAttribute("description")?.StringValue,
                Phone = attributeSet.getAttribute("telephoneNumber")?.StringValue,
                EmailAddress = attributeSet.getAttribute("mail")?.StringValue,
                

                SamAccountType = int.Parse(attributeSet.getAttribute("sAMAccountType")?.StringValue ?? "0"),
            };

            return ldapUser;
        }

        private Identity.LdapEntry CreateEntryFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            return new Identity.LdapEntry
            {
                ObjectSid = attributeSet.getAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.getAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.getAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.getAttribute("objectClass")?.StringValue,
                CommonName = attributeSet.getAttribute("cn")?.StringValue,
                Name = attributeSet.getAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.getAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                SamAccountName = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                SamAccountType = int.Parse(attributeSet.getAttribute("sAMAccountType")?.StringValue ?? "0"),
            };
        }

        ICollection<LdapUser> ILdapService.GetUsersInGroups(ICollection<Identity.LdapEntry> groups)
        {
            throw new NotImplementedException();
        }
    }
}
