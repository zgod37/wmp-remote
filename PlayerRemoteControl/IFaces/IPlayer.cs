namespace PlayerRemoteControl {

    /// <summary>
    /// interface to be accessed by main form for each child window
    /// </summary>
    public interface IPlayer {

        void play();
        void pause();
        void stop();
        void close();

    }
}
