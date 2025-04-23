using ApiAzurePracticaExamen.Data;
using ApiAzurePracticaExamen.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAzurePracticaExamen.Repositories
{
    public class RepositoryCubos
    {
        private CuboContext context;
      

        public RepositoryCubos(CuboContext context)
        {
            this.context = context;
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
    }
}
