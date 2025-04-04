using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiDotNet.Core.Entities;
using WebApiDotNet.Core.Enums.Usuario;

namespace WebApiDotNet.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(c => c.Tipo)
            .HasConversion(new EnumToStringConverter<TipoUsuario>());
    }
}
