using WebApiDotNet.Core.Enums.Usuario;

namespace WebApiDotNet.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateOnly DataCriacao { get; set; }
    public TipoUsuario Tipo { get; set; }
}
