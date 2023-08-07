using System;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices.WindowsRuntime;

namespace mineSwepper
{
    public partial class Form1 : Form
    {
        //Global variables for change them later
        // diffLine -> board line count
        // diffColumn -> board column count
        // diffMine -> board mine count
        // diffB -> button pixel bounds
        // flagCount -> this is needed for count founded mine
        int diffLine = 10, diffColumn = 10, diffMine = 15;
        int diffB = 50,flagCount=0;

        //shotMode -> basicly check game mode (if this is true you lose the game its shotdown the pc) 
        bool shotMode = false;

        //openCellCount -> this is need for is the game finihs or not
        int openCellCount = 0;


        //creating my buttons object array
        buttons[,] b;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startGame();
        }

        private void startGame()
        {
            //reseting all needed variable 
            flagCount = 0;
            openCellCount = 0;

            //label 2 basicly show not found mine count(its based flag count)
            label2.Text = diffMine.ToString();

            //start the button object array
            b = new buttons[diffLine, diffColumn];

            // h equals 24 because of menuStrips height
            int w = 0, h = 24, bounds = diffB;

            //creating all buttons and give all needed property
            for (int i = 0; i < diffLine; i++)
            {
                for (int j = 0; j < diffColumn; j++)
                {
                    b[i, j] = new buttons(i, j);
                    b[i, j].SetBounds(w, h, bounds, bounds);
                    b[i, j].ForeColor = Color.Black;
                    b[i, j].FlatStyle = FlatStyle.Standard;

                    //assign my own mouseClick event
                    b[i, j].MouseClick+=new MouseEventHandler(buttonClick);
                    this.Controls.Add(b[i, j]);
                    w += bounds;
                }
                h += bounds;
                w = 0;
            }

            //creating mine and give all buttons mineCount
            createMine(diffMine);
            mineCount();
        }

        private void createMine(int mineC)
        {
            //this is creating all mine with different location
            int mineCount = mineC;
            Random rnd = new Random();
            int x, y;
            while (mineCount > 0)
            {
                x = rnd.Next(diffLine);
                y = rnd.Next(diffColumn);
                if (!b[x, y].getIsMine())// check buttons is not assign as mine
                {
                    b[x, y].setIsMine(true);
                    mineCount--;
                }
            }
        }

        private void buttonClick(object sender, MouseEventArgs e)
        {
            buttons b = sender as buttons;

            //checking clicked button is flag or flagBox is checked if its not its mean button is clickable
            if (!flagBox.Checked && !b.getIsFlag())
            {
                //checking the button is it mine or not
                if (b.getIsMine())
                {
                    playAudio();

                    //open all remaning button
                    finish();

                    //reseting openCellCount
                    openCellCount = 0;

                    //checking shot mode is true or false
                    if (shotMode)
                    {
                        MessageBox.Show("Sistem kapatılıyor...");
                        Thread.Sleep(2500);
                        var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        Process.Start(psi);
                    }
                    else
                    {   // after win or lose game, open new form and checking result
                        if (finishForm(false) == DialogResult.OK)
                        {
                            removeButton();
                            startGame();
                        }
                    }
                }
                else
                {   // if button is not mine getting button coordinats
                    int x = b.getX();
                    int y = b.getY();

                    // getting mine count around the buttons and it have any mine around this block its image
                    if (b.getCount() != 0)
                    {
                        setImage(b.getCount().ToString(), b.getX(), b.getY());
                    }
                    //if mineCount equals to 0, its meam we need open around the current clicked button
                    openCell(x, y);
                }
                // after click the button, disable the button for not click again same button
                b.Enabled = false;

                //checking the game is finish or not (its based any non-mine button)
                isFinishGame();
            }
            else if (flagBox.Checked)// if flagBox checked its mean user will put on flag clicked button
            {
                // if button is allready have a flag, user will remove the flag
                if (b.getIsFlag())
                {   //change needed variable
                    flagCount--;
                    b.setIsFlag(false);

                    //remove the flag image
                    setImage("null",b.getX(),b.getY());

                    //change the mineCount text on the menuStrip
                    label2.Text = (diffMine - flagCount).ToString();
                }
                else
                {   //if button dont have flag, user put the flag on button

                    //change needed variable
                    flagCount++;
                    b.setIsFlag(true);

                    //set the button image with flag image
                    setImage("flag", b.getX(), b.getY());

                    //change the mineCount text on the menuStrip
                    label2.Text = (diffMine-flagCount).ToString();
                }
            }
        }

