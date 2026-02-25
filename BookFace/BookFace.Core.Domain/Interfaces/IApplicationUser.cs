namespace BookFace.Core.Domain.Interfaces
{
    public interface IApplicationUser
    {
        int Id { get; }
        string Nombre { get; } 
        string Apellido { get; } 
    }
}
