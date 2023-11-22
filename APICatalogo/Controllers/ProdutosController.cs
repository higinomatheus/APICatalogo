using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace APICatalogo.Controllers;

[ApiConventionType(typeof(DefaultApiConventions))]
[Produces("application/json")]
[Route("produtos")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork context, IMapper mapper)
    {
        _uow = context;
        _mapper = mapper;
    }

    [HttpGet("menorpreco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
    {
        try
        {
            var produtos = await _uow.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar sua solicitação.");
        }
    }
    /// <summary>
    /// Exibe uma relação dos produtos
    /// </summary>
    /// <returns>Retorna uma lista de objetos Produto</returns>
    // /produtos
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uow.ProdutoRepository.GetProdutos(produtosParameters);

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }

        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

        return produtosDto;
    }

    /// <summary>
    /// Obtem um produto pelo seu identificador produtoId
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>Um objeto produto</returns>
    // api/produtos/1
    [HttpGet("{id:int}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound($"Produto com id = {id} não encontrado");
        }

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }

    /// <summary>
    /// Inclui uma novo produto
    /// </summary>
    /// <remarks>
    /// Exemplo de request
    ///     POST /produtos
    ///     {
    ///         "nome": "Produto1",
    ///         "descricao": "Uma descrição para o Produto1",
    ///         "preco": 200,
    ///         "imagemUrl": "http://teste.net/1.jpg",
    ///         "categoriaId": 1
    ///     }
    /// </remarks>
    /// <param name="produtoDto"></param>
    /// <returns>O objeto Produto incluído</returns>
    /// <remarks>Retorna um objeto Produto incluído</remarks>
    [HttpPost]
    public async Task<ActionResult> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest("Dados inválidos");

        var produto = _mapper.Map<Produto>(produtoDto);
        _uow.ProdutoRepository.Add(produto);
        await _uow.Commit(); // É neste momento que os dados serão efetivamente persistidos na tabela

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTO);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest("Dados inválidos");
        }

        var produto = _mapper.Map<Produto>(produtoDto);

        _uow.ProdutoRepository.Update(produto);
        await _uow.Commit();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound($"Produto com id = {id} não encontrado");
        }

        _uow.ProdutoRepository.Delete(produto);
        await _uow.Commit();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return Ok(produtoDTO);
    }
}
