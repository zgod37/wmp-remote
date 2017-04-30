using PlayerRemoteControl.IFaces;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace PlayerRemoteControl {
    public partial class PlayerForm : Form, IPlayer {

        /// <summary>
        /// keep instance of the remote so we can register as observer
        /// </summary>
        IRemote remote;

        /// <summary>
        /// flag to determine whether this video is looping
        /// </summary>
        public bool IsLooping { get; set; }

        #region Constructor

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="mainForm">Reference to the main remote form</param>
        /// <param name="url">the file path of the video to be loaded</param>
        public PlayerForm(Form1 mainForm, String url) {
            InitializeComponent();

            this.Text = url;

            //register observer
            this.remote = mainForm;
            mainForm.registerObserver(this);

            //listen for keypresses
            this.KeyPreview = true;

            //set player properties
            Player.Dock = DockStyle.Fill;
            Player.stretchToFit = true;
            Player.URL = url;

            //initalize timestamps
            beginTextBox.Text = "00:00:00";
            endTextBox.Text = "00:00:00";
            jumpTextBox.Text = "00:00:00";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// pause the video
        /// </summary>
        public void pause() {
            if (Player.playState != WMPLib.WMPPlayState.wmppsPaused)
                Player.Ctlcontrols.pause();
        }

        /// <summary>
        /// play/resume the video
        /// </summary>
        public void play() {
            if (Player.playState != WMPLib.WMPPlayState.wmppsPlaying)
                Player.Ctlcontrols.play();
        }

        /// <summary>
        /// stops the video
        /// </summary>
        public void stop() {
            if (Player.playState != WMPLib.WMPPlayState.wmppsStopped)
                Player.Ctlcontrols.stop();
        }

        /// <summary>
        /// un-register observer with mainform on closing
        /// </summary>
        public void close() {
            stop();
            remote.unregisterObserver(this);
            this.Dispose();
        }

        /// <summary>
        /// disable loop button - sends delegate to UI thread
        /// </summary>
        public void disableLoopButton() {
            loopButton.Invoke(new Action(() => {
                loopButton.Enabled = false;
                loopButton.Text = "looping..";
            }));
        }

        /// <summary>
        /// enable loop button - sends delegate to UI thread
        /// </summary>
        public void enableLoopButton() {
            loopButton.Invoke(new Action(() => {
                loopButton.Enabled = true;
                loopButton.Text = "loop";
            }));
        }

        /// <summary>
        /// disable stop button - sends delegate to UI thread
        /// </summary>
        public void disableStopLoopButton() {
            stopButton.Invoke(new Action(() => {
                stopButton.Enabled = false;
                stopButton.Text = "stopped";
            }));
        }

        /// <summary>
        /// enable stop button - sends delegate to UI thread
        /// </summary>
        public void enableStopLoopButton() {
            stopButton.Invoke(new Action(() => {
                stopButton.Enabled = true;
                stopButton.Text = "stop";
            }));
        }

        #endregion

        #region Event Handlers

        #region Form Event Handlers

        /// <summary>
        /// event handler when form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e) {
            close();
        }

        #endregion

        #region WMP Handlers

        private void Player_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e) {
            //if media error is called, check and notify user if file is missing or corrupt
            try {
                IWMPMedia2 errSource = e.pMediaObject as IWMPMedia2;
                IWMPErrorItem errorItem = errSource.Error;
                MessageBox.Show("Error " + errorItem.errorCode.ToString("X") + " in " + errSource.sourceURL);
            } catch (InvalidCastException ex) {
                MessageBox.Show("Error received:" + ex.Message);
            }
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// event handler to stop the loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e) {
            this.IsLooping = false;
            stopButton.Text = "stopping..";
        }

        /// <summary>
        /// event handler to start the loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loopButton_Click(object sender, EventArgs e) {
            prepareForLoop();
        }

        /// <summary>
        /// event handler to go full screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fullScreenButton_Click(object sender, EventArgs e) {
            goFullScreen();
        }

        #endregion

        #region Label Handlers

        /// <summary>
        /// event handler to set endTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endLabel_Click(object sender, EventArgs e) {
            setEndTextBoxToCurrent();
        }

        /// <summary>
        /// event handler to set beginTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void beginLabel_Click(object sender, EventArgs e) {
            setBeginTextBoxToCurrent();
        }

        /// <summary>
        /// event handler to set jumpTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jumpLabel_Click(object sender, EventArgs e) {
            setJumpTextBoxToCurrent();
        }

        #endregion

        #region Keyboard Shortcut Handlers

        /// <summary>
        /// event handler to map keyboard shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerForm_KeyPress(object sender, KeyPressEventArgs e) {

            switch (e.KeyChar) {

                //jump to position on textbox
                case 'j':
                    jumpTo(PlayerUtils.ConvertTimestampToSeconds(jumpTextBox.Text));
                    break;

                //set begin text box
                case 'b':
                    setBeginTextBoxToCurrent();
                    break;

                //set end text box
                case 'e':
                    setEndTextBoxToCurrent();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// event handler to catch "ctrl plus" and "alt plus" key commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerForm_KeyDown(object sender, KeyEventArgs e) {

            if (e.Control) {
                switch (e.KeyCode) {

                    //Ctrl + J to set jump text box to current timestamp
                    case Keys.J:
                        setJumpTextBoxToCurrent();
                        break;

                    //Ctrl + C to dispose
                    case Keys.C:
                        close();
                        break;

                    default:
                        break;
                }
            } else if (e.Alt) {

                switch (e.KeyCode) {

                    //Alt + Up to hide control panel
                    case Keys.Up:
                        hideControls();
                        break;

                    //Alt + Down to show control panel
                    case Keys.Down:
                        showControls();
                        break;

                    default:
                        break;

                }

            }

        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// intialize the necessary components to start the looping task
        /// </summary>
        private void prepareForLoop() {

            //update UI
            disableLoopButton();
            enableStopLoopButton();

            //convert timestamp strings to seconds
            int start = PlayerUtils.ConvertTimestampToSeconds(beginTextBox.Text);
            int end = PlayerUtils.ConvertTimestampToSeconds(endTextBox.Text);

            //bail if timestamps are bogus
            if (start == -1 || end == -1 || start > end) {
                return;
            }

            //start loop in async task
            loop(start, end);
        }

        /// <summary>
        /// use player's ctlcontrols to loop a video - see looping conditions in definition
        /// </summary>
        /// <param name="start">the starting timestamp of the loop in seconds</param>
        /// <param name="end">the ending timestamp of the loop in seconds</param>
        private void loop(int start, int end) {

            //calculate duration in ms, adding 500ms of time
            int delayMillis = ((end * 1000)+500) - (start * 1000);

            //set to start time
            Player.Ctlcontrols.currentPosition = start;

            //**** START LOOP HERE ****
            //uses pre- and post-condition checks to see if user canceled
            this.IsLooping = true;
            Task.Run(async () => {
                int loopCount = 4;
                int count = 0;

                while (this.IsLooping == true) {

                    //divide up wait time into n-Segments for more frequent checks
                    await Task.Delay(delayMillis/(loopCount+1));

                    //break out if user has canceled the loop
                    if ((this.IsLooping == false ||
                            Player.playState != WMPPlayState.wmppsPlaying)) {
                        this.IsLooping = false;
                        break;
                    }

                    //restart when duration has passed
                    if (count == loopCount) {
                        Player.Ctlcontrols.currentPosition = start;
                        count = 0;
                    } else {
                        count++;
                    }
                }

                //update UI after looping
                disableStopLoopButton();
                enableLoopButton();
            });

        }
        
        /// <summary>
        /// hides the bottom row of controls
        /// </summary>
        private void hideControls() {
            tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Absolute;
            tableLayoutPanel1.RowStyles[1].Height = 0;
        }

        /// <summary>
        /// shows the bottom row of controls
        /// </summary>
        private void showControls() {
            tableLayoutPanel1.RowStyles[1].SizeType = SizeType.AutoSize;
        }

        /// <summary>
        /// jump to specified position of video
        /// </summary>
        /// <param name="position">the timestamp in seconds to jump to</param>
        private void jumpTo(int position) {
            Player.Ctlcontrols.currentPosition = position;
        }

        /// <summary>
        /// set player to full screen if the video is playing
        /// **NOTE** this function is buggy if more than one playerform exists!
        /// </summary>
        private void goFullScreen() {
            if (Player.playState == WMPLib.WMPPlayState.wmppsPlaying) {
                Player.fullScreen = true;
            }
        }

        /// <summary>
        /// set begin text box with current timestamp of video
        /// </summary>
        private void setBeginTextBoxToCurrent() {
            beginTextBox.Text = PlayerUtils.NormalizeTimestamp(Player.Ctlcontrols.currentPositionString);
            beginTextBox.Enabled = true;
        }

        /// <summary>
        /// set end text box with current timestamp of video
        /// </summary>
        private void setEndTextBoxToCurrent() {
            endTextBox.Text = PlayerUtils.NormalizeTimestamp(Player.Ctlcontrols.currentPositionString);
            endTextBox.Enabled = true;
        }

        /// <summary>
        /// set jump text box with current timestamp of video
        /// </summary>
        private void setJumpTextBoxToCurrent() {
            jumpTextBox.Text = PlayerUtils.NormalizeTimestamp(Player.Ctlcontrols.currentPositionString);
            jumpTextBox.Enabled = true;
        }

        #endregion

    }
}
