namespace Convidad.TechnicalTest.Models.DTOs
{ 
    public record ChildDto(
    Guid Id,
    string Name,
    string CountryCode,
    bool IsNice);

    public record CreateChildDto(
    string Name,
    string CountryCode,
    bool IsNice = true);
}



