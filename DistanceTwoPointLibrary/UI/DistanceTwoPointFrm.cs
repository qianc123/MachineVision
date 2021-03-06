﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.IO;
using System.Threading;
//using MachineVision.ShuJuJieGou;

using RectLibrary;
using System.Diagnostics;
using MultTree;
using MultTree.MultTreeNodeStruct;
using LogClassLibrary;




namespace DistanceTwoPointLibrary.UI
{
    public partial class DistanceTwoPointFrm : Form
    {
        #region    全局变量

        #region 当前选择定位树节点的信息
        /// <summary>
        /// 当前选择定位树节点的信息
        /// </summary>
        MultTreeControlLibrary.tool_treeview_struct _tool_treeview_struct;
        #endregion

        #region   当前选择 工具跟随树节点的信息
        /// <summary>
        ///  当前选择 工具跟随树节点的信息
        /// </summary>
        MultTreeControlLibrary.tool_treeview_struct _tool_treeview_struct2;
        #endregion

        //#region    循环运行的标志
        ///// <summary>
        ///// 循环运行的标志
        ///// </summary>
        //public bool _xun_huan_yun_xing_biao_zhi = false;
        //#endregion

        //#region   读图的数据
        ///// <summary>
        ///// 读图的数据
        ///// </summary>
        ////   public ReadImageHalcon.ReadImageShuJu.IReadShuJu _IRead;
        //ReadImageHalconLibrary.IReadShuJu _IRead;
        //#endregion

        #region  委托,事件

        /// <summary>
        /// 委托
        /// </summary>
        /// <returns></returns>
        delegate bool _run_delegate();

        /// <summary>
        /// 事件 
        /// </summary>
        event _run_delegate _read;

        /// <summary>
        /// 运行事件
        /// </summary>
        event _run_delegate _run;

        #endregion

        #region   检测时间
        /// <summary>
        /// 检测时间
        /// </summary>
        Stopwatch _stopWatch = new Stopwatch();
        #endregion

        #region  当前的检测流
        /// <summary>
        /// 检测流
        /// </summary>
        CheckStreamLibrary.ICheckStream _ICheckStr;
        #endregion

        #region 测量两点的数据
        /// <summary>
        /// 测量两点的数据
        /// </summary>
        IDistanceTwoPointShuJu _iDistanceTwoPoint = null;
        #endregion

        #region  数据设置
        /// <summary>
        /// 数据设置
        /// </summary>
        Set_DistanceTwoPiont _setDistanceTwo = null;
        #endregion

        #region  处理的数据结果字典
        /// <summary>
        /// 处理的数据结果字典
        /// </summary>
        Dictionary<string, Object> _result = new Dictionary<string, object>();
        #endregion

        #endregion

        #region   构造函数
        public DistanceTwoPointFrm(CheckStreamLibrary.ICheckStream ICheckStr_)
        {
            InitializeComponent();
            _ICheckStr = ICheckStr_;
            //BeginInvoke(new Action(delegate
            //{
            //HOperatorSet.GenEmptyObj(out _ho_Image);
            //_ho_Image.Dispose();
            //}));
        }
        #endregion

        #region  初始化
        private void ParentFrm_Load(object sender, EventArgs e)
        {
            halconWinControl_ROI1.init();
            halconWinControl_ROI1.Repaint += ROIMoveEvent;

            _read += read_one_image;
            _run += run;

            #region 第一个点树
            //TreeNode tr_0 = new TreeNode();
            //tr_0.Text = "驱动1";
            //tr_0.Name = "System_1"; //新建一个驱动
            //if (treeView1.Nodes != null)
            //{
            //    treeView1.Nodes.Clear();
            //}//清空树
            //TreeStatic.load_MultTreeNode_To_TreeNode_ContainsSelf(tr_0, _ICheckStr.Check_Root);
            //treeView1.Nodes.Add(tr_0);//添加一个检测
            //treeView1.ExpandAll();

            MultTree.operationTreeViewTool.initTreeView(treeView1, _ICheckStr.Check_Root);

            #endregion

            #region   第二个点树
            //TreeNode tr_1 = new TreeNode();
            //tr_1.Text = "驱动1";
            //tr_1.Name = "System_1"; //新建一个驱动

            //if (treeView2.Nodes != null)
            //{
            //    treeView2.Nodes.Clear();
            //}//清空树

            //TreeStatic.load_MultTreeNode_To_TreeNode_ContainsSelf(tr_1, _ICheckStr.Check_Root);

            //treeView2.Nodes.Add(tr_1);//添加一个检测
            //treeView2.ExpandAll();
            
            MultTree.operationTreeViewTool.initTreeView(treeView2, _ICheckStr.Check_Root);
            
            #endregion
            
            #region  初始化工具
            _setDistanceTwo = new Set_DistanceTwoPiont();
            #endregion

#if DEBUG ==true
            if (TreeStatic.Mult_Tree_Node.Obj == null)
            {
                _iDistanceTwoPoint = new DistanceTwoPointShuJu();
            }
            else
            {
                _iDistanceTwoPoint = (DistanceTwoPointShuJu)TreeStatic.Mult_Tree_Node.Obj;
            }
#else
            
#endif

            #region  同步定位点
            this._setDistanceTwo.Set_showParameter(_iDistanceTwoPoint, listBox_DiYiGeDian, listBox_DiErGeDingWeiDian);
            #endregion
            
        }
        #endregion
        
