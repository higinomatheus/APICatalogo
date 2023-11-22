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

[Produces("application/json")]
[Route("categorias")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[EnableCors("PermitirApiRequest")]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork context, IMapper mapper)
    {
        _uow = context;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get(
        [FromQuery] CategoriasParameters categoriasParameters
    )
    {
        try
        {
            var categorias = await _uow.CategoriaRepository.GetCategorias(categoriasParameters);

            if (categorias is null)
            {
                return NotFound("Categorias não encontradas");
            }

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return Ok(categoriasDto);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar sua solicitação.");
        }
    }

    /// <summary>
    /// Obter uma Categoria pelo seu Id
    /// </summary>
    /// <param name="id">codigo da categoria</param>
    /// <returns>Objetos Categoria</returns>
    //[EnableCors("PermitirApiRequest")]
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _uow.CategoriaRepository.GetById(c => c.CategoriaId == id);

        if (categoria == null)
        {
            return NotFound($"Categoria com id = {id} não encontrada");
        }

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDto);
    }

    [HttpGet("produtos")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
    {
        var categorias = await _uow.CategoriaRepository.GetCategoriasProdutos();

        var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

        return categoriasDto;
    }

    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request
    ///     POST /categorias
    ///     {
    ///         "categoriaId": 1,
    ///         "nome": "categoria1",
    ///         "imagemUrl": "http://teste.net/1.jpg"
    ///     }
    /// </remarks>
    /// <param name="categoriaDto">objeto Categoria</param>
    /// <returns>O objeto Categoria incluída</returns>
    /// <remarks>Retorna um objeto Categoria incluído</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDTO>> Post([FromBody] CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
            return BadRequest("Dados inválidos");

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _uow.CategoriaRepository.Add(categoria);
        await _uow.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaDTO.CategoriaId }, categoriaDTO);

    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            return BadRequest("Dados inválidos");
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _uow.CategoriaRepository.Update(categoria);
        await _uow.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uow.CategoriaRepository.GetById(c => c.CategoriaId == id);

        if (categoria is null)
        {
            return NotFound($"Categoria com id = {id} não encontrada");
        }

        _uow.CategoriaRepository.Delete(categoria);
        await _uow.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }
}
