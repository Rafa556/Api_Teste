using System;
using System.Collections.Generic;
using System.Linq;
using API_REST.Data;
using API_REST.HATEOAS;
using API_REST.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_REST.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database){
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO","GET");
            HATEOAS.AddAction("DELETE_PRODUCT","DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT","PATCH");
        }
        [HttpGet]
        public IActionResult Get(){
            var produtos = database.Produtos.ToList();
            List<ProdutoContainer> produtosHATEOAS = new List<ProdutoContainer>();
            foreach(var prod in produtos){
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = prod;
                produtoHATEOAS.links = HATEOAS.GetActions(prod.Id.ToString());
                produtosHATEOAS.Add(produtoHATEOAS);

            }
            return Ok(produtosHATEOAS); //Status code = 200 && dados
        }
        [HttpGet("{Id}")]
        public IActionResult Get(int id){
            try{
            Produto produto = database.Produtos.First(p => p.Id == id);
            ProdutoContainer produtoHATEOAS = new ProdutoContainer();
            produtoHATEOAS.produto = produto;
            produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());
            return Ok(produtoHATEOAS);
            }catch(Exception){ 
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp){

            if(pTemp.Preco <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "O preço não pode ser menor de 0"});
            }

            if(pTemp.Nome.Length <= 1){
                Response.StatusCode = 400;
                return new ObjectResult(new{msg="O nome do produto precisa ter mais caracteres"});
            }
            Produto p = new Produto();
            
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges(); 

            Response.StatusCode = 201;
            return new ObjectResult(new{msg = "produto salvo"});
           // return Ok(new );
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            try{    
                Produto produto = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produto);
                database.SaveChanges();
                return Ok(produto);
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }
        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto){
            if(produto.Id > 0){
                try{
                var p = database.Produtos.First(pTemp => pTemp.Id == produto.Id);
                if(p != null){

                    p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                    p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;

                    database.SaveChanges();
                    return Ok();

                }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "O preço não pode ser menor de 0"});
                }
                }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "O preço não pode ser menor de 0"});
                }

            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "O preço não pode ser menor de 0"});

            }
        }

        public class ProdutoTemp{
            public string Nome {get;set;}
            public float Preco {get;set;}
        }

        public class ProdutoContainer{
            public Produto produto {get; set;}
            public Link[] links{get; set;}
        }
    }
}