using System;

namespace Framework.LDAP.Model
{
    public class LDAPUser
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string NombreAD { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaUltimoLogin { get; set; }

        public DateTime? FechaBaja { get; set; }

        public bool Activo { get; set; }
    }
}
