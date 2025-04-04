using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Core.Entities;
using WebApiDotNet.Core.Enums.Usuario;
using WebApiDotNet.Core.Messaging;
using WebApiDotNet.Infrastructure.Data;

namespace WebApiDotNet.Features.Usuarios;

public static class AdicionarUsuario
{
    public class Command : BaseRequest<Result<Response>>
    {
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
                .WithMessage("O nome não pode estar vazio");

            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("Email não pode estar vazio");
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

            var nomeExiste = await _dbContext.Usuarios.AnyAsync(c => c.Nome == request.Nome, cancellationToken);
            var emailExiste = await _dbContext.Usuarios.AnyAsync(c => c.Email == request.Email, cancellationToken);

            if (nomeExiste)
            {
                AdicionarErro("O Nome já Existe");
                return Error<Response>(HttpStatusCode.Conflict);
            }
            if (emailExiste)
            {
                AdicionarErro("O Email já Existe");
                return Error<Response>(HttpStatusCode.Conflict);
            }

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                DataCriacao = DateOnly.FromDateTime(DateTime.Now),
                Tipo = request.Tipo,
            };

            await _dbContext.Usuarios.AddAsync(usuario, cancellationToken);
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