        #region     运行
        private void tSB_run_one_Click(object sender, EventArgs e)
        {
            trigger_run();
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <returns></returns>
        bool run()
        {
            if (halconWinControl_ROI1.Exit_Image() == false)
            {
                return false;
            }
            _stopWatch.Restart();
            bool ok = false;

            //_distanceTwo.Analyze(this._iDistanceTwoPoint, this.halconWinControl_ROI1.Ho_Image);
            //_distanceTwo.show(this._iDistanceTwoPoint, halconWinControl_ROI1.HalconWindow1);
            this._result.Clear();
            this._iDistanceTwoPoint.analyze_show(halconWinControl_ROI1.HalconWindow1, "1", ref this._result);

            _stopWatch.Stop();

            Invoke(new Action(delegate
            {
                m_CtrlHStatusLabelCtrl.Text = _stopWatch.ElapsedMilliseconds.ToString();
                //if (_xun_huan_yun_xing_biao_zhi)
                //{
                //    Thread th = new Thread(new ThreadStart(() =>
                //    {
                //        trigger_read();
                //    }));
                //    th.Start();
                //}
            }));

            ok = true;
            return ok;
        }
        #endregion

        #region   读取一张图片事件

        bool read_one_image()
        {
            bool ok = false;
            try
            {
                _stopWatch.Restart();

                if (!halconWinControl_ROI1.Exit_Image())
                {
                    halconWinControl_ROI1.Ho_Image.Dispose();
                    //ReadImage.read_Image(_IRead);
                }

                halconWinControl_ROI1.Ho_Image = this._iDistanceTwoPoint.ImageFather.Ho_image;
                _stopWatch.Stop();

                Invoke(new Action(delegate
                {
                    m_CtrlHStatusLabelCtrl.Text = _stopWatch.ElapsedMilliseconds.ToString();
                }));

                //if (_xun_huan_yun_xing_biao_zhi == true)
                //{
                //    Thread thr = new Thread(new ThreadStart(trigger_read));
                //    thr.Start();
                //}

                #region   无用代码
                //int i = listBox_acquire_picture.Items.Count;
                //if (i > 0)
                //{
                //    if (_number_Image >= i)
                //    {
                //        _number_Image = 0;
                //    }

                //    ReadImageHalcon.Tool.readImage read = new ReadImageHalcon.Tool.readImage();
                //    listBox_acquire_picture.SelectedIndex = _number_Image;

                //    string path = listBox_acquire_picture.SelectedItem.ToString();
                //    _number_Image++;

                //    read.read_Image(path, halconWinControl_11.HalconWindow1);
                //    halconWinControl_11.Ho_Image = read.ho_Image;
                //    ok = true;

                //    if (_xun_huan_yun_xing_biao_zhi)
                //    {
                //        if (_read != null)
                //        {
                //            _read();
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogFile.Error, "取图片一张出错");
                MessageBox.Show("取图片一张出错:" + ex.Message);
                ok = false;
            }
            return ok;
        }

        #endregion

        #region  取一张图片
        private void tSB_read_image_Click(object sender, EventArgs e)
        {
            trigger_read();
        }
        #endregion

        #region 退出
        private void acquriefrm_FormClosing(object sender, FormClosingEventArgs e)
        {

            // MachineVision.ShuJuJieGou.TreeStatic.Mult_Tree_Node.Obj = _re;         

        }
        #endregion

        #region  无用代码
        #region 读取图片后的回调函数
        //void read_callBack(IAsyncResult re)
        //{
        //    if (_xun_huan_yun_xing_biao_zhi)
        //    {
        //        _read.BeginInvoke(read_callBack, null);
        //    }
        //}
        #endregion
        #endregion

