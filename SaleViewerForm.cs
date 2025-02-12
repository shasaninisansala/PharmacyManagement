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
    public partial class SaleViewerForm : Form
    {
        public SaleViewerForm()
        {
            InitializeComponent();
        }

        private DataTable GetSalesReportData()
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            DataTable dt = new DataTable();

                    string query = @"
                SELECT 
                    tblbill.bill_id,
                    tblbill.bill_date,
                    tblbill.cus_Name,
                    tblbill.net_Amount,
                    COUNT(tblbill_item.billitem_id) AS TotalItems
                FROM 
                    tblbill
                INNER JOIN 
                    tblbill_item ON tblbill.bill_id = tblbill_item.bill_id
                GROUP BY 
                    tblbill.bill_id, tblbill.bill_date, tblbill.cus_Name, tblbill.net_Amount";

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

        private void SaleViewerForm_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable salesData = GetSalesReportData(); 

                if (salesData == null || salesData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SalesReport myReport = new SalesReport(); 
                myReport.SetDataSource(salesData); 

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
