using ApiAzurePracticaExamen.Helpers;
using ApiAzurePracticaExamen.Models;
using ApiAzurePracticaExamen.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiAzurePracticaExamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperUsuarioToken helper;

        public CubosController(RepositoryCubos repo, HelperUsuarioToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> GetCubos()
        {
            return await this.repo.GetCubosAsync();
        }

        [HttpGet("{marca}")]
        public async Task<ActionResult<List<Cubo>>> GetCubosMarca(string marca)
        {
            return await this.repo.GetCubosMarcaAsync(marca);
        }

        [HttpPost]
        public async Task InsertarUsuario(Usuario user)
        {
            await this.repo.InsertUsuario(user.Nombre, user.Email, user.Pass, user.Imagen);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<UsuarioModel>> Perfil()
        {
            UsuarioModel model = this.helper.GetUsuario();
            return model;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<UsuarioModel>> GetCompraUsuario()
        {

            UsuarioModel model = this.helper.GetUsuario();
            var compras = await this.repo.GetPedidosUsuarioAsync(model.IdUsuario);
            return Ok(compras);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task InsertarPedido(int idCubo)
        {
            UsuarioModel model = this.helper.GetUsuario();
            await this.repo.InsertarPedido(model.IdUsuario, idCubo);
        }
    }
}
