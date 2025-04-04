using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Core.Messaging;
using WebApiDotNet.Infrastructure.Data;

namespace WebApiDotNet.Features.Usuarios;

public static class ExcluirUsuario
{
    public class Command : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class Handler(ApplicationDbContext dbContext) : BaseHandler<Command, Result>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public override async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(request.Id, cancellationToken);
            if (usuario is null)
            {
                AdicionarErro("Usuário não encontrado");
                return Error(HttpStatusCode.NotFound);
            }

            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return Success();
        }
    }
}
