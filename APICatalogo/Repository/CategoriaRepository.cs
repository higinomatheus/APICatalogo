﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters)
    {

        return await PagedList<Categoria>.ToPagedList(
            Get().OrderBy(on => on.CategoriaId),
            categoriasParameters.PageNumber,
            categoriasParameters.PageSize
        );
    }


    public async Task<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        return await Get().Include(c => c.Produtos).ToListAsync();
    }
}