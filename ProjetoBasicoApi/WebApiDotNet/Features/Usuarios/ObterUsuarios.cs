using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Core.Entities;
using WebApiDotNet.Core.Enums.Usuario;
using WebApiDotNet.Core.Messaging;
using WebApiDotNet.Core.Paginator;
using WebApiDotNet.Infrastructure.Data;

namespace WebApiDotNet.Features.Usuarios;

public static class ObterUsuarios
{
    public class Command : QueryParams, IRequest<Result<QueryResponse<Response>>>
    {
        public string Pesquisa { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateOnly DataCriacao { get; set; }
        public TipoUsuario Tipo { get; set; }
    }

    public class Handler(ApplicationDbContext dbContext, IMapper mapper) : BaseHandler<Command, Result<QueryResponse<Response>>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        public override async Task<Result<QueryResponse<Response>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(request.Pesquisa))
            {
                query = query
                    .Where(c => c.Nome.Contains(request.Pesquisa)
                    || c.Email.Contains(request.Pesquisa)
                    || c.Tipo.ToString().Contains(request.Pesquisa)
                    || c.DataCriacao.ToString().Contains(request.Pesquisa));
            }

            query = query.ApplyPagination(request, out var count);

            var response = query.ProjectTo<Response>(_mapper.ConfigurationProvider);

            return Success(new QueryResponse<Response>
            {
                Items = await response.ToListAsync(cancellationToken),
                Count = count,
                Page = request.Page.Value,
                PageSize = request.PageSize
            });
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
