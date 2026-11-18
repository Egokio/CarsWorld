using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class CarGroupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Guid> CarIds { get; set; } = new();
}