using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIcw
{
    public partial class ViewStock : Form
    {
        public ViewStock()
        {
            InitializeComponent();
        }

        private void LoadStockData()
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT [code], [name], [price], [qty] FROM [dbo].[tblstocks]";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable stockTable = new DataTable();
                    adapter.Fill(stockTable);
                    dgvstocks.DataSource = stockTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading stock data: " + ex.Message);
                }
            }
        }

            private void button1_Click(object sender, EventArgs e)
        {
            LoadStockData();
        }
    }
}
