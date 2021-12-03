using System.ComponentModel.DataAnnotations;

namespace Giphy.Application.Filters;

public class GifFilter
{
    [Range(1, int.MaxValue, ErrorMessage = "Page should be greater or equal 1")]
    public int? Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Min limit 1, max limit 50")]
    public int? Limit { get; set; } = 20;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Query should not be empty string")]
    public string Name { get; set; } = null!;

    public string Query => $"q={Name}&limit={Limit}&offset={(Page - 1) * Limit}";
}