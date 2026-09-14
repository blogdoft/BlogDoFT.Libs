using System;

namespace FlagrExample.Flags;

public class OnOffFlag
{
    private readonly string OnState = "on";

    public OnOffFlag(string? flagState)
    {
        IsOn = false;
        if (flagState?.Equals(OnState, StringComparison.OrdinalIgnoreCase) == true)
        {
            IsOn = true;
        }
    }

    public bool IsOn { get; }

    public bool IsOff => !IsOn;
}