        private DialogResult finishForm(bool win)//getting user win the game or not
        {

            //creating form for win or lose screen and give the necessary variable
            Form form = new Form();
            Label label = new Label();

            Button startButton = new Button();
            Button lookButton = new Button();

            startButton.Text = "New game";
            lookButton.Text = "Back to the board";

            if (win)
            {
                form.Text = "Congratulations !";
                label.Text = "Congratulations you win the game!";
            }
            else
            {
                form.Text = "Unfortunately !";
                label.Text = "unfortunately you lose the game.";
            }

            //give my buttons ok and cancel
            startButton.DialogResult= DialogResult.OK;
            lookButton.DialogResult= DialogResult.Cancel;


            label.SetBounds(15, 10, 169, 20);
            startButton.SetBounds(20, 40, 70, 40);
            lookButton.SetBounds(100, 40, 70, 40);

            form.ClientSize = new Size(200,100 );
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            // add the buttons and label in the form
            form.Controls.AddRange(new Control[] {label,startButton,lookButton});
            form.AcceptButton=startButton;
            form.CancelButton=lookButton;

            //getting clicked button for checking the user want the start new game or look the board
            DialogResult dialogResult = form.ShowDialog();

            //return the result
            return dialogResult;

        }

        private void playAudio()
        {       
            // play the explosion sound effect
            SoundPlayer audio = new SoundPlayer(Properties.Resources.explosion);
            audio.Play();
        }

        private void setImage(String img, int x,int y)// give the buttons image from the sources
        {
            switch(img)
            {
                case "1":
                    b[x, y].Image = Properties.Resources._1;
                    break;
                case "2":
                    b[x, y].Image = Properties.Resources._2;
                    break;
                case "3":
                    b[x, y].Image = Properties.Resources._3;
                    break;
                case "4":
                    b[x, y].Image = Properties.Resources._4;
                    break;
                case "5":
                    b[x, y].Image = Properties.Resources._5;
                    break;
                case "6":
                    b[x, y].Image = Properties.Resources._6;
                    break;
                case "7":
                    b[x, y].Image = Properties.Resources._7;
                    break;
                case "8":
                    b[x, y].Image = Properties.Resources._8;
                    break;
                case "mine":
                    b[x, y].Image = Properties.Resources.mine;
                    break;
                case "flag":
                    b[x, y].Image = Properties.Resources.flag;
                    break;
                case "null":
                    b[x, y].Image = null;
                    break;
                case "false":
                    b[x, y].Image = Properties.Resources.false_flag; ;
                    break;
                case "true":
                    b[x, y].Image = Properties.Resources.true_flag;
                    break;
            }
        }

        private void isFinishGame()
        {
            //this is calculate the all non-mine is open or not
            if ((diffLine) * (diffColumn) - diffMine == openCellCount)
            {//if user opend all non-mine button this mean the game is finish
                openCellCount = 0;
                finish();
                if (finishForm(true) == DialogResult.OK)//if user want start new game, then start the new game function
                {
                    removeButton();
                    startGame();
                }
            }
        }

        private void setClick(bool click)
        {
            if (click)
            {

            }
        }

