using BookFace.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BookFace.Infraestructure.Persistence.DBContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        //public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Amistad> Amistades { get; set; }
        public DbSet<SolicitudAmistad> SolicitudesAmistad { get; set; }
        public DbSet<Reaccion> Reacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             #region Publicacion (relación interna de dominio)
            // Una Publicacion tiene muchos Comentarios
            modelBuilder.Entity<Publicacion>()
                .HasMany(p => p.Comentarios)
                .WithOne(c => c.Publicacion)
                .HasForeignKey(c => c.PublicacionId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Publicacion, sus Comentarios también se eliminan

            // Una Publicacion tiene muchas Reacciones
            modelBuilder.Entity<Publicacion>()
                .HasMany(p => p.Reacciones) // Publicacion tiene una colección de Reacciones
                .WithOne() // Reaccion pertenece a una Publicacion (pero Reaccion no tiene una propiedad de navegación de vuelta)
                .HasForeignKey(r => r.PublicacionId) // La FK en Reaccion es PublicacionId
                .OnDelete(DeleteBehavior.Cascade); // Generalmente, las reacciones se eliminan cuando se elimina la publicación
            #endregion

            #region Comentario (relación interna de dominio)
            // Comentario anidado: Un Comentario puede tener muchas Respuestas
            modelBuilder.Entity<Comentario>()
                .HasMany(c => c.Respuestas)
                .WithOne(c => c.ComentarioPadre)
                .HasForeignKey(c => c.ComentarioPadreId)
                .IsRequired(false) // La FK es opcional (un comentario puede no tener padre)
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminaciones en cascada complejas
            #endregion

            #region Amistad (SOLO reglas de unicidad, NO relaciones con Usuario/ApplicationUser)
            // --- Unicidad de la amistad ---
            // Asegura que no haya amistades duplicadas para el mismo par de usuarios.
            modelBuilder.Entity<Amistad>()
                .HasIndex(a => new { a.UsuarioId1, a.UsuarioId2 })
                .IsUnique();
            #endregion

            #region SolicitudAmistad (SOLO reglas de unicidad, NO relaciones con Usuario/ApplicationUser)
            // --- Unicidad de la solicitud ---
            // Para asegurar que un usuario no pueda enviar múltiples solicitudes pendientes al mismo destinatario.
            modelBuilder.Entity<SolicitudAmistad>()
                .HasIndex(s => new { s.RemitenteId, s.ReceptorId, s.Estado })
                .IsUnique()
                .HasFilter("[Estado] = 0"); // Un filtro para la unicidad: solo si el estado es 'Pendiente' (0 en el enum)
            #endregion

            #region Reaccion (la relación de Reaccion a Publicacion ya está en #region Publicacion.
            
            #endregion
        }
    }
}