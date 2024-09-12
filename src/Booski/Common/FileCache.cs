using System.ComponentModel.DataAnnotations;
using Booski.Enums;

namespace Booski.Common;

public class FileCache
{
    [Key]
    public string Uri { get; set; }

    public bool Available { get; set; }
    public DateTime? DownloadedAt { get; set; }
    public string Filename { get; set; }
}