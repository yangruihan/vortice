﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Alimer.Graphics;

namespace Alimer.Engine;

/// <summary>
/// Defines an <see cref="Application"/> that handles Game logic.
/// </summary>
public class GameApplication : Application
{
    public GameApplication(GraphicsBackendType preferredGraphicsBackend = GraphicsBackendType.Count)
        : base(default, preferredGraphicsBackend)
    {
        SceneSystem = new SceneSystem(Services);
        Services.AddService(SceneSystem);
        GameSystems.Add(SceneSystem);
    }

    public IList<IGameSystem> GameSystems { get; } = new List<IGameSystem>();

    public SceneSystem SceneSystem { get; }

    protected override void Update(AppTime time)
    {
        base.Update(time);

        foreach (IGameSystem system in GameSystems)
        {
            system.Update(time);
        }
    }

    protected override void BeginDraw()
    {
        base.BeginDraw();

        foreach (IGameSystem system in GameSystems)
        {
            system.BeginDraw();
        }
    }

    protected override void Draw(RenderContext renderContext, Texture outputTexture, AppTime time)
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.Draw(renderContext, outputTexture, time);
        }

        base.Draw(renderContext, outputTexture, time);
    }

    protected override void EndDraw()
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.EndDraw();
        }

        base.EndDraw();
    }
}
