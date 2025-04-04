using System.Net;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Core.Entities;
using WebApiDotNet.Core.Enums.Usuario;
using WebApiDotNet.Core.Messaging;
using WebApiDotNet.Infrastructure.Data;

namespace WebApiDotNet.Features.Usuarios;

public static class AtualizarUsuario
{
    public class Command : BaseRequest<Result<Response>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public TipoUsuario Tipo { get; set; }
        public override bool EhValido()
        {
            ValidationResult = new CommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O nome do usuário não pode estar vazia");

            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("O email do usuário não pode estar vazia");
        }
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
            if (!request.EhValido())
            {
                AdicionarErros(request.ValidationResult);
                return Error<Response>();
            }

            var usuario = await _dbContext.Usuarios.FindAsync(request.Id, cancellationToken);
            if (usuario is null)
            {
                AdicionarErro("Usuário não encontrado");
                return Error<Response>(HttpStatusCode.NotFound);
            }

            var nomeExiste = await _dbContext.Usuarios.AnyAsync(c => c.Nome == request.Nome && c.Id != request.Id, cancellationToken);
            var emailExiste = await _dbContext.Usuarios.AnyAsync(c => c.Email == request.Email && c.Id != request.Id, cancellationToken);

            if (nomeExiste)
            {
                AdicionarErro("O nome do usuário já existe");
                return Error<Response>(HttpStatusCode.Conflict);
            }
            if (emailExiste)
            {
                AdicionarErro("O email do usuário já existe");
                return Error<Response>(HttpStatusCode.Conflict);
            }

            usuario.Nome = request.Nome;
            usuario.Email = request.Email;
            usuario.Tipo = request.Tipo;

            _dbContext.Usuarios.Update(usuario);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
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
