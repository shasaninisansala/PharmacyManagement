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
using CrystalDecisions.CrystalReports.Engine;


namespace GUIcw
{
    public partial class StockViewerForm : Form
    {
        public StockViewerForm()
        {
            InitializeComponent();
        }
        
        private DataTable GetStockData()
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            DataTable dt = new DataTable();

            string query = @"
                            SELECT 
                                code, 
                                name,
                                price,
                                qty
                            FROM 
                                tblstocks";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private void StockViewerForm_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable stockData = GetStockData();  // Fetch updated data from the database

                if (stockData == null || stockData.Rows.Count == 0)
                {
                    MessageBox.Show("No stock data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                StockReport myReport = new StockReport();  
                myReport.SetDataSource(stockData); 

                crystalReportViewer1.ReportSource = myReport;   
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
