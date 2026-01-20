
namespace _3C.Actors.States
{
    public class ActorState: State
    {
        private Actor character;
        public Actor Actor { get {
             if(character == null)
                    character = Controller as Actor;
                return character;
            } }
    }
}
