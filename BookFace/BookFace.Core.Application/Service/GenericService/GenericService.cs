using AutoMapper;
using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Domain.IRepository;

namespace BookFace.Core.Application.Service.GenericService
{
    public class GenericService<TEntity, TSaveViewModel, TViewModel> : IGenericService<TSaveViewModel, TViewModel>
         where TEntity : class
         where TSaveViewModel : class
         where TViewModel : class
    {
        private readonly IRepository<TEntity> _repository; // Inyección de dependencia del repositorio genérico
        private readonly IMapper _mapper; // Inyección de dependencia de AutoMapper

        public GenericService(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Método para agregar una nueva entidad
        public virtual async Task<TSaveViewModel> Add(TSaveViewModel vm)
        {
            // Mapea el SaveViewModel de entrada a la entidad de dominio
            var entity = _mapper.Map<TEntity>(vm);

            // Guarda la entidad en la base de datos a través del repositorio
            entity = await _repository.AddAsync(entity);

            // Mapea la entidad guardada (que ahora tendrá su ID y otros valores generados)
            // de vuelta a un SaveViewModel para retornarlo
            return _mapper.Map<TSaveViewModel>(entity);
        }

        // Método para eliminar una entidad por ID
        public virtual async Task Delete(int id)
        {
            // Llama al método DeleteAsync del repositorio
            await _repository.DeleteAsync(id);
        }

        // Método para obtener todas las entidades y mapearlas a ViewModels
        public virtual async Task<List<TViewModel>> GetAllViewModel()
        {
            // Obtiene todas las entidades del repositorio
            var entities = await _repository.GetAllAsync();

            // Mapea la lista de entidades a una lista de ViewModels para retornarla
            return _mapper.Map<List<TViewModel>>(entities);
        }

        // Método para obtener una entidad por ID y mapearla a un SaveViewModel (útil para edición)
        public virtual async Task<TSaveViewModel> GetByIdSaveViewModel(int id)
        {
            // Obtiene la entidad del repositorio
            var entity = await _repository.GetByIdAsync(id);

            // Mapea la entidad a un SaveViewModel para retornarlo
            return _mapper.Map<TSaveViewModel>(entity);
        }

        // Método para actualizar una entidad existente
        public virtual async Task Update(TSaveViewModel vm)
        {
           
            var entityToUpdate = _mapper.Map<TEntity>(vm);

            // Llama al método UpdateAsync del repositorio
            await _repository.UpdateAsync(entityToUpdate);
        }

        public virtual async Task<TViewModel> GetByIdViewModel(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                // Es buena práctica lanzar una excepción aquí si el recurso no se encuentra
                // para que el controlador pueda devolver un 404 Not Found.
                throw new KeyNotFoundException($"No se encontró la entidad con ID {id}.");
            }
            return _mapper.Map<TViewModel>(entity);
        }
    }
}
