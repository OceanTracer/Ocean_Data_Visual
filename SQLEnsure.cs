using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data_Visual
{
    public partial class SQLEnsure : Form
    {

        public SQLEnsure()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
            try
            {
                myconn.Open();
                MessageBox.Show("链接成功");
                myconn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:27017/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接
            var database = client.GetDatabase("SST_res"); //数据库名称
            var collection = database.GetCollection<BsonDocument>("2001-01");

            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Eq("Lon", 150.025) & filterBuilder.Eq("Lat", 50.025);
            var result = collection.Find<BsonDocument>(filter).First();
            MessageBox.Show((result.GetValue("Band").ToString())+ "\n"+"连接成功");

        }
    }
}
