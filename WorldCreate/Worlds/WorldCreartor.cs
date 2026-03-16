using UnityEngine;
namespace WorldCreate.Worlds

{
    public class WorldCreartor : SingletonMono<WorldCreartor>
    {
        private ChucksContainer chucksContainer;
        protected override void SingletonAwake()
        {
            chucksContainer = new ChucksContainer(1);
        }
      
    }
}