        private void finish()// after win or lose the game, then open the all buttons
        {
            for (int i = 0; i < diffLine; i++)
            {
                for (int j = 0; j < diffColumn; j++)
                {
                    //if button is not mine and user put the flag on button, game put the falseFlag image 
                    //for more readable gameaBoard
                    if (!b[i, j].getIsMine()&& b[i, j].getIsFlag())
                    {
                        setImage("false", i, j);
                    }

                    //if button is mine and user put the flag on button, game put the trueFlag image 
                    //for more readable gameaBoard
                    else if (b[i, j].getIsMine()&& b[i, j].getIsFlag())
                    {
                        setImage("true", i, j);
                    }

                    //if button is mine and user dont put the flag, game put the mine image 
                    //for more readable gameaBoard
                    else if (b[i, j].getIsMine() && !b[i, j].getIsFlag())
                    {
                        setImage("mine", i, j);
                    }

                    //if button is not mine and also user didnt put the flag then check
                    //how many mine is araound the buttons and put the needed image
                    else
                    {
                        //if its have more then 0 mine around
                        if (b[i, j].getCount() != 0)
                        {
                            setImage(b[i, j].getCount().ToString(),i,j);
                        }
                    }

                    //and disable the button for finish the game
                    b[i, j].Enabled = false;
                }
            }
        }

