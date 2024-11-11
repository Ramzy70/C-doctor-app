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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClinicManagementSystem
{
    public partial class AdminPanel : Form
    {
        public AdminPanel()
        {
            InitializeComponent();

            // Make text boxes read-only
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox9.ReadOnly = true;
            textBox10.ReadOnly = true;
            textBox11.ReadOnly = true;
            textBox12.ReadOnly = true;

        }

        private void updateList(string query)
        {
            SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
            SqlCommand command = con.CreateCommand();
            con.Open();
            command.CommandText = "SELECT account_id, account_name, account_type FROM account WHERE account_type in (0, 1) AND (account_name LIKE @query OR account_phone LIKE @query) ORDER BY account_type";
            command.Parameters.AddWithValue("@query", query + "%");

            SqlDataReader reader = command.ExecuteReader();

            listBox1.Items.Clear();
            while (reader.Read())
                listBox1.Items.Add(new account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));

            con.Close();
        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {
            updateList("");
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            updateList(textBox6.Text);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int account_id;
            try
            {
                account_id = ((account)listBox1.SelectedItem).getId();
            } catch(Exception) {
                return;
            }
            SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
            SqlCommand command = con.CreateCommand();
            command.CommandText = "SELECT user_username, account_name, account_dob, account_phone, account_type, account_note, account_creation_date FROM [user], account WHERE user_id = account_user_id AND account_id=@id";
            command.Parameters.AddWithValue("@id", account_id);
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                textBox4.Text = account_id.ToString();
                textBox5.Text = reader.GetValue(0).ToString();
                textBox7.Text = reader.GetValue(1).ToString();
                textBox8.Text = reader.GetValue(2).ToString();
                textBox9.Text = reader.GetValue(3).ToString();

                if (reader.GetInt32(4) == 0)
                    textBox10.Text = "Secretary";
                else
                    textBox10.Text = "Doctor";

                textBox11.Text = reader.GetValue(5).ToString();
                textBox12.Text = reader.GetValue(6).ToString();
            }

            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!validateInputs())
            {
                MessageBox.Show("Please check the input fields again!");
                return;
            }

            SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
            SqlCommand command = con.CreateCommand();
            command.CommandText = "INSERT INTO [user] (user_username, user_password) VALUES(@username, @password)";
            command.Parameters.AddWithValue("@username", textBox1.Text);
            command.Parameters.AddWithValue("@password", utils.hashPassword(textBox2.Text));
            con.Open();
            if (command.ExecuteNonQuery() > 0)
            {
                //We created the record in user table
                command.CommandText = "SELECT user_id FROM [user] WHERE user_username = @username";
                int user_id = (int)command.ExecuteScalar();

                command.CommandText = "INSERT INTO account (account_user_id, account_name, account_type, account_note, account_creation_date) VALUES (@user_id, @name, @type, @notes, @date)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@user_id", user_id);
                command.Parameters.AddWithValue("@name", textBox3.Text);
                command.Parameters.AddWithValue("@type", comboBox1.SelectedIndex);
                command.Parameters.AddWithValue("@notes", textBox13.Text);
                command.Parameters.AddWithValue("@date", DateTime.Now);

                if (command.ExecuteNonQuery() > 0)
                {
                    //All good => Account created!
                    MessageBox.Show("Account was successfully created!");
                }
                else
                    MessageBox.Show("Error while creating the account!");
            }
            else
                MessageBox.Show("Error while creating the account!");
            con.Close();
            updateList("");
        }

        private bool validateInputs()
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                return false;

            if (comboBox1.SelectedIndex < 0)
                return false;

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "")
            {
                MessageBox.Show("There is no selected user!");
                return;
            }

            SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
            SqlCommand command = con.CreateCommand();
            command.CommandText = "DELETE FROM [user] WHERE user_username=@username";
            command.Parameters.AddWithValue("@username", textBox5.Text);
            con.Open();
            if (command.ExecuteNonQuery() > 0)
                MessageBox.Show("Account was deleted!");
            else
                MessageBox.Show("Account was not deleted!");
            con.Close();
            updateList("");
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
        }
    }
}
