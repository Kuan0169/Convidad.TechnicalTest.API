namespace Convidad.TechnicalTest.Models.DTOs
{
    public record ReindeerDto(
       Guid Id,
        string Name,
        string PlateNumber,
        double Weight,
        int Packets
    );
}