        private void yeniOyunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //starting new game if shotMode is not active
            if (!shotMode)
            {
                removeButton();
                startGame();
            }
            else
            {
                MessageBox.Show("You cannot start a new game while Shot mode is on.");
            }
        }

        private void kolayToolStripMenuItem_Click(object sender, EventArgs e)
        {// starting easy game mode

            //remove the all button befor the starting new game 
            removeButton();

            //set the default easy mode window size and start middle of the screen
            this.ClientSize = new Size(501, 525);
            this.StartPosition = FormStartPosition.CenterScreen;

            //give easy game mode line, column, mine count and button size
            diffLine = 10;
            diffColumn = 10;
            diffMine = 15;
            diffB = 50;

            //after give all variables start the game
            startGame();
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {// starting easy game mode

            //remove the all button befor the starting new game
            removeButton();

            //set the default easy mode window size and start middle of the screen
            this.ClientSize = new Size(526, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            //give easy game mode line, column, mine count and button size
            diffLine = 15;
            diffColumn = 15;
            diffMine = 32;
            diffB = 35;

            //after give all variables start the game
            startGame();
        }

        private void zorToolStripMenuItem_Click(object sender, EventArgs e)
        {// starting easy game mode

            //remove the all button befor the starting new game
            removeButton();

            //set the default easy mode window size and start middle of the screen
            this.ClientSize = new Size(501, 525);
            this.StartPosition = FormStartPosition.CenterScreen;

            //give easy game mode line, column, mine count and button size
            diffLine = 20;
            diffColumn = 20;
            diffMine = 44;
            diffB = 25;

            //after give all variables start the game
            startGame();
        }

        private void mineCount()
        {//set all buttons mineCount
            for (int i = 0; i < diffLine; i++)
            {
                for (int j = 0; j < diffColumn; j++)
                {
                    mineCountCalc(i, j);
                }
            }
        }

        private void mineCountCalc(int x, int y)
        {//search the mine around the current giving x and y coordinats
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {

                    //check the button is located the border on the gameBoard or not
                    if (!(i < 0 || i > diffLine - 1 || j < 0 || j > diffColumn - 1))
                    {
                        if (b[i, j].getIsMine())
                        {//if button have any mine around it, increas the mineCount one by one
                            b[x, y].setCount(b[x, y].getCount() + 1);
                        }
                    }
                }
            }
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {//this method basiclly creating for the users custom gameBoard

            //save the current variable for if user didnt give value or user want the cancel process
            int tmpLine = diffLine, tmpColumn = diffColumn, tmpMine = diffMine, tmpBSize = diffB;

            //check the form return ok or cancel value
            if (InputBox(ref diffLine, ref diffColumn, ref diffMine, ref diffB) == DialogResult.OK)
            {
                //remove the all button befor the starting new game
                removeButton();

                //change the screen size full screen because user will put any value for custom gameMode
                WindowState = FormWindowState.Maximized;

                //after give all variables start the game
                startGame();
            }
            else
            {//if user cancel the custom mode then give the saved value at the start

                //remove the all button befor the starting new game
                removeButton();

                //give the saved value at the start
                diffLine = tmpLine;
                diffColumn = tmpColumn;
                diffMine = tmpMine;
                diffB = tmpBSize;

                //start new game
                startGame();
            }
        }

        private static DialogResult InputBox(ref int diffLine, ref int diffColumn, ref int diffMine, ref int diffB)
        {   //creating new form for the custom game mode input
            Form form = new Form();
            Label label = new Label();
            Label label2 = new Label();
            Label label3 = new Label();
            Label label4 = new Label();


            TextBox customLineCount = new TextBox();
            TextBox customColumnCount = new TextBox();
            TextBox customMineCount = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            TextBox custombutonSize = new TextBox();

            form.Text = "Custom Difficulty";
            label.Text = "Line count";
            label2.Text = "Column count";
            label3.Text = "Mine count";
            label4.Text = "Square size";
            buttonOk.Text = "Ok";
            buttonCancel.Text = "Cancel";


            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(10, 10, 20, 5);
            label2.SetBounds(10, 35, 20, 5);
            label3.SetBounds(10, 60, 20, 5);
            label4.SetBounds(10, 85, 20, 5);

            customLineCount.SetBounds(90, 7, 50, 30);
            customColumnCount.SetBounds(90, 32, 50, 30);
            customMineCount.SetBounds(90, 57, 50, 30);
            custombutonSize.SetBounds(90, 82, 50, 30);
            custombutonSize.Text = "50";
            buttonOk.SetBounds(10, 110, 50, 30);
            buttonCancel.SetBounds(90, 110, 50, 30);

            label.AutoSize = true;
            label2.AutoSize = true;
            label3.AutoSize = true;
            label4.AutoSize = true;

            form.ClientSize = new Size(150, 145);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Controls.AddRange(new Control[] { label, label2, label4, label3, customLineCount, customColumnCount, customMineCount, custombutonSize, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            //check the given value is int or not
            //if value is not int or empty return the cancel value
            int deger;
            if (int.TryParse(customLineCount.Text, out deger) && int.TryParse(customColumnCount.Text, out deger) && int.TryParse(customMineCount.Text, out deger) && int.TryParse(custombutonSize.Text, out deger))
            {
                diffLine = int.Parse(customLineCount.Text);
                diffColumn = int.Parse(customColumnCount.Text);
                diffMine = int.Parse(customMineCount.Text);
                diffB = int.Parse(custombutonSize.Text);
            }
            else
            {
                return DialogResult.Cancel;
            }
            return dialogResult;
        }

        private void shotModeToolStripMenuItem_Click(object sender, EventArgs e)
        {//Turn on the shotMode 
            var tmp = sender as ToolStripMenuItem;
            if (openCellCount == 0)//if game allready start, user cant turn on or turn off the shotMode
            {
                if (!shotMode)
                {
                    shotMode = true;
                    tmp.Checked = true;
                }
                else
                {
                    shotMode = false;
                    tmp.Checked = false;
                }
            }
        }

        private void openCell(int x, int y)
        {//open cell if cliced button dont have  any mine arround it
         //this method working recursive

            //check the current button location is it border or not
            if (x >= 0 && x < diffLine && y >= 0 && y < diffColumn)
            {

                //check the button is enable and its not a mine
                if (!b[x, y].getIsMine() && b[x, y].Enabled == true)
                {
                    //if button have any mine around it, then put the needed image on the button
                    if (b[x, y].getCount() != 0)
                    {
                        setImage(b[x, y].getCount().ToString(), x,y);
                    }

                    //set the button disable for the user doesnt need the click the buton
                    b[x, y].Enabled = false;

                    //increas the open cell count for the checking later is game finish or not
                    openCellCount += 1;

                    //if button dont have any mine around, then call the method again
                    if (b[x, y].getCount() == 0)
                    {
                        //check the all button at the around clicked button
                        openCell(x - 1, y);
                        openCell(x, y - 1);
                        openCell(x + 1, y);
                        openCell(x, y + 1);
                        openCell(x - 1, y - 1);
                        openCell(x - 1, y + 1);
                        openCell(x + 1, y - 1);
                        openCell(x + 1, y + 1);
                    }

                }
            }
        }

        private void removeButton()
        {// basiclly remove all button from the game form
            foreach (var rmv in b)
            {
                Controls.Remove(rmv);
            }
        }
    }
}
