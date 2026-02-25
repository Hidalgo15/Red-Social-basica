using AutoMapper;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using BookFace.Core.Application.ViewModel.ViewModel.Comentario;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Application.AutoMapper
{
    public class ComentarioProfile :Profile
    {
        public ComentarioProfile()
        {
            // Mapeo de Entidad Comentario a ComentarioViewModel
            // AutoMapper puede manejar el mapeo de la colección 'Respuestas' de forma recursiva
            // siempre y cuando los objetos anidados estén cargados desde la base de datos (con .Include()).
            CreateMap<Comentario, ComentarioViewModel>();

            // Mapeo de ComentarioSaveViewModel a Entidad Comentario
            // Se usa ConstructUsing para invocar el constructor de la entidad, asegurando que
            // las validaciones y lógicas de inicialización se ejecuten.
            CreateMap<ComentarioSaveViewModel, Comentario>()
                .ConstructUsing(src => new Comentario(
                    src.Contenido,
                    src.UsuarioId,
                    src.PublicacionId,
                    src.ComentarioPadreId
                ));
        }
    }
}
