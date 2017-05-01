using PlayerRemoteControl.IFaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PlayerRemoteControl {
    public partial class Form1 : Form, IRemote {

        /// <summary>
        /// list of current open players
        /// each child will register as an observer
        /// </summary>
        public List<IPlayer> Players { get; set; }

        #region Constructor

        /// <summary>
        /// default constructor for main form
        /// </summary>
        public Form1() {
            InitializeComponent();
            this.Players = new List<IPlayer>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// add player to observer list
        /// </summary>
        /// <param name="observer"></param>
        public void registerObserver(IPlayer observer) {
            this.Players.Add(observer);
        }

        /// <summary>
        /// remove player from observer list
        /// </summary>
        /// <param name="observer"></param>
        public void unregisterObserver(IPlayer observer) {
            this.Players.Remove(observer);
        }

        #endregion

        #region Event Handlers
        //TODO: seperate these into a controller for MVC pattern


        /// <summary>
        /// click handler to load new player window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadButton_Click(object sender, EventArgs e) {
            loadNewPlayer();
        }

        /// <summary>
        /// click handler to pause all child players
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseButton_Click(object sender, EventArgs e) {
            pauseAll();
        }

        /// <summary>
        /// click handler to play/resume all child players
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e) {
            playAll();
        }

        /// <summary>
        /// click handler to close all players
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e) {
            closeAll();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// open file dialog and load file in a new player window
        /// </summary>
        private void loadNewPlayer() {

            //open folder dialog box to get target file
            String[] targetFiles = null;
            using (var dialog = new OpenFileDialog()) {
                dialog.Multiselect = true;
                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {

                    targetFiles = dialog.FileNames;
                }
            }

            //if target file selected, open new form and add to children
            if (targetFiles != null) {
                PlayerForm player = new PlayerForm(this, targetFiles, Players.Count);
                player.Show();
            }
        }

        /// <summary>
        /// pause all the currently loaded videos
        /// </summary>
        private void pauseAll() {
            foreach (IPlayer player in Players) {
                player.pause();
            }
        }

        /// <summary>
        /// play all the currently loaded videos
        /// </summary>
        private void playAll() {
            foreach (IPlayer player in Players) {
                player.play();
            }
        }

        /// <summary>
        /// stop all the currently loaded videos
        /// </summary>
        private void stopAll() {
            foreach (IPlayer player in Players) {
                player.close();
            }
        }

        /// <summary>
        /// close all videos
        /// </summary>
        private void closeAll() {

            //because the player's unregister, make copy of list to avoid invalid operation
            List<IPlayer> copyOfPlayers = new List<IPlayer>(Players);
            foreach (IPlayer player in copyOfPlayers) {
                player.close();
            }
            copyOfPlayers.Clear();
        }

        #endregion

    }
}
