using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using API_REST.Data;
using API_REST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_REST.bin
{
        [Route("api/v1/[controller]")]
        [ApiController]
        public class UsuariosController : ControllerBase
        {
        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database){
            this.database = database;}



            [HttpPost("registro")]
            public IActionResult Registro([FromBody] Usuario usuario){
                database.Add(usuario);
                database.SaveChanges();
                return Ok(new{msg="Usuario cadastrado!"}); 
            }
            [HttpPost("login")]
            public IActionResult Login([FromBody] Usuario credenciais){
                
                try{
                Usuario usuario = database.Usuarios.First(user => user.Email.Equals(credenciais.Email));

                if(usuario != null){
                    if(usuario.Senha.Equals(credenciais.Senha)){

                        string chaveDeSeguranca = "HE_HE_HE";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca)); 
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);

                        var JWT = new JwtSecurityToken(
                            issuer: "Sonmarket.com",
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));

                    }else{
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                    }

                }else{
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                
                }
            }catch(Exception){
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                
            }
            
        }
    }
}
