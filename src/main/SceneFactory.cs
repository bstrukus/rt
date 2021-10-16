/*
 * Copyright Ben Strukus
 */

using JsonObjects;
using System;
using System.Diagnostics;
using rt.Math;
using System.Collections.Generic;

namespace rt
{
    internal class SceneFactory
    {
        public SceneFactory()
        {
            //Singleton stuff
        }

        public Scene CreateScene(JsonObjects.SceneData sceneData)
        {
            return new Scene();
        }
    }
}