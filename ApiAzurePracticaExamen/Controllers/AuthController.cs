using ApiAzurePracticaExamen.Helpers;
using ApiAzurePracticaExamen.Models;
using ApiAzurePracticaExamen.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiAzurePracticaExamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperActionServicesOAuth helper;

        public AuthController(RepositoryCubos repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario usuario = await this.repo.LogInUsuariosAsync(model.Email, model.Pass);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);


                UsuarioModel modeUsuario = new UsuarioModel();
                modeUsuario.IdUsuario = usuario.IdUsuario;
                modeUsuario.Nombre = usuario.Nombre;
                modeUsuario.Email = usuario.Email;
                modeUsuario.Imagen = usuario.Imagen;

                string jsonUsuario = JsonConvert.SerializeObject(modeUsuario);
                string jsonCrifado = HelperCryptography.EncryptString(jsonUsuario);
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonUsuario)
                };

                JwtSecurityToken token = new JwtSecurityToken(
                    claims: informacion,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    notBefore: DateTime.UtcNow
                    );
                //POR ULTIMO DEVOLVEMOS LA RESPUESTA AFIRMATIVA CON UN OBJETO QUE CONTENGA EL TOKEN (ANONIMO)
                return Ok(
                    new
                    {
                        response = new JwtSecurityTokenHandler().WriteToken(token)
                    });
            }
        }
    }
}
