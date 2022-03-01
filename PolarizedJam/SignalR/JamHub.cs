using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PolarizedJam.Database;

namespace PolarizedJam.SignalR;

public class JamHub : Hub<IClientHub>
{
    private readonly DatabaseContext _db;
    public const string HubUrl = "/jam/{id:guid}/live";

    private Guid? JamId
    {
        get
        {
            Context.Items.TryGetValue("JamId", out var obj);
            if (obj is string value)
            {
                return !Guid.TryParse(value, out var id) ? null : id;
            }

            return null;
        }
        set => Context.Items["JamId"] = value.ToString();
    }

    public JamHub(DatabaseContext db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var request = Context.Features.Get<IHttpContextFeature>()!.HttpContext!.Request;
        if (request.RouteValues.TryGetValue("id", out var rawAccountId))
        {
            var jamIdString = rawAccountId as string;
            if (!string.IsNullOrWhiteSpace(jamIdString) && Guid.TryParse(jamIdString, out var jamId))
            {
                JamId = jamId;
                await Groups.AddToGroupAsync(Context.ConnectionId, jamIdString);
                await base.OnConnectedAsync();
                return;
            }
        }

        if (!JamId.HasValue || !(await _db.Jams.AnyAsync(x => x.Id == JamId)))
        {
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (JamId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, JamId.Value.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Update(Guid shapeId, string shapeType, int shapeX, int shapeY)
    {
        var jam = _db.Jams.First(x => x.Id == JamId);
        var shape = jam.Data.Shapes.FirstOrDefault(x => x.Id == shapeId);

        if (shape == null)
        {
            shape = new Shape()
            {
                Id = shapeId,
                Type = shapeType,
                X = shapeX,
                Y = shapeY,
            };
            jam.Data.Shapes.Add(shape);
        }
        else
        {
            shape.X = shapeX;
            shape.Y = shapeY;
        }

        _db.Update(jam);
        await _db.SaveChangesAsync();
        await Clients.OthersInGroup(JamId.ToString()!).Update(shapeId, shapeType, shapeX, shapeY);
    }

    public async Task Move(string username, int x, int y)
    {
        await Clients.OthersInGroup(JamId.ToString()!).Move(username, x, y);
    }
}
