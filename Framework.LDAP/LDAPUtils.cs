using Framework.LDAP.Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;

namespace Framework.LDAP
{
    public class LDAPUtils
    {
        private string _dominio = string.Empty;
        private readonly string _usuario;
        private readonly string _password;
        private readonly DirectoryEntry _directoryEntry;

        private const string _filtroGrupos = "(&(objectClass=user)(objectCategory=person))";
        private string _filtroUsuarios = "sAMAccountName=";

        public LDAPUtils(string pUserName, string pPassword, string pDominio)
        {
            _usuario = pUserName;
            _filtroUsuarios = string.Concat(_filtroUsuarios, pUserName);
            _password = pPassword;
            _dominio = pDominio;
            _directoryEntry = new DirectoryEntry() { Path = _dominio, Username = _usuario, Password = _password, AuthenticationType = AuthenticationTypes.Secure };
        }        

        public void SetDominio(string pDominio)
        {
            _dominio = pDominio;
        }

        public LDAPUser LoginUsuario()
        {
            try
            {
                SearchResult result = GetDirectorySearcher(_filtroUsuarios).FindOne();
                return ConvertToLDAPUser(result);
            }
            catch (DirectoryServicesCOMException)
            {
                //Usuario y cave incorrecta
                return null;
            }
            catch (COMException comex)
            {
                //Error de conexión
                throw comex;
            }
        }

        public List<LDAPUser> GetAllUsers(string pDominio)
        {
            _dominio = pDominio;
            return GetAllUsers();
        }

        public List<LDAPUser> GetAllUsers()
        {
            List<LDAPUser> usuarios = new List<LDAPUser>();

            SearchResultCollection resultCol = GetDirectorySearcher(_filtroGrupos).FindAll();

            if (resultCol != null)
            {
                for (int counter = 0; counter < resultCol.Count; counter++)
                {
                    var result = resultCol[counter];
                    if (result.Properties.Contains("sn") && result.Properties.Contains("givenname") && result.Properties.Contains("samaccountname"))
                    {
                        LDAPUser user = ConvertToLDAPUser(result);
                        usuarios.Add(user);
                    }
                }
            }
            return usuarios.OrderBy(u => u.Nombre).ThenBy(u => u.Apellido).ToList();
        }

        public void ResetPassword(string pOldPassword, string pNewPassword)
        {
            try
            {
                SearchResult sr = GetDirectorySearcher(_filtroUsuarios).FindOne();
                if (sr != null)
                {
                    DirectoryEntry userdirectory = sr.GetDirectoryEntry();
                    userdirectory.Invoke("ChangePassword", new object[] { pOldPassword, pNewPassword });
                    userdirectory.CommitChanges();
                    userdirectory.RefreshCache();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private LDAPUser ConvertToLDAPUser(SearchResult pResult)
        {
            if (pResult != null)
            {
                bool activo = IsActive(pResult.GetDirectoryEntry());
                DateTime lastModification = DateTime.Parse(pResult.Properties["whenchanged"][0].ToString());
                DateTime? deleteDate = null;

                if (!activo)
                {
                    deleteDate = lastModification;
                }

                var nombre = pResult.Properties["name"].Count > 0 ? pResult.Properties["name"][0].ToString() : string.Empty;
                var apellido = pResult.Properties["sn"].Count > 0 ? pResult.Properties["sn"][0].ToString() : string.Empty;
                var email = pResult.Properties["mail"].Count > 0 ? pResult.Properties["mail"][0].ToString() : string.Empty;
                var telefono = pResult.Properties["telephonenumber"].Count > 0 ? pResult.Properties["telephonenumber"][0].ToString() : string.Empty;
                var fechaCreacion = pResult.Properties["whencreated"].Count > 0 ? pResult.Properties["whencreated"][0].ToString() : string.Empty;
                var fechaUltimoLogin = pResult.Properties["whenchanged"].Count > 0 ? pResult.Properties["whenchanged"][0].ToString() : string.Empty;
                var nombreAD = pResult.Properties["samaccountname"].Count > 0 ? pResult.Properties["samaccountname"][0].ToString() : string.Empty;

                return new LDAPUser()
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Telefono = telefono,
                    FechaCreacion = DateTime.Parse(fechaCreacion),
                    FechaUltimoLogin = DateTime.Parse(fechaUltimoLogin),
                    NombreAD = nombreAD,
                    Activo = activo,
                    FechaBaja = deleteDate
                };
            }

            return null;
        }

        private bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        private DirectorySearcher GetDirectorySearcher(string pFiltro)
        {
            string strSearch = pFiltro;
            DirectorySearcher dsSystem = new DirectorySearcher(_directoryEntry, strSearch)
            {
                SearchScope = SearchScope.Subtree
            };

            return dsSystem;
        }
    }
}
