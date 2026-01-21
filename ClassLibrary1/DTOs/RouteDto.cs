namespace Convidad.TechnicalTest.Models.DTOs;

public record RouteDto(
Guid Id,
string Name,
string Region,
int CapacityPerNight);

public record CreateRouteDto(
string Name,
string Region,
int CapacityPerNight = 50);
