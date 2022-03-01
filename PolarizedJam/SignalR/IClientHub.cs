namespace PolarizedJam.SignalR;

public interface IClientHub
{
    Task Update(Guid shapeId, string shapeType, int shapeX, int shapeY);
    Task Move(string username, int x, int y);
}
