using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class ActionResult
{
    public bool Success { get; }
    public string MessageKey { get; }
    public object[] Parameters { get; }

    public ActionResult(
        bool success,
        string messageKey,
        params object[] parameters)
    {
        Success = success;
        MessageKey = messageKey;
        Parameters = parameters;
    }
}
