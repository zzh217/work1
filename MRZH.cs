using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace MRZH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Gwmeiti[] gwmeitis = new Gwmeiti[100];
        Gnmeiti[] gnmeitis = new Gnmeiti[100];
        //string[] gwname = new string[100];
        //string[] gwdb = new string[100];
        //string[] gnname = new string[100];
        //string[] gndb = new string[100];
        string wnum;
        string nnum;


        Newsinfo[] choicenewsinfos = new Newsinfo[50];
        Newsinfo[] newsinfo_ft = new Newsinfo[6666];
        Newsinfo[] newsinfo_hls = new Newsinfo[6666];
        Newsinfo[] newsinfo_fhpl = new Newsinfo[6666];
        Newsinfo[] newsinfo_gmsp = new Newsinfo[6666];
        Newsinfo[] newsinfo_zaobao = new Newsinfo[6666];
        Newsinfo[] newsinfo_voa = new Newsinfo[6666];
        Newsinfo[] newsinfo_squtnik_eluosiweixingtongxunshe = new Newsinfo[6666];
        Newsinfo[][] newsinfos = new Newsinfo[66666][];
        

        public static int GetHanNumFromString(string str)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

            for (int i = 0; i < str.Length; i++)
            {
                if (regex.IsMatch(str[i].ToString()))
                {
                    count++;
                }
            }

            return count;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            string pathtext;
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+ @"\config.txt";//配置文件目录
            try
            {
                if (File.Exists(filePath))
                {
                    treeView1.LabelEdit = true;//可编辑状态
                    string rest = ".*&";
                    string rest2 = ".*#";
                    pathtext = File.ReadAllText(filePath);
                    byte[] mybyte = Encoding.UTF8.GetBytes(pathtext);
                    pathtext = Encoding.UTF8.GetString(mybyte);//若出现乱码问题，修改配置文件格式即可
                    Regex regnum = new Regex(@"WNUM\d+");
                    Match match = regnum.Match(pathtext);
                    wnum = match.Groups[0].Value.Replace("WNUM", "");//读取国外媒体数量
                    TreeNode nodegw = new TreeNode();//添加根节点
                    nodegw.Text = "国外媒体";
                    treeView1.Nodes.Add(nodegw);
                    //MessageBox.Show("wnum=" + wnum);
                    Regex regwmt = new Regex(@"GW[\s\S]*EGW");
                    Match match1 = regwmt.Match(pathtext);
                    string wmt = match1.Groups[0].Value.Replace("EGW", "").Replace("GW", "");//提取国外媒体配置信息
                    Regex regnnum = new Regex(@"NNUM\d+");
                    Match matchn = regnnum.Match(pathtext);
                    nnum = matchn.Groups[0].Value.Replace("NNUM", "");//读取国内媒体数量
                    TreeNode nodegn = new TreeNode();//添加根节点
                    nodegn.Text = "国内媒体";
                    treeView1.Nodes.Add(nodegn);
                    //MessageBox.Show("nnum=" + nnum);
                    Regex regnmt = new Regex(@"GN[\s\S]*EGN");
                    Match matchn1 = regnmt.Match(pathtext);
                    string nmt = matchn1.Groups[0].Value.Replace("EGN", "").Replace("GN", "");//提取国内媒体配置信息


                    TreeNode[] gwtreeNodes = new TreeNode[int.Parse(wnum)];//设置国外媒体树状图节点数组
                    for (int i = 0; i < int.Parse(wnum); i++)//根据配置文件生成树状结构
                    {
                        string regs = "#" + i.ToString() + rest;
                        Regex regwmt1 = new Regex(regs);
                        Match match2 = regwmt1.Match(wmt);
                        gwtreeNodes[i] = new TreeNode();
                        gwtreeNodes[i].Text = match2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", "");
                        gwmeitis[i].gwname = match2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", "");//将网站名称加如数组
                        string regs2 = "&" + rest2 + i.ToString();
                        Regex regwmt12 = new Regex(regs2);
                        Match match22 = regwmt12.Match(wmt);
                        gwmeitis[i].gwdb = match22.Groups[0].Value.Replace("&", "").Replace("#" + i.ToString(), "");//将网站数据库名加如数组
                        //MessageBox.Show(gwmeitis[i].gwdb);
                        nodegw.Nodes.Add(gwtreeNodes[i]);
                        //MessageBox.Show(match2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", ""));
                    }

                    TreeNode[] gntreeNodes = new TreeNode[int.Parse(nnum)];//设置国内媒体树状图节点数组
                    for (int i = 0; i < int.Parse(nnum); i++)//根据配置文件生成树状结构
                    {
                        string regs = "#" + i.ToString() + rest;
                        Regex regnmt1 = new Regex(regs);
                        Match matchn2 = regnmt1.Match(nmt);
                        gntreeNodes[i] = new TreeNode();
                        gntreeNodes[i].Text = matchn2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", "");
                        gnmeitis[i].gnname = matchn2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", "");//将网站名称加如数组
                        string regs2 = "&" + rest2 + i.ToString();
                        Regex regwmt12 = new Regex(regs2);
                        Match match22 = regwmt12.Match(nmt);
                        gnmeitis[i].gndb = match22.Groups[0].Value.Replace("&", "").Replace("#" + i.ToString(), "");//将网站数据库名加如数组
                        nodegn.Nodes.Add(gntreeNodes[i]);
                        //MessageBox.Show(matchn2.Groups[0].Value.Replace("#" + i.ToString(), "").Replace("&", ""));
                    }
                }
                else
                {
                    MessageBox.Show("配置文件不存在，请检查网络配置或者联系管理员");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FromClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();//退出应用程序
        }

        public string Onetoyi(int num)
        {
            string res = "";
            switch (num.ToString())
            {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
            }
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rq = DateTime.Now.ToLongDateString().ToString();
            MessageBox.Show("你选择了" + (numchoice-1).ToString() + "篇文章");
            if (jgflag ==4)
            {
                //F:\\TestTxt.txt不存在这个文档的话新建。
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt"))
                {
                    StreamWriter strmsave = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt", false, System.Text.Encoding.Default);
                    strmsave.Write("*****，" + DateTime.Now.ToLongDateString().ToString().Replace(DateTime.Now.Year.ToString() + "年", "") + "***媒体报道摘要如下:\r\n\r\n");
                    for (int i = 1; i <= 4; i++)
                    {
                        strmsave.Write("    " + choicenewsinfos[i].title + "\r\n");
                    }
                    strmsave.Write("\r\n");
                    string res;
                    for (int i = 1; i <= 4; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "。“" + choicenewsinfos[i].namemt + "”" + "\r\n");
                    }
                    strmsave.Write("\r\n    *****：\r\n");
                    for (int i = 1; i <= 4; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Write("    *****：\r\n");
                    for (int i = 5; i <= 6; i++)
                    {
                        res = Onetoyi(i-4);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Close();

                }
                //F:\\TestTxt.txt存在这个文档的话直接打开写入。
                else
                {
                    StreamWriter strmsave = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt", false, System.Text.Encoding.Default);
                    strmsave.Write(*****，" + DateTime.Now.ToLongDateString().ToString().Replace(DateTime.Now.Year.ToString() + "年", "") + "***媒体报道摘要如下:\r\n\r\n");
                    for (int i = 1; i <= 4; i++)
                    {
                        strmsave.Write("    " + choicenewsinfos[i].title + "\r\n");
                    }
                    strmsave.Write("\r\n");
                    string res;
                    for (int i = 1; i <= 4; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "。“"+choicenewsinfos[i].namemt+"”"+"\r\n");
                    }
                    strmsave.Write("\r\n    *****：\r\n");
                    for (int i = 1; i <= 4; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Write("    *****：\r\n");
                    for (int i = 5; i <= 6; i++)
                    {
                        res = Onetoyi(i-4);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Close();

                }
            }
            if ( jgflag ==6)
            {
                //F:\\TestTxt.txt不存在这个文档的话新建。
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt"))
                {
                    StreamWriter strmsave = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt", false, System.Text.Encoding.Default);
                    strmsave.Write("*****，" + DateTime.Now.ToLongDateString().ToString().Replace(DateTime.Now.Year.ToString() + "年", "") + "***媒体报道摘要如下:\r\n\r\n");
                    for (int i = 1; i <= 6; i++)
                    {
                        strmsave.Write("    " + choicenewsinfos[i].title + "\r\n");
                    }
                    strmsave.Write("\r\n");
                    string res;
                    for (int i = 1; i <= 6; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "。“" + choicenewsinfos[i].namemt + "”" + "\r\n");
                    }
                    strmsave.Write("\r\n    *****：\r\n");
                    for (int i = 1; i <= 6; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Write("    *****：\r\n");
                    for (int i = 7; i <= 8; i++)
                    {
                        res = Onetoyi(i - 6);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Close();

                }
                //F:\\TestTxt.txt存在这个文档的话直接打开写入。
                else
                {
                    StreamWriter strmsave = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\" + rq + ".txt", false, System.Text.Encoding.Default);
                    strmsave.Write("*****，" + DateTime.Now.ToLongDateString().ToString().Replace(DateTime.Now.Year.ToString() + "年", "") + "***媒体报道摘要如下:\r\n\r\n");
                    for (int i = 1; i <= 6; i++)
                    {
                        strmsave.Write("    " + choicenewsinfos[i].title + "\r\n");
                    }
                    strmsave.Write("\r\n");
                    string res;
                    for (int i = 1; i <= 6; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "。“" + choicenewsinfos[i].namemt + "”" + "\r\n");
                    }
                    strmsave.Write("\r\n    *****：\r\n");
                    for (int i = 1; i <= 6; i++)
                    {
                        res = Onetoyi(i);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Write("    *****：\r\n");
                    for (int i = 7; i <= 8; i++)
                    {
                        res = Onetoyi(i - 6);
                        strmsave.Write("    " + res + "、" + choicenewsinfos[i].title + "\r\n" + "    " + choicenewsinfos[i].namemt + "  " + choicenewsinfos[i].time + "  " + choicenewsinfos[i].writer + "\r\n" + choicenewsinfos[i].content + "\r\n\r\n");
                    }
                    strmsave.Close();

                }
            }
            MessageBox.Show("已生成！请在桌面查看"+rq + ".txt");
        }

        //string[] newsnr = new string[100];
        string[] newstitle_ft = new string[100];
        string[] newstitle_hls = new string[100];
        string[] newstitle_fhpl = new string[100];
        string[] newstitle_gmsp = new string[100];
        string[] newstitle_voa = new string[100];
        string[] newstitle_zaobao = new string[100];
        string[] newstitle_squtnik_eluosiweixingtongxunshe = new string[100];
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (treeView1.SelectedNode.Text == "FT中文网")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM ft_chinese_website WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_ft).Contains("FT" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_ft[newsnum] = "FT" + newsnum + " " + rd["title"].ToString();
                        newsinfo_ft[newsnum].title = rd["title"].ToString();
                        newsinfo_ft[newsnum].content = "    "+rd["content"].ToString().Replace("版权声明：本文版权归FT中文网所有，未经允许任何单位或个人不得转载，复制或以任何其他方式使用本文全部或部分，侵权必究。", "").Replace("收藏\r\n", "").Replace("\r\n", "\r\n    ");
                        newsinfo_ft[newsnum].id = rd["ID"].ToString();
                        newsinfo_ft[newsnum].flag = rd["flag"].ToString();
                        newsinfo_ft[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_ft[newsnum].time = rd["time"].ToString();
                        newsinfo_ft[newsnum].writer = rd["writer"].ToString();
                        newsinfo_ft[newsnum].url = rd["url"].ToString();
                    }
                    if(newsinfo_ft[newsnum].flag==1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("FT" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("FT" + newsnum + " " + rd["title"].ToString());
                    }
                    
                    newsnum++;
                }
                
                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_ft)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"FT\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    //MessageBox.Show(matchnr.Groups[0].Value.Replace("FT", ""));
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_ft[int.Parse(matchnr.Groups[0].Value.Replace("FT", ""))].content).ToString();
                    label2.Text = newsinfo_ft[int.Parse(matchnr.Groups[0].Value.Replace("FT", ""))].time;
                    textBox1.Text = newsinfo_ft[int.Parse(matchnr.Groups[0].Value.Replace("FT", ""))].content;
                }
            }



            if (treeView1.SelectedNode.Text == "韩联社")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM hanlianshe WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_hls).Contains("hls" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_hls[newsnum] = "hls" + newsnum + " " + rd["title"].ToString();
                        newsinfo_hls[newsnum].title =rd["title"].ToString();
                        newsinfo_hls[newsnum].content = "    " + rd["content"].ToString().Replace("【版权归韩联社所有，未经授权严禁转载复制】", "").Replace("\r\n", "\r\n    ");
                        newsinfo_hls[newsnum].id = rd["ID"].ToString();
                        newsinfo_hls[newsnum].flag = rd["flag"].ToString();
                        newsinfo_hls[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_hls[newsnum].time = rd["time"].ToString();
                        newsinfo_hls[newsnum].writer = rd["writer"].ToString();
                        newsinfo_hls[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_hls[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("hls" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("hls" + newsnum + " " + rd["title"].ToString());
                    }
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_hls)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"hls\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    //MessageBox.Show(matchnr.Groups[0].Value.Replace("hls", ""));
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_hls[int.Parse(matchnr.Groups[0].Value.Replace("hls", ""))].content).ToString();
                    label2.Text = newsinfo_hls[int.Parse(matchnr.Groups[0].Value.Replace("hls", ""))].time;
                    textBox1.Text = newsinfo_hls[int.Parse(matchnr.Groups[0].Value.Replace("hls", ""))].content;
                }
            }



            if (treeView1.SelectedNode.Text == "凤凰评论")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM fhpl WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_fhpl).Contains("fhpl" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_fhpl[newsnum] = "fhpl" + newsnum + " " + rd["title"].ToString();
                        newsinfo_fhpl[newsnum].title = rd["title"].ToString();
                        newsinfo_fhpl[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                        newsinfo_fhpl[newsnum].id = rd["ID"].ToString();
                        newsinfo_fhpl[newsnum].flag = rd["flag"].ToString();
                        newsinfo_fhpl[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_fhpl[newsnum].time = rd["time"].ToString();
                        newsinfo_fhpl[newsnum].writer = rd["writer"].ToString();
                        newsinfo_fhpl[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_fhpl[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("fhpl" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("fhpl" + newsnum + " " + rd["title"].ToString());
                    }
                    
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_fhpl)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"fhpl\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_fhpl[int.Parse(matchnr.Groups[0].Value.Replace("fhpl", ""))].content).ToString();
                    label2.Text = newsinfo_fhpl[int.Parse(matchnr.Groups[0].Value.Replace("fhpl", ""))].time;
                    textBox1.Text = newsinfo_fhpl[int.Parse(matchnr.Groups[0].Value.Replace("fhpl", ""))].content;
                }
            }




            if (treeView1.SelectedNode.Text == "光明时评")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM gmsp WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_gmsp).Contains("gmsp" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_gmsp[newsnum] = "gmsp" + newsnum + " " + rd["title"].ToString();
                        newsinfo_gmsp[newsnum].title = rd["title"].ToString();
                        newsinfo_gmsp[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                        newsinfo_gmsp[newsnum].id = rd["ID"].ToString();
                        newsinfo_gmsp[newsnum].flag = rd["flag"].ToString();
                        newsinfo_gmsp[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_gmsp[newsnum].time = rd["time"].ToString();
                        newsinfo_gmsp[newsnum].writer = rd["writer"].ToString();
                        newsinfo_gmsp[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_gmsp[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("gmsp" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("gmsp" + newsnum + " " + rd["title"].ToString());
                    }
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_gmsp)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"gmsp\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_gmsp[int.Parse(matchnr.Groups[0].Value.Replace("gmsp", ""))].content).ToString();
                    label2.Text = newsinfo_gmsp[int.Parse(matchnr.Groups[0].Value.Replace("gmsp", ""))].time;
                    textBox1.Text = newsinfo_gmsp[int.Parse(matchnr.Groups[0].Value.Replace("gmsp", ""))].content;
                }
            }


            if (treeView1.SelectedNode.Text == "美国之音")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM voa WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_voa).Contains("voa" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_voa[newsnum] = "voa" + newsnum + " " + rd["title"].ToString();
                        newsinfo_voa[newsnum].title = rd["title"].ToString();
                        newsinfo_voa[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                        newsinfo_voa[newsnum].id = rd["ID"].ToString();
                        newsinfo_voa[newsnum].flag = rd["flag"].ToString();
                        newsinfo_voa[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_voa[newsnum].time = rd["time"].ToString();
                        newsinfo_voa[newsnum].writer = rd["writer"].ToString();
                        newsinfo_voa[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_voa[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("voa" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("voa" + newsnum + " " + rd["title"].ToString());
                    }
                    
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_voa)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"voa\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_voa[int.Parse(matchnr.Groups[0].Value.Replace("voa", ""))].content).ToString();
                    label2.Text = newsinfo_voa[int.Parse(matchnr.Groups[0].Value.Replace("voa", ""))].time;
                    textBox1.Text = newsinfo_voa[int.Parse(matchnr.Groups[0].Value.Replace("voa", ""))].content;
                }
            }




            if (treeView1.SelectedNode.Text == "联合早报")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM zaobao WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_zaobao).Contains("zaobao" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_zaobao[newsnum] = "zaobao" + newsnum + " " + rd["title"].ToString();
                        newsinfo_zaobao[newsnum].title = rd["title"].ToString().Replace(" | 联合早报网", "");
                        newsinfo_zaobao[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                        newsinfo_zaobao[newsnum].id = rd["ID"].ToString();
                        newsinfo_zaobao[newsnum].flag = rd["flag"].ToString();
                        newsinfo_zaobao[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_zaobao[newsnum].time = rd["time"].ToString();
                        newsinfo_zaobao[newsnum].writer = rd["writer"].ToString();
                        newsinfo_zaobao[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_zaobao[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("zaobao" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("zaobao" + newsnum + " " + rd["title"].ToString());
                    }
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_zaobao)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"zaobao\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_zaobao[int.Parse(matchnr.Groups[0].Value.Replace("zaobao", ""))].content).ToString();
                    label2.Text = newsinfo_zaobao[int.Parse(matchnr.Groups[0].Value.Replace("zaobao", ""))].time;
                    textBox1.Text = newsinfo_zaobao[int.Parse(matchnr.Groups[0].Value.Replace("zaobao", ""))].content;
                }
            }


            if (treeView1.SelectedNode.Text == "俄罗斯卫星通讯社")
            {
                int a = 0;
                treeView1.SelectedNode.Nodes.Clear();
                //Array.Clear(newsnr, 0, newsnr.Length);
                //Array.Clear(newstitle, 0, newstitle.Length);
                int newsnum = 1;
                string dayy = DateTime.Now.DayOfYear.ToString();
                string sqltext = "SELECT * FROM squtnik_eluosiweixingtongxunshe WHERE " + dayy + "-ID<=2";//选取近两天的数据
                //MessageBox.Show(sqltext);
                MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                while (rd.Read())
                {
                    //MessageBox.Show(rd.ToString());
                    //判断是否已经包含数据
                    bool exists = ((IList)newstitle_squtnik_eluosiweixingtongxunshe).Contains("squtnik" + newsnum + " " + rd["title"].ToString());
                    if (exists)
                        a = 1;
                    else
                    {
                        newstitle_squtnik_eluosiweixingtongxunshe[newsnum] = "squtnik" + newsnum + " " + rd["title"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].title = rd["title"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].id = rd["ID"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].flag = rd["flag"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].synopsis = rd["synopsis"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].time = rd["time"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].writer = rd["writer"].ToString();
                        newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].url = rd["url"].ToString();
                    }
                    if (newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].flag == 1.ToString())
                    {
                        treeView1.SelectedNode.Nodes.Add("squtnik" + newsnum + " " + rd["title"].ToString()).BackColor = Color.Red;
                    }
                    else
                    {
                        treeView1.SelectedNode.Nodes.Add("squtnik" + newsnum + " " + rd["title"].ToString());
                    }
                    newsnum++;
                }

                //MessageBox.Show(treeView1.SelectedNode.Text);
            }
            foreach (string ss in newstitle_squtnik_eluosiweixingtongxunshe)
            {
                if (treeView1.SelectedNode.Text == ss)
                {
                    string regs = @"squtnik\d+";
                    Regex xuanzr = new Regex(regs);
                    Match matchnr = xuanzr.Match(ss);
                    textBox1.Clear();
                    label1.Text = GetHanNumFromString(newsinfo_squtnik_eluosiweixingtongxunshe[int.Parse(matchnr.Groups[0].Value.Replace("squtnik", ""))].content).ToString();
                    label2.Text = newsinfo_squtnik_eluosiweixingtongxunshe[int.Parse(matchnr.Groups[0].Value.Replace("squtnik", ""))].time;
                    textBox1.Text = newsinfo_squtnik_eluosiweixingtongxunshe[int.Parse(matchnr.Groups[0].Value.Replace("squtnik", ""))].content;
                }
            }

        }

        string[] choice_title = new string[100];
        int jgflag;
        int numchoice = 1;
        int numszbh;
        string nameszmc;
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {                
                bool exists = ((IList)choice_title).Contains(treeView1.SelectedNode.Text);
                if (exists)
                    MessageBox.Show("已存在");
                else
                {
                    string regs1 = @".+?\d+";
                    Regex shuzi1 = new Regex(regs1);
                    Match matchnsz1 = shuzi1.Match(treeView1.SelectedNode.Text);
                    //MessageBox.Show(matchnsz1.Groups[0].ToString());

                    string regs = @"\d+";
                    Regex shuzi = new Regex(regs);
                    Match matchnsz = shuzi.Match(matchnsz1.Groups[0].ToString());
                    numszbh = int.Parse(matchnsz.Groups[0].ToString());
                    nameszmc = matchnsz1.Groups[0].ToString().Replace(matchnsz.Groups[0].ToString(), "");
                    nameszmc = Regex.Replace(nameszmc, @"\d+", "", RegexOptions.IgnoreCase).Replace(" ", "");
                    //MessageBox.Show(numszbh.ToString() + "     " + nameszmc);
                    //MessageBox.Show(matchnsz.Groups[0].ToString());

                    choice_title[numchoice] = treeView1.SelectedNode.Text;
                    switch (nameszmc)
                    {
                        case "hls":

                            choicenewsinfos[numchoice] = newsinfo_hls[numszbh];
                            string sqltext = "UPDATE hanlianshe set flag = 1 where url = '" + choicenewsinfos[numchoice].url+"'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext);
                            choicenewsinfos[numchoice] = newsinfo_hls[numszbh];
                            choicenewsinfos[numchoice].namemt = "韩联社";
                            treeView1.SelectedNode.BackColor = Color.Red;
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "FT":
                            choicenewsinfos[numchoice] = newsinfo_ft[numszbh];
                            string sqltext1 = "UPDATE ft_chinese_website set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext1);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_ft[numszbh];
                            choicenewsinfos[numchoice].namemt = "FT中文网";
                            
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "fhpl":
                            choicenewsinfos[numchoice] = newsinfo_fhpl[numszbh];
                            string sqltext2 = "UPDATE fhpl set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext2);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_fhpl[numszbh];
                            choicenewsinfos[numchoice].namemt = "凤凰评论";
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "gmsp":
                            choicenewsinfos[numchoice] = newsinfo_gmsp[numszbh];
                            string sqltext3 = "UPDATE gmsp set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext3);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_gmsp[numszbh];
                            choicenewsinfos[numchoice].namemt = "光明时评";
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "zaobao":
                            choicenewsinfos[numchoice].namemt = "联合早报";
                            string sqltext4 = "UPDATE zaobao set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext4);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_zaobao[numszbh];
                            choicenewsinfos[numchoice].namemt = "联合早报";
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "voa":
                            choicenewsinfos[numchoice].namemt = "美国之音";
                            string sqltext5 = "UPDATE voa set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext5);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_voa[numszbh];
                            choicenewsinfos[numchoice].namemt = "美国之音";
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;
                        case "squtnik":
                            choicenewsinfos[numchoice] = newsinfo_squtnik_eluosiweixingtongxunshe[numszbh];
                            string sqltext6 = "UPDATE squtnik_eluosiweixingtongxunshe set flag = 1 where url = '" + choicenewsinfos[numchoice].url + "'; ";
                            MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext6);
                            treeView1.SelectedNode.BackColor = Color.Red;
                            choicenewsinfos[numchoice] = newsinfo_squtnik_eluosiweixingtongxunshe[numszbh];
                            choicenewsinfos[numchoice].namemt = "俄罗斯卫星通讯社";
                            //MessageBox.Show(choicenewsinfos[numchoice].title);
                            break;

                    }
                    //MessageBox.Show(numchoice.ToString());
                    listBox1.Items.Add(numchoice.ToString() + "#   " + treeView1.SelectedNode.Text);
                    numchoice++;
                    if (numchoice == 6)
                        jgflag = 4;
                    if (numchoice == 8)
                        jgflag = 6;

                }

            }
            catch
            {

                if (treeView1.SelectedNode.Text == "FT中文网")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM ft_chinese_website WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                                 //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_ft).Contains("FT" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_ft[newsnum] = "FT" + newsnum + " " + rd["title"].ToString();
                            newsinfo_ft[newsnum].title = rd["title"].ToString();
                            newsinfo_ft[newsnum].content = "    " + rd["content"].ToString().Replace("版权声明：本文版权归FT中文网所有，未经允许任何单位或个人不得转载，复制或以任何其他方式使用本文全部或部分，侵权必究。", "").Replace("收藏\r\n", "").Replace("\r\n", "\r\n    ");
                            newsinfo_ft[newsnum].id = rd["ID"].ToString();
                            newsinfo_ft[newsnum].flag = rd["flag"].ToString();
                            newsinfo_ft[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_ft[newsnum].time = rd["time"].ToString();
                            newsinfo_ft[newsnum].writer = rd["writer"].ToString();
                            newsinfo_ft[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("FT" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                



                if (treeView1.SelectedNode.Text == "韩联社")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM hanlianshe WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                         //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_hls).Contains("hls" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_hls[newsnum] = "hls" + newsnum + " " + rd["title"].ToString();
                            newsinfo_hls[newsnum].title = rd["title"].ToString();
                            newsinfo_hls[newsnum].content = "    " + rd["content"].ToString().Replace("【版权归韩联社所有，未经授权严禁转载复制】", "").Replace("\r\n", "\r\n    ");
                            newsinfo_hls[newsnum].id = rd["ID"].ToString();
                            newsinfo_hls[newsnum].flag = rd["flag"].ToString();
                            newsinfo_hls[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_hls[newsnum].time = rd["time"].ToString();
                            newsinfo_hls[newsnum].writer = rd["writer"].ToString();
                            newsinfo_hls[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("hls" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                



                if (treeView1.SelectedNode.Text == "凤凰评论")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM fhpl WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                   //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_fhpl).Contains("fhpl" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_fhpl[newsnum] = "fhpl" + newsnum + " " + rd["title"].ToString();
                            newsinfo_fhpl[newsnum].title = rd["title"].ToString();
                            newsinfo_fhpl[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                            newsinfo_fhpl[newsnum].id = rd["ID"].ToString();
                            newsinfo_fhpl[newsnum].flag = rd["flag"].ToString();
                            newsinfo_fhpl[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_fhpl[newsnum].time = rd["time"].ToString();
                            newsinfo_fhpl[newsnum].writer = rd["writer"].ToString();
                            newsinfo_fhpl[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("fhpl" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                




                if (treeView1.SelectedNode.Text == "光明时评")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM gmsp WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                   //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_gmsp).Contains("gmsp" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_gmsp[newsnum] = "gmsp" + newsnum + " " + rd["title"].ToString();
                            newsinfo_gmsp[newsnum].title = rd["title"].ToString();
                            newsinfo_gmsp[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                            newsinfo_gmsp[newsnum].id = rd["ID"].ToString();
                            newsinfo_gmsp[newsnum].flag = rd["flag"].ToString();
                            newsinfo_gmsp[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_gmsp[newsnum].time = rd["time"].ToString();
                            newsinfo_gmsp[newsnum].writer = rd["writer"].ToString();
                            newsinfo_gmsp[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("gmsp" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                


                if (treeView1.SelectedNode.Text == "美国之音")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM voa WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                  //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_voa).Contains("voa" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_voa[newsnum] = "voa" + newsnum + " " + rd["title"].ToString();
                            newsinfo_voa[newsnum].title = rd["title"].ToString();
                            newsinfo_voa[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                            newsinfo_voa[newsnum].id = rd["ID"].ToString();
                            newsinfo_voa[newsnum].flag = rd["flag"].ToString();
                            newsinfo_voa[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_voa[newsnum].time = rd["time"].ToString();
                            newsinfo_voa[newsnum].writer = rd["writer"].ToString();
                            newsinfo_voa[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("voa" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                




                if (treeView1.SelectedNode.Text == "联合早报")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM zaobao WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                     //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_zaobao).Contains("zaobao" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_zaobao[newsnum] = "zaobao" + newsnum + " " + rd["title"].ToString();
                            newsinfo_zaobao[newsnum].title = rd["title"].ToString().Replace(" | 联合早报网", "");
                            newsinfo_zaobao[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                            newsinfo_zaobao[newsnum].id = rd["ID"].ToString();
                            newsinfo_zaobao[newsnum].flag = rd["flag"].ToString();
                            newsinfo_zaobao[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_zaobao[newsnum].time = rd["time"].ToString();
                            newsinfo_zaobao[newsnum].writer = rd["writer"].ToString();
                            newsinfo_zaobao[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("zaobao" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                


                if (treeView1.SelectedNode.Text == "俄罗斯卫星通讯社")
                {
                    int a = 0;
                    treeView1.SelectedNode.Nodes.Clear();
                    //Array.Clear(newsnr, 0, newsnr.Length);
                    //Array.Clear(newstitle, 0, newstitle.Length);
                    int newsnum = 1;
                    string dayy = DateTime.Now.DayOfYear.ToString();
                    string sqltext = "SELECT * FROM squtnik_eluosiweixingtongxunshe WHERE " + dayy + "-ID<=2";//选取近两天的数据
                                                                                                              //MessageBox.Show(sqltext);
                    MySqlDataReader rd = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sqltext);
                    while (rd.Read())
                    {
                        //MessageBox.Show(rd.ToString());
                        //判断是否已经包含数据
                        bool exists = ((IList)newstitle_squtnik_eluosiweixingtongxunshe).Contains("squtnik" + newsnum + " " + rd["title"].ToString());
                        if (exists)
                            a = 1;
                        else
                        {
                            newstitle_squtnik_eluosiweixingtongxunshe[newsnum] = "squtnik" + newsnum + " " + rd["title"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].title = rd["title"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].content = "    " + rd["content"].ToString().Replace("\r\n", "\r\n    ");
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].id = rd["ID"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].flag = rd["flag"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].synopsis = rd["synopsis"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].time = rd["time"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].writer = rd["writer"].ToString();
                            newsinfo_squtnik_eluosiweixingtongxunshe[newsnum].url = rd["url"].ToString();
                        }
                        treeView1.SelectedNode.Nodes.Add("squtnik" + newsnum + " " + rd["title"].ToString());
                        newsnum++;
                    }

                    //MessageBox.Show(treeView1.SelectedNode.Text);
                }
                
            }
        }
        int numcho = 0;
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string index = this.listBox1.SelectedItem.ToString();
            string regs1 = @"\d#";
            Regex shuzi1 = new Regex(regs1);
            Match matchnsz1 = shuzi1.Match(index);
            //MessageBox.Show(matchnsz1.Groups[0].ToString().Replace("#",""));
            numcho = int.Parse(matchnsz1.Groups[0].ToString().Replace("#", ""));
            for (int i = numcho; i < 49; i++)
            {
                choice_title[i] = choice_title[i + 1];
                choicenewsinfos[i] = choicenewsinfos[i+1];
            }
            numchoice--;
            if (numchoice == 6)
                jgflag = 4;
            if (numchoice == 8)
                jgflag = 6;
            listBox1.Items.Clear();
            for (int i = 1; i < numchoice; i++)
            {
                listBox1.Items.Add(i.ToString() + "#   " + choice_title[i]);
            }
            //MessageBox.Show(index);
            
        }
    }

    public struct Newsinfo
    {
        public string url;
        public string title;
        public string time;
        public string content;
        public string id;
        public string flag;
        public string writer;
        public string synopsis;
        public string namemt;
    }
    public struct Gwmeiti
    {
        
        public string gwname;
        public string gwdb;
    }
    public struct Gnmeiti
    {
        public string gnname;
        public string gndb;
    }
}
