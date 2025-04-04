using System.Net;
using AutoMapper;
using MediatR;
using WebApiDotNet.Core.Entities;
using WebApiDotNet.Core.Enums.Usuario;
using WebApiDotNet.Core.Messaging;
using WebApiDotNet.Infrastructure.Data;

namespace WebApiDotNet.Features.Usuarios;

public static class ObterUsuario
{
    public class Command : IRequest<Result<Response>>
    {
        public int Id { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateOnly DataCriacao { get; set; }
        public TipoUsuario Tipo { get; set; }
    }

    public class Handler(ApplicationDbContext dbContext, IMapper mapper) : BaseHandler<Command, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(request.Id, cancellationToken);

            if (usuario is null)
            {
                AdicionarErro("Usuário não encontrada");
                return Error<Response>(HttpStatusCode.NotFound);
            }

            var response = _mapper.Map<Response>(usuario);
            return Success(response);
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, Response>();
        }
    }
}
