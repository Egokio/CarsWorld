using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public interface IWorldStorage
{
    Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default);

    Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default);
}