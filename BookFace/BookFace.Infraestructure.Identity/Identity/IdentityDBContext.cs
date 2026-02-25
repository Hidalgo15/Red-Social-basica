using BookFace.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Identity.Identity
{
    public class IdentityDBContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public IdentityDBContext(DbContextOptions<IdentityDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // MUY IMPORTANTE: Llama al base para que Identity configure sus tablas
            // Puedes añadir personalizaciones adicionales del modelo para las tablas de Identity aquí si es necesario
            // Por ejemplo, para configurar propiedades adicionales en ApplicationUser
            builder.HasDefaultSchema("Identity"); // Configura el esquema por defecto para las tablas de Identity   
            
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Usuarios"); // Cambia el nombre de la tabla si es necesario
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FotoPerfilUrl).HasMaxLength(255); // Ajusta la longitud si es necesario
                entity.Property(e => e.Telefono).HasMaxLength(20);    // Ajusta la longitud si es necesario
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.EstaActivo).IsRequired();

                // *** Relaciones de ApplicationUser con tus entidades de dominio ***
                // Estas configuraciones establecen las relaciones ONE-TO-MANY
                // donde ApplicationUser es el "ONE" y tus entidades de dominio son el "MANY".
                // No especificamos la propiedad de navegación inversa (WithOne()) porque
                // Publicacion, Comentario, etc., no tienen una propiedad ApplicationUser en su modelo de dominio
                // si estamos manteniendo la separación de contextos, solo el FK (UsuarioId).

                // ApplicationUser tiene muchas Publicaciones
                entity.HasMany(u => u.Publicaciones)
                    .WithOne() // Publicacion tiene un Usuario (referencia implícita)
                    .HasForeignKey(p => p.UsuarioId) // La FK en Publicacion que apunta a ApplicationUser
                    .OnDelete(DeleteBehavior.Cascade); // Si se elimina un usuario, sus publicaciones se eliminan

                // ApplicationUser tiene muchos Comentarios
                entity.HasMany(u => u.Comentarios)
                    .WithOne() // Comentario tiene un Usuario
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de comentarios

                // ApplicationUser tiene muchas Amistades como Usuario1
                entity.HasMany(u => u.AmistadesComoUsuario1)
                    .WithOne() // Amistad tiene un Usuario1
                    .HasForeignKey(a => a.UsuarioId1)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de amistades

                // ApplicationUser tiene muchas Amistades como Usuario2
                entity.HasMany(u => u.AmistadesComoUsuario2)
                    .WithOne() // Amistad tiene un Usuario2
                    .HasForeignKey(a => a.UsuarioId2)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de amistades

                // ApplicationUser tiene muchas Solicitudes de Amistad enviadas
                entity.HasMany(u => u.SolicitudesEnviadas)
                    .WithOne() // SolicitudAmistad tiene un Remitente
                    .HasForeignKey(s => s.RemitenteId)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de solicitudes

                // ApplicationUser tiene muchas Solicitudes de Amistad recibidas
                entity.HasMany(u => u.SolicitudesRecibidas)
                    .WithOne() // SolicitudAmistad tiene un Receptor
                    .HasForeignKey(s => s.ReceptorId)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de solicitudes

                // ApplicationUser tiene muchas Reacciones
                entity.HasMany(u => u.Reacciones)
                    .WithOne() // Reaccion tiene un Usuario
                    .HasForeignKey(r => r.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada de reacciones

            });
        }
    }
}