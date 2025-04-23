using ApiAzurePracticaExamen.Data;
using ApiAzurePracticaExamen.Models;
using ApiAzurePracticaExamen.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiAzurePracticaExamen.Repositories
{
    public class RepositoryCubos
    {
        private CuboContext context;
        private ServiceStorageBlob service;

        public RepositoryCubos(CuboContext context, ServiceStorageBlob service)
        {
            this.context = context;
            this.service = service;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }

        public async Task<List<Cubo>> GetCubosMarcaAsync(string marca)
        {
            var consulta = from datos in this.context.Cubos
                           where datos.Marca == marca
                           select datos;
            return await consulta.ToListAsync(); ;
        }

        public async Task InsertUsuario(string nombre, string email, string pass, string imagen)
        {
            var maxIdUsuario = (from datos in this.context.Usuarios select datos.IdUsuario).Max();

            int idusuario = maxIdUsuario + 1;

            Usuario usuario = new Usuario();
            usuario.IdUsuario = idusuario;
            usuario.Nombre = nombre;
            usuario.Email = email;
            usuario.Pass = pass;
            usuario.Imagen = imagen;

            this.context.Usuarios.Add(usuario);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> LogInUsuariosAsync(string email, string pass)
        {
            return await this.context.Usuarios.Where(x => x.Email == email && x.Pass == pass).FirstOrDefaultAsync();
        }

        public async Task<List<Compra>> GetPedidosUsuarioAsync(int id)
        {
            return await this.context.Compras
                .Where(x => x.IdUsuario == id)
                .ToListAsync();
        }

        public async Task InsertarPedido(int idcubo, int idusuario)
        {
            var maxIdPedido = (from datos in this.context.Compras
                               select datos.IdPedido).Max();

            int idpedido = maxIdPedido + 1;
            DateTime fecha = DateTime.Now;

            Compra pedido = new Compra();
            pedido.IdPedido = idpedido;
            pedido.IdCubo = idcubo;
            pedido.IdUsuario = idusuario;
            pedido.Fecha = fecha;

            this.context.Compras.Add(pedido);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Cubo>> GetCubosBlobAsync()
        {
            List<Cubo> cubos = await this.context.Cubos.ToListAsync();
            string containerUrl = this.service.GetContainerUrl("cubo");

            foreach (Cubo c in cubos)
            {
                if (!c.Imagen.StartsWith("http"))
                {
                    string imagePath = c.Imagen;
                    if (!imagePath.StartsWith("CUBOS/"))
                    {
                        imagePath = "CUBOS/" + imagePath;
                    }

                    c.Imagen = containerUrl + "/" + imagePath;
                }
            }

            return cubos;
        }

        public async Task<UsuarioModel> PerfilBlobAsync(int id)
        {
            Usuario usuario = await this.context.Usuarios
                .Where(x => x.IdUsuario == id)
                .FirstOrDefaultAsync();

            UsuarioModel model = new UsuarioModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Imagen = usuario.Imagen
            };

            if (!string.IsNullOrEmpty(usuario.Imagen))
            {
                string containerUrl = this.service.GetContainerUrl("cubo");

                if (!usuario.Imagen.StartsWith("http"))
                {
                    string imagePath = usuario.Imagen;
                    if (!imagePath.StartsWith("USUARIOS/"))
                    {
                        imagePath = "USUARIOS/" + imagePath;
                    }

                    model.Imagen = containerUrl + "/" + imagePath;
                }
                else
                {
                    model.Imagen = usuario.Imagen;
                }
            }

            return model;
        }

    }
}
