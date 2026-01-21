namespace Convidad.TechnicalTest.Models.DTOs;
public record RouteDto(
    Guid Id,
    string Name,
    string Region,
    int CapacityPerNight
);
