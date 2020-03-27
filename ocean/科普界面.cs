using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ocean
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        //提高控件绘制速度
        protected override CreateParams CreateParams
        {
            get
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x02000000;
                    return cp;
                }
                else
                {
                    return base.CreateParams;
                }
            }
        }

      

        /*1节点被选中 ,TreeView有焦点*/
        private SolidBrush brush1 = new SolidBrush(Color.FromArgb(209, 232, 255));//填充颜色
        private Pen pen1 = new Pen(Color.FromArgb(102, 167, 232), 1);//边框颜色

        /*2节点被选中 ,TreeView没有焦点*/
        private SolidBrush brush2 = new SolidBrush(Color.FromArgb(247, 247, 247));
        private Pen pen2 = new Pen(Color.FromArgb(222, 222, 222), 1);

        /*3 MouseMove的时候 画光标所在的节点的背景*/
        private SolidBrush brush3 = new SolidBrush(Color.FromArgb(229, 243, 251));
        private Pen pen3 = new Pen(Color.FromArgb(112, 192, 231), 1);

   
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
          Rectangle  nodeRect = new Rectangle(1,
                                              e.Bounds.Top,
                                              e.Bounds.Width - 3,
                                              e.Bounds.Height - 1);
            //1     选中的节点背景=========================================
            if (e.Node.IsSelected)
            {
                //TreeView有焦点的时候 画选中的节点
                if (treeView1.Focused)
                {
                    e.Graphics.FillRectangle(brush1, nodeRect);
                    e.Graphics.DrawRectangle(pen1, nodeRect);

                    /*测试 画聚焦的边框*/
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, Color.Black, SystemColors.Highlight);
                }
                /*TreeView失去焦点的时候 */
                else 
                {
                    e.Graphics.FillRectangle(brush2, nodeRect);
                    e.Graphics.DrawRectangle(pen2, nodeRect);
                }
            }
            else if ((e.State & TreeNodeStates.Hot) != 0 && e.Node.Text != "")//|| currentMouseMoveNode == e.Node)
            {
                e.Graphics.FillRectangle(brush3, nodeRect);
                e.Graphics.DrawRectangle(pen3, nodeRect);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }



            //2     +-号绘制=========================================
           
            Rectangle plusRect = new Rectangle(e.Node.Bounds.Left - 32,nodeRect.Top + 7, 9, 9); // +-号的大小 是9 * 9
                                                               
            if (e.Node.IsExpanded)
                e.Graphics.DrawImage(arrowImageList.Images[1], plusRect);
            else if (e.Node.IsExpanded == false && e.Node.Nodes.Count > 0)
                e.Graphics.DrawImage(arrowImageList.Images[0], plusRect);


            /*测试用 画出+-号出现的矩形*/
            if (e.Node.Nodes.Count > 0)
                e.Graphics.DrawRectangle(new Pen(Color.Red), plusRect);



            //3     文本绘制=========================================
            Rectangle nodeTextRect = new Rectangle(
                                                    e.Node.Bounds.Left,
                                                    e.Node.Bounds.Top +2 ,
                                                    e.Node.Bounds.Width+2,
                                                    e.Node.Bounds.Height
                                                    ); 
            nodeTextRect.Width += 4;
            nodeTextRect.Height -= 4;

            e.Graphics.DrawString(e.Node.Text,
                                  e.Node.TreeView.Font,
                                  new SolidBrush(Color.Black),
                                  nodeTextRect);


            //画子节点个数 (111)
            if (e.Node.GetNodeCount(true) > 0)
            {
                e.Graphics.DrawString(  string.Format("({0})", e.Node.GetNodeCount(true)),
                                        new Font("Arial", 8),
                                        Brushes.Gray,
                                        nodeTextRect.Right - 4, 
                                        nodeTextRect.Top + 2);  
            }

            /*测试用，画文字出现的矩形*/
            if (e.Node.Text != "")
                e.Graphics.DrawRectangle(new Pen(Color.Blue), nodeTextRect);


            //4     画IImageList 中的图标===================================================================
            int currt_X = e.Node.Bounds.X;
            if (treeView1.ImageList != null && treeView1.ImageList.Images.Count>0)
            {
                //图标大小16*16
                Rectangle imagebox = new Rectangle(
                    e.Node.Bounds.X - 3 - 16,
                    e.Node.Bounds.Y + 3,
                    16,//IMAGELIST IMAGE WIDTH
                    16);//HEIGHT


                int index = e.Node.ImageIndex;
                string imagekey = e.Node.ImageKey;
                if (imagekey != "" && treeView1.ImageList.Images.ContainsKey(imagekey))
                    e.Graphics.DrawImage(treeView1.ImageList.Images[imagekey], imagebox);
                else
                {
                    if (e.Node.ImageIndex < 0)
                        index = 0;
                    else if (index > treeView1.ImageList.Images.Count - 1)
                        index = 0;
                    e.Graphics.DrawImage(treeView1.ImageList.Images[index], imagebox);
                }
                currt_X -= 19;

                /*测试 画IMAGELIST的矩形*/
                if (e.Node.ImageIndex > 0)
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1), imagebox);

              
            }


            /*测试 画所有的边框*/
            nodeRect = new Rectangle(1,
                         e.Bounds.Top+1,
                         e.Bounds.Width - 3,
                         e.Bounds.Height - 2);
            e.Graphics.DrawRectangle(new Pen(Color.Gray),nodeRect);
        }


        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node != null)
            {
                //禁止选中空白项
                if (e.Node.Text == "")
                {
                    //响应上下键
                    if (ArrowKeyUp)
                    {
                        if (e.Node.PrevNode != null && e.Node.PrevNode.Text != "")
                            treeView1.SelectedNode = e.Node.PrevNode;
                    }

                    if (ArrowKeyDown)
                    {
                        if (e.Node.NextNode != null && e.Node.NextNode.Text != "")
                            treeView1.SelectedNode = e.Node.NextNode;
                    }

                    e.Cancel = true;
                }
            }

        }
        
        bool ArrowKeyUp = false;
        bool ArrowKeyDown = false;

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            ArrowKeyUp = (e.KeyCode == Keys.Up);
            if (e.KeyCode==Keys.Down)
            {
                Text ="Down";
            }

            ArrowKeyDown = (e.KeyCode == Keys.Down);
            if (e.KeyCode==Keys.Up)
            {
                Text ="UP";
            }

        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Text == "名字由来")
            {
                this.tabControl1.SelectedTab = this.tabPage1;
                tabControl1.Visible = true;


            }
            if (e.Node.Text == "定义标准")
            {
                this.tabControl1.SelectedTab = this.tabPage2;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "周期")
            {
                this.tabControl1.SelectedTab = this.tabPage3;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "厄尔尼诺的成因")
            {
                this.tabControl1.SelectedTab = this.tabPage4;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "厄尔尼诺的表现")
            {
                this.tabControl1.SelectedTab = this.tabPage5;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "对中国的影响")
            {
                this.tabControl1.SelectedTab = this.tabPage6;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "对全球的影响")
            {
                this.tabControl1.SelectedTab = this.tabPage7;
                tabControl1.Visible = true;
            }
            if (e.Node.Text == "厄尔尼诺的实例")
            {
                this.tabControl1.SelectedTab = this.tabPage8;
                tabControl1.Visible = true;
            }
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
        }

        private void rtfRichTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.panel1.Focus();
        }

        private void rtfRichTextBox2_MouseMove(object sender, MouseEventArgs e)
        {
            this.panel2.Focus();
        }

        private void rtfRichTextBox9_MouseMove(object sender, MouseEventArgs e)
        {
            this.panel2.Focus();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void rtfRichTextBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
         //   PanelScrollHelper.InitializePanelScroll(panel1);
         //   PanelScrollHelper.InitializePanelScroll(panel2);
        }

        private void rtfRichTextBox1_Click(object sender, EventArgs e)
        {
            this.panel1.Focus();
        }



    }


}
