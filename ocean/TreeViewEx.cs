using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{

    public class TreeViewEx : TreeView
    {
        #region 双缓存重绘

        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        //private const int WM_MOUSEWHEEL = 0x020A;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETBKCOLOR = TV_FIRST + 29;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        private void UpdateExtendedStyles()
        {
            int Style = 0;

            if (DoubleBuffered)
                Style |= TVS_EX_DOUBLEBUFFER;

            if (Style != 0)
                API.SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)Style);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateExtendedStyles();
            if (!API.IsWinXP)
                API.SendMessage(Handle, TVM_SETBKCOLOR, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(BackColor));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                Message m = new Message();
                m.HWnd = Handle;
                m.Msg = API.WM_PRINTCLIENT;
                m.WParam = e.Graphics.GetHdc();
                m.LParam = (IntPtr)API.PRF_CLIENT;
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }
        #endregion

        public TreeViewEx()
        {
            treeView1 = this;
            treeView1.HotTracking = true;
            treeView1.HideSelection = false;

            treeView1.SelectedImageIndex = treeView1.ImageIndex;

            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 9.7F);
            this.treeView1.FullRowSelect = true;
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawAll;

            //treeView1.Nodes[1].Expand();
            //treeView1.Nodes[5].Expand();
            //treeView1.SelectedNode = treeView1.Nodes[1];


            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
        }



        #region DrawNode

        public ImageList arrowImageList
        {
            get
            {
                return arrowImageList1;
            }
            set
            {
                arrowImageList1 = value;
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

            #region 1     选中的节点背景=========================================
            Rectangle nodeRect = new Rectangle(1,
                                                e.Bounds.Top,
                                                e.Bounds.Width - 3,
                                                e.Bounds.Height - 1);

            if (e.Node.IsSelected)
            {
                //TreeView有焦点的时候 画选中的节点
                if (treeView1.Focused)
                {
                    e.Graphics.FillRectangle(brush1, nodeRect);
                    e.Graphics.DrawRectangle(pen1, nodeRect);

                    /*测试 画聚焦的边框*/
                    //ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, Color.Black, SystemColors.Highlight);
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
            #endregion

            #region 2     +-号绘制=========================================
            Rectangle plusRect = new Rectangle(e.Node.Bounds.Left - 32, nodeRect.Top + 7, 9, 9); // +-号的大小 是9 * 9

            if (e.Node.IsExpanded)
                e.Graphics.DrawImage(arrowImageList.Images[1], plusRect);
            else if (e.Node.IsExpanded == false && e.Node.Nodes.Count > 0)
                ;//e.Graphics.DrawImage(arrowImageList.Images[0], plusRect);


            /*测试用 画出+-号出现的矩形*/
            //if (e.Node.Nodes.Count > 0)
            //    e.Graphics.DrawRectangle(new Pen(Color.Red), plusRect);
            #endregion

            #region 3     画节点文本=========================================
            Rectangle nodeTextRect = new Rectangle(
                                                    e.Node.Bounds.Left,
                                                    e.Node.Bounds.Top + 2,
                                                    e.Node.Bounds.Width + 2,
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
                e.Graphics.DrawString(string.Format("({0})", e.Node.GetNodeCount(true)),
                                        new Font("Arial", 8),
                                        Brushes.Gray,
                                        nodeTextRect.Right - 4,
                                        nodeTextRect.Top + 2);
            }

            ///*测试用，画文字出现的矩形*/
            //if (e.Node.Text != "")
            //    e.Graphics.DrawRectangle(new Pen(Color.Blue), nodeTextRect);
            #endregion

            #region 4   画IImageList 中的图标===================================================================

            int currt_X = e.Node.Bounds.X;
            if (treeView1.ImageList != null && treeView1.ImageList.Images.Count > 0)
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
                //if (e.Node.ImageIndex > 0)
                //    e.Graphics.DrawRectangle(new Pen(Color.Black, 1), imagebox);
            }
            #endregion

            ///*测试 画所有的边框*/
            //nodeRect = new Rectangle(1,
            //             e.Bounds.Top + 1,
            //             e.Bounds.Width - 3,
            //             e.Bounds.Height - 2);
            //e.Graphics.DrawRectangle(new Pen(Color.Gray), nodeRect);
        }

        #endregion


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
            else
            { }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            ArrowKeyUp = (e.KeyCode == Keys.Up);
            if (e.KeyCode == Keys.Down)
            {
                Text = "Down";
            }

            ArrowKeyDown = (e.KeyCode == Keys.Down);
            if (e.KeyCode == Keys.Up)
            {
                Text = "UP";
            }

        }


        private bool ArrowKeyUp = false;
        private bool ArrowKeyDown = false;
        private System.Windows.Forms.ImageList arrowImageList1;
        private TreeView treeView1;

    }

    public class API
    {
        private const int WS_HSCROLL = 0x100000;
        private const int WS_VSCROLL = 0x200000;
        private const int GWL_STYLE = (-16);

        [DllImport("User32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, CursorType cursor);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);


        public const int WM_PRINTCLIENT = 0x0318;
        public const int PRF_CLIENT = 0x00000004;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// 显示系统光标小手
        /// 像Win7非经典主题的小手光标
        /// </summary>
        public static Cursor Hand
        {
            get
            {
                IntPtr h = LoadCursor(IntPtr.Zero, CursorType.IDC_HAND);
                return new Cursor(h);
            }
        }

        public static bool IsWinXP
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) &&
                    ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
            }
        }

        public static bool IsWinVista
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
            }
        }

        /// <summary>
        /// 判断是否出现垂直滚动条
        /// </summary>
        /// <param name="ctrl">待测控件</param>
        /// <returns>出现垂直滚动条返回true，否则为false</returns>
        public static bool IsVerticalScrollBarVisible(Control ctrl)
        {
            if (!ctrl.IsHandleCreated)
                return false;

            return (GetWindowLong(ctrl.Handle, GWL_STYLE) & WS_VSCROLL) != 0;
        }

        /// <summary>
        /// 判断是否出现水平滚动条
        /// </summary>
        /// <param name="ctrl">待测控件</param>
        /// <returns>出现水平滚动条返回true，否则为false</returns>
        public static bool IsHorizontalScrollBarVisible(Control ctrl)
        {
            if (!ctrl.IsHandleCreated)
                return false;
            return (GetWindowLong(ctrl.Handle, GWL_STYLE) & WS_HSCROLL) != 0;
        }
    }

    public enum CursorType : uint
    {
        IDC_ARROW = 32512U,
        IDC_IBEAM = 32513U,
        IDC_WAIT = 32514U,
        IDC_CROSS = 32515U,
        IDC_UPARROW = 32516U,
        IDC_SIZE = 32640U,
        IDC_ICON = 32641U,
        IDC_SIZENWSE = 32642U,
        IDC_SIZENESW = 32643U,
        IDC_SIZEWE = 32644U,
        IDC_SIZENS = 32645U,
        IDC_SIZEALL = 32646U,
        IDC_NO = 32648U,
        //小手
        IDC_HAND = 32649U,
        IDC_APPSTARTING = 32650U,
        IDC_HELP = 32651U
    }

}