        #region   触发取图
        /// <summary>
        /// 触发取图
        /// </summary>
        void trigger_read()
        {
            if (_read != null)
            {
                _read();
            }
        }
        #endregion

        #region  触发运行
        /// <summary>
        /// 触发运行
        /// </summary>
        void trigger_run()
        {
            if (_run != null)
            {
                _run();
            }
        }
        #endregion

        #region treeview  选择工具
        /// <summary>
        /// 选择工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tool_treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode current_treenode = e.Node;
            if (e.Node.Text == "驱动1")
            {
                return;
            }//预防卡死
            treeView1.Enabled = false;//预防频率过高，卡死            
            #region  取出节点数据
            _tool_treeview_struct.Current_Nodes_Name = e.Node.Name;//记录当前选择节点的名称
            _tool_treeview_struct.Current_Nodes_Text = e.Node.Text;

            MultTree.operationTreeViewTool.get_parent_node_path(ref current_treenode, ref _tool_treeview_struct);
            //get_parent_node_path(ref current_treenode, ref _tool_treeview_struct);
            #endregion
            treeView1.Enabled = true;//预防频率过高，卡死
            #region  无用代码
            //#region  取出节点数据 
            //_tool_treeview_struct.Current_Nodes_Name = e.Node.Name;//记录当前选择节点的名称
            //_tool_treeview_struct.Current_Nodes_Text = e.Node.Text;

            //#region  获取父节点名称
            //string parentnodepath = "";

            ////_tool_treeview_struct.current_nodes_parent.Clear();

            //for (bool parent = true; parent == true;)//获取当前节点的所有父节点的名称
            //{
            //    if (current_treenode.Parent != null)
            //    {
            //        //_tool_treeview_struct.current_nodes_parent.Insert(0, current_treenode.Parent.Name);
            //        parentnodepath = current_treenode.Parent.Name + '>' + parentnodepath;
            //        current_treenode = current_treenode.Parent;

            //    }
            //    else
            //    {
            //        parent = false;
            //    }
            //}

            //_tool_treeview_struct.Current_Node_lujing = parentnodepath + _tool_treeview_struct.Current_Nodes_Name;
            //#endregion

            //string[] node_name = _tool_treeview_struct.Current_Node_lujing.Split('>');

            //MultTreeNode_Interface mu;

            //mu = TreeStatic.get_Root().findNodeById(node_name[1]);

            //int node_name_number = node_name.Length;
            //for (int i = 2; i < node_name_number; i++)
            //{
            //    mu = mu.findNodeById(node_name[i]);
            //}
            //_rect_Mu = mu;
            //#endregion
            #endregion

            synchroinization_listBox_DingWeiGongJu();
        }
        #endregion

        #region   同步第一个定位点的列表
        /// <summary>
        /// 同步第一个定位点的列表
        /// </summary>
        void synchroinization_listBox_DingWeiGongJu()
        {
            if (this.listBox_DiYiGeDian.Items.Contains(_tool_treeview_struct.Current_Node_lujing))
            {
                this.listBox_DiYiGeDian.SelectedIndex = this.listBox_DiYiGeDian.Items.IndexOf(_tool_treeview_struct.Current_Node_lujing);
            }
            else
            {
                this.listBox_DiYiGeDian.ClearSelected();
            }
        }
        #endregion

        //#region  获取父节点路径
        ///// <summary>
        ///// 获取父节点路径
        ///// </summary>
        ///// <param name="current_treenode"></param>
        ///// <param name="_tool_treeview_struct"></param>
        //static void get_parent_node_path(ref  TreeNode current_treenode, ref  MultTreeControlLibrary.tool_treeview_struct _tool_treeview_struct)
        //{
        //    #region  获取父节点名称
        //    string parentnodepath = "";
        //    //_tool_treeview_struct.current_nodes_parent.Clear();
        //    for (bool parent = true; parent == true; )//获取当前节点的所有父节点的名称
        //    {
        //        if (current_treenode.Parent != null)
        //        {
        //            //_tool_treeview_struct.current_nodes_parent.Insert(0, current_treenode.Parent.Name);
        //            parentnodepath = current_treenode.Parent.Name + '>' + parentnodepath;
        //            current_treenode = current_treenode.Parent;
        //        }
        //        else
        //        {
        //            parent = false;
        //        }
        //    }
        //    _tool_treeview_struct.Current_Node_lujing = parentnodepath + _tool_treeview_struct.Current_Nodes_Name;
        //    #endregion

