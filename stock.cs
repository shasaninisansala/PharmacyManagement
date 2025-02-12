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
    public partial class stock : Form
    {
        public stock()
        {
            InitializeComponent();
        }

        private void stock_Load(object sender, EventArgs e)
        {

        }

        private void ClearFields()
        {
            txtcode.Clear();
            txtiname.Clear();
            txtprice.Clear();
            txtqty.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtcode.Text) ||
                string.IsNullOrWhiteSpace(txtiname.Text) ||
                string.IsNullOrWhiteSpace(txtprice.Text) ||
                string.IsNullOrWhiteSpace(txtqty.Text))
            {
                MessageBox.Show("Please fill in all the fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtprice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtqty.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            //connection
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            SqlConnection con = new SqlConnection(cs);
            con.Open();


            //command
            string sql = "INSERT INTO tblstocks(code,name,price,qty) VALUES (@code,@name,@price,@qty)";
            SqlCommand com = new SqlCommand(sql, con);
            com.Parameters.AddWithValue("@code", this.txtcode.Text);
            com.Parameters.AddWithValue("@name", this.txtiname.Text);
            com.Parameters.AddWithValue("@price", this.txtprice.Text);
            com.Parameters.AddWithValue("@qty", this.txtqty.Text);

            //Execute
            int ret = com.ExecuteNonQuery();
            MessageBox.Show("No of records inserted:" + ret, "Information");

            ClearFields();

            //Disconnect
            con.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Create a connection with MS SQL Server
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            //Define a Command with SQL Statement
            string sql = "SELECT * FROM tblstocks";
            SqlCommand com = new SqlCommand(sql, con);

            SqlDataAdapter dap = new SqlDataAdapter(sql, con);
            DataSet ds = new DataSet();
            dap.Fill(ds);

            /* this.txtcode.Text = ds.Tables[0].Rows[0][0].ToString();
             this.txtiname.Text = ds.Tables[0].Rows[0][1].ToString();
             this.txtprice.Text = ds.Tables[0].Rows[0][2].ToString();
             this.txtqty.Text = ds.Tables[0].Rows[0][3].ToString();*/
             

            this.dataGridView1.DataSource = ds.Tables[0];
            con.Close();
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {

            // Check if any of the fields are empty
            if (string.IsNullOrWhiteSpace(txtcode.Text) ||
                string.IsNullOrWhiteSpace(txtiname.Text) ||
                string.IsNullOrWhiteSpace(txtprice.Text) ||
                string.IsNullOrWhiteSpace(txtqty.Text))
            {
                MessageBox.Show("Please fill in all the fields before updating.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            SqlConnection con = new SqlConnection(cs);

            try
            {
                con.Open();

                string Sql1 = "SELECT qty FROM tblstocks WHERE code = @code";
                SqlCommand selectCmd = new SqlCommand(Sql1, con);
                selectCmd.Parameters.AddWithValue("@code", this.txtcode.Text);

                object result = selectCmd.ExecuteScalar(); // Get the current qty
                if (result == null)
                {
                    MessageBox.Show("Item not found.", "Error");
                    return;
                }

                int currentQty = Convert.ToInt32(result);

                // Add the entered qty to the current qty
                int enteredQty = int.Parse(this.txtqty.Text);
                int updatedQty = chkAddQty.Checked ? currentQty + enteredQty : enteredQty;
                // Define a command
                string sql = "UPDATE tblstocks SET name=@name, price=@price, qty=@qty WHERE code=@code";
                SqlCommand com = new SqlCommand(sql, con);

                // Add parameters to prevent SQL injection
                com.Parameters.AddWithValue("@code", this.txtcode.Text);
                com.Parameters.AddWithValue("@name", this.txtiname.Text);
                com.Parameters.AddWithValue("@price", decimal.Parse(this.txtprice.Text));
                com.Parameters.AddWithValue("@qty", updatedQty);

                // Execute the command
                int ret = com.ExecuteNonQuery();
                MessageBox.Show("No of records Updated: " + ret, "Information");

                // Refresh the DataGridView
                RefreshDataGridView();
                ClearFields();
                txtSearch.Clear();
                chkAddQty.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error");
            }
            finally
            {
                // Disconnect from SQL Server
                con.Close();
            }
        }

        void RefreshDataGridView()
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            string refreshSql = "SELECT * FROM tblstocks";
            SqlDataAdapter da = new SqlDataAdapter(refreshSql, cs);
            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to refresh DataGridView: " + ex.Message, "Error");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            string itemCode = txtSearch.Text;

            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT * FROM tblstocks WHERE code = @ItemCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtcode.Text = reader["code"].ToString();
                    txtiname.Text = reader["name"].ToString();
                    txtprice.Text = reader["price"].ToString();
                    txtqty.Text = reader["qty"].ToString();
                }
                else
                {
                    MessageBox.Show("Item not found.");
                }
                conn.Close();
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            string itemCode = txtSearch.Text;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "DELETE FROM tblstocks WHERE code = @ItemCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Item deleted successfully.");
                    RefreshDataGridView();
                    ClearFields();
                    txtSearch.Clear();
                }
                else
                {
                    MessageBox.Show("Error deleting item.");
                }
            }
        }

        private void button1_Click_3(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear all text fields
            ClearFields();
            chkAddQty.Checked = false;
            txtSearch.Clear();
        }
    }
}
