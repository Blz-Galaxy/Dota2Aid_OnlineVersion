using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Dota2Aid
{
    public partial class MainFrm : Form
    {
        DataTable heroDataTable;
        DataTable antiDataTable;
        DataTable combDataTabel;
        string html;
        Dictionary<string, string> Ch2En = new Dictionary<string, string>();
        IList<string> heroList = new List<string>();
        AboutBox ab = new AboutBox();

        public MainFrm()
        {
            InitializeComponent();
            newIndex();
            ab.Hide();
            this.Text = ab.AssemblyProduct;
        }

        public static IList<string> myClone( IList<string> source)
        {
            IList<string> newList = new List<string>(source.Count);
            foreach (var item in source)
            {
                newList.Add(item);
            }
            return newList;
        }

        private string GetWebContent(string Url)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //声明一个HttpWebRequest请求 
                request.Timeout = 30000;
                //设置连接超时时间 
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("utf-8");
                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                strResult = streamReader.ReadToEnd();
            }
            catch
            {
                MessageBox.Show("获取信息失败，请检查网络连接");
            }
            return strResult;
        }

        private void getHeros()
        {
            heroDataTable = new DataTable("heros");
            heroDataTable.Columns.Add("英雄名", typeof(string));            

            //要抓取的URL地址 
            string Url = "http://www.dotamax.com/hero/";
            //得到指定Url的源码 
            string html = GetWebContent(Url);

            string EnName, ChName;
            string key;
            int index = 0;
            //string output = "";
            int count = 0;

            do
            {
                key = "onclick=\"DoNav('/hero/detail/";
                int i = html.IndexOf(key, index);

                if (i == -1)
                    break;

                i += key.Length;
                int j = html.IndexOf("/", i);
                EnName = html.Substring(i, j - i);

                key = "<span style=\"color:#ccc !important;text-align: center;\">";
                i = html.IndexOf(key, j + 1);
                i += key.Length;
                j = html.IndexOf(" </span>", i);
                ChName = html.Substring(i, j - i);


                Ch2En.Add(ChName, EnName);
                heroList.Add(ChName);

                DataRow dr = heroDataTable.NewRow();
                dr["英雄名"] = ChName;
                heroDataTable.Rows.Add(dr);
                count++;

                index = j;

            } while (true);
            comboBox1.DataSource = myClone(heroList);
            comboBox2.DataSource = myClone(heroList);
            comboBox3.DataSource = myClone(heroList);
            comboBox4.DataSource = myClone(heroList);
            comboBox5.DataSource = myClone(heroList);
            comboBox6.DataSource = myClone(heroList);
            comboBox7.DataSource = myClone(heroList);
            comboBox8.DataSource = myClone(heroList);
            comboBox9.DataSource = myClone(heroList);
            comboBox10.DataSource = myClone(heroList);
        }
            

        private void newIndex()
        { 
            antiDataTable = new DataTable();
            combDataTabel = new DataTable();
            getHeros();

            heroDataTable.Columns.Add("推荐指数", typeof(double));
            heroDataTable.Columns.Add("克制1号", typeof(double));
            heroDataTable.Columns.Add("克制2号", typeof(double));
            heroDataTable.Columns.Add("克制3号", typeof(double));
            heroDataTable.Columns.Add("克制4号", typeof(double));
            heroDataTable.Columns.Add("克制5号", typeof(double));
            heroDataTable.Columns.Add("配合6号", typeof(double));
            heroDataTable.Columns.Add("配合7号", typeof(double));
            heroDataTable.Columns.Add("配合8号", typeof(double));
            heroDataTable.Columns.Add("配合9号", typeof(double));
            heroDataTable.Columns.Add("配合10号", typeof(double));
            for(int i = 0;i<Ch2En.Count;i++)
            {
                for (int j = 1; j < 12; j++)
                {
                    heroDataTable.Rows[i][j] = 0.0;
                }
            }    

            //将ds 数据集绑定到 dataGridView数据集里边
            dataGridView1.DataSource = heroDataTable; 
        }


        private void addAntiIndex(string hero,int no)
        {
            no++;

            string CurEnName = Ch2En[hero];
            string CurChName = hero;
            string Url = "http://www.dotamax.com/hero/detail/match_up_anti/" + CurEnName + "/";
            //得到指定Url的源码 
            html = GetWebContent(Url);


            string AntiName, AntiValue, WinRate, UsedTime;
            string key;            
            int index = 0;;

            do
            {
                key = "<span class=\"hero-name-list\">";
                int i = html.IndexOf(key, index);

                if (i == -1)
                {
                    autoSorting();
                    return;
                }

                i += key.Length;
                int j = html.IndexOf("</span>", i);
                AntiName = html.Substring(i, j - i);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 1);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                AntiValue = html.Substring(i, j - i);

                //去除反抓取
                j = html.IndexOf("-->", j);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 3);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                WinRate = html.Substring(i, j - i);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 1);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                UsedTime = html.Substring(i, j - i);

                index = j;


                AntiValue = AntiValue.Substring(0, AntiValue.Length - 1);
                double value = Convert.ToDouble(AntiValue);
                int t_no = findHero(AntiName);
                heroDataTable.Rows[t_no][no] = -value;

                double sum = 0;
                for (int h = 2; h < 12; h++)
                {
                    sum += (double)heroDataTable.Rows[t_no][h];
                }
                heroDataTable.Rows[t_no][1] = sum;               
            } while (true);            
        }

        private void addCombIndex(string hero, int no)
        {
            no++;

            string CurEnName = Ch2En[hero];
            string CurChName = hero;
            string Url = "http://www.dotamax.com/hero/detail/match_up_comb/" + CurEnName + "/";
            //得到指定Url的源码 
            html = GetWebContent(Url);


            string CombName, CombValue, WinRate, UsedTime;
            string key;
            int index = 0; ;

            do
            {
                key = "<span class=\"hero-name-list\">";
                int i = html.IndexOf(key, index);

                if (i == -1)
                {
                    autoSorting();
                    return;
                }

                i += key.Length;
                int j = html.IndexOf("</span>", i);
                CombName = html.Substring(i, j - i);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 1);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                CombValue = html.Substring(i, j - i);

                //去除反抓取
                j = html.IndexOf("-->", j);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 3);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                WinRate = html.Substring(i, j - i);

                key = "<div style=\"height: 10px\">";
                i = html.IndexOf(key, j + 1);
                i += key.Length;
                j = html.IndexOf("</div>", i);
                UsedTime = html.Substring(i, j - i);

                index = j;


                CombValue = CombValue.Substring(0, CombValue.Length - 1);
                double value = Convert.ToDouble(CombValue);
                int t_no = findHero(CombName);
                heroDataTable.Rows[t_no][no] = value;

                double sum = 0;
                for (int h = 2; h < 12; h++)
                {
                    sum += (double)heroDataTable.Rows[t_no][h];
                }
                heroDataTable.Rows[t_no][1] = sum;
            } while (true); 
        }

        private int findHero(string name)
        {
            for (int i = 0; i < heroDataTable.Rows.Count; i++)
            {
                if (heroDataTable.Rows[i][0].ToString() == name)
                    return i;
            }
            MessageBox.Show("找不到" + name);
            return -1;
        }

        private void autoSorting()
        {
            //设定排序的列
            DataGridViewColumn sortColumn = dataGridView1.Columns[1];

            //设定排序的方向（升序、降序）
            ListSortDirection sortDirection = ListSortDirection.Descending;
            //if (dataGridView1.SortedColumn != null && dataGridView1.SortedColumn.Equals(sortColumn))
            //{
            //    sortDirection = dataGridView1.SortOrder == System.Windows.Forms.SortOrder.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            //}

            //进行排序
            dataGridView1.Sort(sortColumn, sortDirection);
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
        }

        private void 更新UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void 置于顶层TopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            置于顶层TopToolStripMenuItem.Checked = !置于顶层TopToolStripMenuItem.Checked;
        }

        private void 关于AToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            ab.Show();
        }

        private Image getImageFrom(string url)
        {
            WebRequest webreq = WebRequest.Create(url);
            //webreq.Method = "GET";
            //红色部分为文件URL地址，这里是一张图片
            WebResponse webres = webreq.GetResponse();
            Stream stream = webres.GetResponseStream();
            System.Drawing.Image image;
            image = System.Drawing.Image.FromStream(stream);
            stream.Close();
            return image;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addAntiIndex(comboBox1.Text, 1);
            pictureBox1.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addAntiIndex(comboBox2.Text, 2);
            pictureBox2.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addAntiIndex(comboBox3.Text, 3);
            pictureBox3.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addAntiIndex(comboBox4.Text, 4);
            pictureBox4.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addAntiIndex(comboBox5.Text, 5);
            pictureBox5.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addCombIndex(comboBox10.Text, 10);
            pictureBox10.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            pictureBox9.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));
            addCombIndex(comboBox9.Text, 9);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addCombIndex(comboBox8.Text, 8);
            pictureBox8.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addCombIndex(comboBox7.Text, 7);
            pictureBox7.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            addCombIndex(comboBox6.Text, 6);
            pictureBox6.Image = getImageFrom(string.Format("http://www.dota2.com.cn/images/heroes/{0}_hphover.png", Ch2En[((ComboBox)sender).Text]));            
        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("输入敌方及友方英雄后，程序会根据英雄的克制与配合指数在表格中给出选择建议。（英雄按照顺时针编号，可以不用细究哈）\n\n小提示：\n1. 可以输入英雄首汉字后按向下键自动完成输入；\n2. 可以ctrl + c复制右侧的推荐英雄名完成输入；\n3. 推荐将DOTA2设置成无边框的窗口模式，方便置顶输入，避免TAB来回切换。\n\n目前做的比较粗糙，如有什么BUG或者建议请联系 QQ：49946392 THX：）");
        }

    }
}
