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

namespace GUIcw
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username=txtname.Text;
            string password=txtpass.Text;

            //connection
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            //command
            string sql = "SELECT * FROM tbluser WHERE username=@username AND password=@password";
            SqlCommand com = new SqlCommand(sql, con);
            com.Parameters.AddWithValue("@username", username);
            com.Parameters.AddWithValue("@password", password);
            SqlDataReader dr = com.ExecuteReader();
            {
                if (dr.HasRows)
                {
                    MessageBox.Show("Login Successful!");
                    Home home = new Home();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
        }

        private void txtpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            txtname.Text = string.Empty;
            txtpass.Text = string.Empty;

            
            txtname.Focus();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
