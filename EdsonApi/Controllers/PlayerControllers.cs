using EdsonApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PlayerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private const string FileName = "players.json";
    private static readonly bool UseFilePersistence = true;
    private static readonly List<Player> Players;

    static PlayerController()
    {
        Players = new List<Player>
        {
            new Player { Id = 1, Vida = 100, QuantidadeItens = 0, PosicaoX = 0, PosicaoY = 0, PosicaoZ = 0 },
            new Player { Id = 2, Vida = 80, QuantidadeItens = 2, PosicaoX = 1.5f, PosicaoY = 0, PosicaoZ = -3f }
        };

        if (UseFilePersistence)
        {
            try
            {
                if (System.IO.File.Exists(FileName))
                {
                    var text = System.IO.File.ReadAllText(FileName);
                    var loaded = JsonSerializer.Deserialize<List<Player>>(text);
                    if (loaded != null)
                    {
                        Players.Clear();
                        Players.AddRange(loaded);
                    }
                }
                else
                {
                    SaveToFile();
                }
            }
            catch
            {
            }
        }
    }

    private static void SaveToFile()
    {
        try
        {
            var text = JsonSerializer.Serialize(Players);
            System.IO.File.WriteAllText(FileName, text);
        }
        catch
        {
        }
    }

    [HttpGet("GetPlayers")]
    public ActionResult<IEnumerable<Player>> GetPlayers() => Ok(Players);

    [HttpGet("GetPlayer/{id:int}")]
    public ActionResult<Player> GetPlayer(int id)
    {
        var p = Players.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost("AddPlayer")]
    public ActionResult<Player> AddPlayer([FromBody] Player player)
    {
        if (player == null) return BadRequest();
        if (Players.Any(x => x.Id == player.Id))
        {
            var newId = Players.Any() ? Players.Max(x => x.Id) + 1 : 1;
            player.Id = newId;
        }
        Players.Add(player);
        if (UseFilePersistence) SaveToFile();
        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }

    [HttpDelete("DeletePlayer/{id:int}")]
    public IActionResult DeletePlayer(int id)
    {
        var p = Players.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        Players.Remove(p);
        if (UseFilePersistence) SaveToFile();
        return NoContent();
    }

    [HttpPut("UpdatePlayer/{id:int}")]
    public IActionResult UpdatePlayer(int id, [FromBody] Player player)
    {
        if (player == null) return BadRequest();
        var existing = Players.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.Vida = player.Vida;
        existing.QuantidadeItens = player.QuantidadeItens;
        existing.PosicaoX = player.PosicaoX;
        existing.PosicaoY = player.PosicaoY;
        existing.PosicaoZ = player.PosicaoZ;
        if (UseFilePersistence) SaveToFile();
        return NoContent();
    }
}
