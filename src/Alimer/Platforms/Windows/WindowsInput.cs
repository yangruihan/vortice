// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Input;

internal class WindowsInput : InputManager
{
    private readonly WindowsPlatform _platform;

    public WindowsInput(WindowsPlatform platform)
    {
        _platform = platform;
    }
}
