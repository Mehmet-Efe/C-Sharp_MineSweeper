using System.Windows.Forms;

namespace mineSwepper
{
    internal class buttons : Button//inheritance from button
    {
        //set the needed variables
        int x;
        int y;
        bool isFlag;
        int count = 0;
        bool isMine = false;

        //creating needed set-get methods

        public buttons(int x, int y)
        {
            this.y = y;
            this.x = x;
            isFlag = false;
        }

        public void setCount(int count) { this.count = count; }
        public int getCount() { return count; }

        public void setIsMine(bool isMine) { this.isMine = isMine; }

        public bool getIsMine() { return isMine; }

        public int getX() { return x; }
        public int getY() { return y; }

        public void setIsFlag(bool isClick) { this.isFlag = isClick; }

        public bool getIsFlag() { return isFlag; }

    }
}
