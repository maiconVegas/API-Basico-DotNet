using System.ComponentModel;

namespace WebApiDotNet.Core.Enums.Usuario;

public enum TipoUsuario
{
    [Description("Administrador")]
    Administrador,

    [Description("Convidado")]
    Convidado,

    [Description("Comum")]
    Comum
}
