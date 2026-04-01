using System.ComponentModel.DataAnnotations;
using SQLite;
namespace VirtueTracker.Models;

public class Virtue
{
    [PrimaryKey]
    public string Id {get; set;} = default!;
    public string Name {get; set;} = string.Empty;

}

