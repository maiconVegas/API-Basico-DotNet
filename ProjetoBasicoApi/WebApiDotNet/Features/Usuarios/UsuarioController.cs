using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiDotNet.Core.Controllers;
using WebApiDotNet.Core.Paginator;

namespace WebApiDotNet.Features.Usuarios;

[Route("api/usuarios")]
public class UsuarioController : MainController
{
    private readonly IMediator _mediator;
    public UsuarioController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(QueryResponse<ObterUsuarios.Response>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ObterUsuarios(string pesquisa, string orderBy, OrderDirection orderDirection, int? page, int? pageSize)
    {
        var result = await _mediator.Send(new ObterUsuarios.Command
        {
            Pesquisa = pesquisa,
            OrderBy = orderBy,
            Page = page,
            Direction = orderDirection,
            PageSize = pageSize
        });
        return CustomResponse(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ObterUsuario.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ObterUsuario(int id)
    {
        var result = await _mediator.Send(new ObterUsuario.Command
        {
            Id = id
        });
        return CustomResponse(result);
    }


    [HttpPost]
    [ProducesResponseType(typeof(AdicionarUsuario.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AdicionarUsuario(AdicionarUsuario.Command command)
    {
        var result = await _mediator.Send(command);
        return CustomResponse(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(AtualizarUsuario.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> AtualizarUsuario(int id, AtualizarUsuario.Command command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return CustomResponse(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeletarUsuario(int id)
    {
        var resultado = await _mediator.Send(new ExcluirUsuario.Command
        {
            Id = id
        });
        return CustomResponse(resultado);
    }

}
