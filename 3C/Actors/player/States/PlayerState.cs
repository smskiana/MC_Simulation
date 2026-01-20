
namespace _3C.Actors.player.States
{
    public abstract class PlayerState : State
    {
        private Player player;
        protected Player Player { get {
                if(player==null) player = Controller as Player;
                return player;
            } }
       
    }
}
