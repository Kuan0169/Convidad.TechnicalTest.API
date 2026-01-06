using System;
using System.Collections.Generic;
using System.Text;

namespace Convidad.TechnicalTest.Data.DTOs
{
    public record ChildDto
    (
        Guid Id,
        string Name,
        string CountryCode,
        bool IsNice 
    );
}
