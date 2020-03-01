using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Office.Interop.Excel;//Excel
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using System.Reflection;
using System.IO;

namespace Data_Visual
{
    public partial class DataProcess : Form
    {
        public DataProcess()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        MongoClient client = new MongoClient("mongodb://localhost");
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == ""||textBox3.Text.Trim()=="")
            {
                MessageBox.Show("请完整输入数据");
            }
            else if (Convert.ToInt32(textBox1.Text) > 180 || Convert.ToInt32(textBox1.Text) < -180 || Convert.ToInt32(textBox2.Text) > 90 || Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入的经纬度有误");
            }
            else if(Convert.ToDouble(textBox3.Text)<272.15|| Convert.ToDouble(textBox3.Text)>322.15)
            {
                MessageBox.Show("输入的温度疑似有误，请检查");
            }
            else
            {
                var database = client.GetDatabase("SST_res"); //数据库名称
                string ctname = dateTimePicker1.Text;
                var collection = database.GetCollection<SST>(ctname);
                var filterBuilder = Builders<SST>.Filter;
                var filter = filterBuilder.Eq("Lon", Convert.ToDouble(textBox1.Text) + 0.025) & filterBuilder.Eq("Lat", Convert.ToDouble(textBox2.Text) + 0.025);
                var list = collection.Find<SST>(filter).ToList();
                if (list.Count == 0)
                {
                    MessageBox.Show("未查询到该条记录，将进行插入");
                    SST s = new SST();
                    s.Lon = Convert.ToDouble(textBox1.Text)+0.025;
                    s.Lat = Convert.ToDouble(textBox2.Text)+0.025;
                    s.Band = Convert.ToDouble(textBox3.Text);
                    collection.InsertOne(s);
                    MessageBox.Show("插入成功");
                }
                else
                {
                    MessageBox.Show("查询到已有记录，将进行修改");
                    var update = Builders<SST>.Update.Set("Band", Convert.ToDouble(textBox3.Text));
                    var result = collection.UpdateOne(filter, update);
                    MessageBox.Show("修改成功");
                }
            }
            
        }

        public class SST
        {
            public ObjectId _id { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }
            public double Band { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("请完整输入数据");
            }
            else if (Convert.ToInt32(textBox1.Text) > 180 || Convert.ToInt32(textBox1.Text) < -180 || Convert.ToInt32(textBox2.Text) > 90 || Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入的经纬度有误");
            }
            else
            {
                var database = client.GetDatabase("SST_res"); //数据库名称
                string ctname = dateTimePicker1.Text;
                var collection = database.GetCollection<SST>(ctname);
                var filterBuilder = Builders<SST>.Filter;
                var filter = filterBuilder.Eq("Lon", Convert.ToDouble(textBox1.Text) + 0.025) & filterBuilder.Eq("Lat", Convert.ToDouble(textBox2.Text) + 0.025);
                var list = collection.Find<SST>(filter).ToList();
                if (list.Count == 0)
                {
                    MessageBox.Show("未查询到该条记录，无法删除");
                }
                else
                {
                    MessageBox.Show("查询到已有记录，将进行删除");
                    var result = collection.DeleteOne(filter);
                    MessageBox.Show("删除成功");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
        //public async Task<bool> CollectionExistsAsync(string collectionName)
        //{
        //    var filter = new BsonDocument("name", collectionName);
        //    //filter by collection name
        //    var collections = await client.GetDatabase("SST_res").ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
        //    //check for existence
        //    return await collections.AnyAsync();
        //}
        public static object[,] GetExcelRangeData(string excelPath, string stCell, string edCell)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            object oMissiong = Missing.Value;
            try
            {
                workBook = app.Workbooks.Open(excelPath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                    oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                if (workBook == null)
                    return null;

                Worksheet workSheet = (Worksheet)workBook.Worksheets.Item[1];
                //使用下述语句可以从头读取到最后，按需使用
                //var maxN = workSheet.Range[startCell].End[XlDirection.xlDown].Row;
                return workSheet.Range[stCell + ":" + edCell].Value2;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                //COM组件方式调用完记得释放资源
                if (workBook != null)
                {
                    workBook.Close(false, oMissiong, oMissiong);
                    Marshal.ReleaseComObject(workBook);
                    app.Workbooks.Close();
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
            }
        }
    }
}
