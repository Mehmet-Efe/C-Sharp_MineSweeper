namespace calc
{
    public partial class form1 : Form
    {
        public form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            System.Drawing.Graphics graphicsObj;

            graphicsObj = this.CreateGraphics();

            Pen myPen = new Pen(System.Drawing.Color.Red, 5);

            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

            myPen.Color = System.Drawing.Color.RoyalBlue;

            myPen.Width = 3;

            graphicsObj.DrawLine(myPen, 20, 20, 200, 210);

        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}