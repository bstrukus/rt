/*
 * Copyright Ben Strukus
 */

namespace rt.Present
{
    internal class SceneFactory
    {
        public SceneFactory()
        {
            //Singleton stuff
        }

        public Scene CreateScene(Data.SceneData sceneData)
        {
            return new Scene();
        }
    }
}