        //    string[] node_name = _tool_treeview_struct.Current_Node_lujing.Split('>');
        //    // MultTreeNode_Interface mu;
        //    IMultTreeNode mu;
        //    mu = TreeStatic.Root.findNodeById(node_name[1]);
        //    int node_name_number = node_name.Length;
        //    for (int i = 2; i < node_name_number; i++)
        //    {
        //        mu = mu.findNodeById(node_name[i]);
        //    }
        //    //TreeStatic.Mult_Tree_Node = mu;
        //    _tool_treeview_struct.Current_MultTreeNode = mu;

        //    #region  无用代码
        //    //#region 判断是不是取图节点
        //    //if (mu.SelfId.Contains("acquire"))
        //    //{
        //    //    //  MachineVision.ShuJuJieGou.TreeStatic.Mult_Tree_Node_Picture = mu;
        //    //    TreeStatic.Mult_Tree_Node_Picture = mu;
        //    //}
        //    //#endregion
        //    #endregion
        //}
        //#endregion

        #region    同步第二个定位点列表
        /// <summary>
        /// 同步第二个定位点列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode current_treenode = e.Node;
            if (e.Node.Text == "驱动1")
            {
                return;
            }//预防卡死

            treeView2.Enabled = false;//预防频率过高，卡死

            #region  取出节点数据
            _tool_treeview_struct2.Current_Nodes_Name = e.Node.Name;//记录当前选择节点的名称
            _tool_treeview_struct2.Current_Nodes_Text = e.Node.Text;

            MultTree.operationTreeViewTool.get_parent_node_path(ref current_treenode, ref _tool_treeview_struct2);
          //get_parent_node_path(ref current_treenode, ref _tool_treeview_struct2);
            #endregion

            treeView2.Enabled = true;//预防频率过高，卡死

            synchroinization_GenSuiDingWeiLieBian();
        }
        #endregion

        #region 同步第二个定位点列表
        /// <summary>
        /// 同步第二个定位点列表
        /// </summary>
        void synchroinization_GenSuiDingWeiLieBian()
        {
            if (this.listBox_DiErGeDingWeiDian.Items.Contains(_tool_treeview_struct2.Current_Node_lujing))
            {
                this.listBox_DiErGeDingWeiDian.SelectedIndex = this.listBox_DiErGeDingWeiDian.Items.IndexOf(_tool_treeview_struct2.Current_Node_lujing);
            }
            else
            {
                this.listBox_DiErGeDingWeiDian.ClearSelected();
            }
        }
        #endregion

        #region  确定第一个点
        private void button_QueDingDiYiGeDian_Click(object sender, EventArgs e)
        {
            queDingDiYiGeDingWeiDian();
        }

        /// <summary>
        /// 确定第一个点
        /// </summary>
        void queDingDiYiGeDingWeiDian()
        {
            if (_setDistanceTwo.Set_QueDingDiYiGeDian(this._iDistanceTwoPoint, _tool_treeview_struct.Current_MultTreeNode.Obj))
            {
                #region  同步数据
                if (listBox_DiYiGeDian.Items.Count > 0)
                {
                    listBox_DiYiGeDian.Items.Clear();
                }
                _iDistanceTwoPoint.PathOne = _tool_treeview_struct.Current_Node_lujing;
                listBox_DiYiGeDian.Items.Add(_iDistanceTwoPoint.PathOne);
                #endregion
            }
            else
            {
                MessageBox.Show("选择工具错误");
            }
        }
        #endregion

        #region   确定第二个点
        private void button_QueDingDiErGeDian_Click(object sender, EventArgs e)
        {
            queDingDiErGeDingWeiDian();
        }

        void queDingDiErGeDingWeiDian()
        {
            if (_setDistanceTwo.Set_QueDingDiErGeDian(this._iDistanceTwoPoint, _tool_treeview_struct2.Current_MultTreeNode.Obj))
            {
                #region  同步数据

                if (listBox_DiErGeDingWeiDian.Items.Count > 0)
                {
                    listBox_DiErGeDingWeiDian.Items.Clear();
                }

                _iDistanceTwoPoint.PathTwo = _tool_treeview_struct2.Current_Node_lujing;
                listBox_DiErGeDingWeiDian.Items.Add(_iDistanceTwoPoint.PathOne);
                #endregion
            }
            else
            {
                MessageBox.Show("选择工具错误");
            }
        }
        #endregion

        #region   图片移动是刷新
        /// <summary>
        /// 图片移动是刷新
        /// </summary>
        /// <param name="_halconWindow1"></param>
        void ROIMoveEvent(HWindow _halconWindow1)
        {
            this._iDistanceTwoPoint.show(_halconWindow1);
        }
        #endregion
    }
}
