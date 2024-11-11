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
using Guna.UI2.WinForms;
using Clinic_Management_System;

namespace ClinicManagementSystem
{
    public partial class EditProfile : Form
    {
        int account_id , account_type;
        public EditProfile(int account_id)
        {
            InitializeComponent();
            this.account_id = account_id;
            SqlCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = "SELECT account_id , account_type FROM account WHERE account_user_id = @user_id";
            cmd.Parameters.AddWithValue("@user_id", this.account_id);
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    this.account_type = reader.GetInt32(1);
                    con.Close();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                con.Close(); // Ensure the connection is closed
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);


        private void EditProfile_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT user_username , account_name , account_dob , account_phone , account_type , account_note , account_creation_date , doctor_specialty , doctor_email , CabinetName , Cabinet_Address FROM [user] , account WHERE account_user_id = user_id AND account_id = @account_id";
            cmd.Parameters.AddWithValue("@account_id", account_id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                textBox4.Text = account_id.ToString();
                textBox5.Text = reader.GetValue(0).ToString();
                textBox7.Text = reader.GetValue(1).ToString();
                try
                {
                    dateTimePicker1.Value = DateTime.Parse(reader.GetValue(2).ToString());
                }
                catch (Exception)
                {

                }
                textBox9.Text = reader.GetValue(3).ToString();
                if (reader.GetInt32(4) == 0) { 
                    textBox10.Text = "Secretary";
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox6.Enabled = false;
                }
                else if (reader.GetInt32(4) == 1)
                {
                    textBox10.Text = "Doctor";
                    textBox1.Text = reader.GetValue(8).ToString();
                    textBox2.Text = reader.GetValue(7).ToString();
                    textBox3.Text = reader.GetValue(9).ToString();
                    textBox6.Text = reader.GetValue(10).ToString();
                }
                else if (reader.GetInt32(4) == 2)
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox6.Enabled = false;
                    textBox10.Text = "Patient";
                }
                textBox11.Text = reader.GetValue(5).ToString();
                textBox12.Text = reader.GetValue(6).ToString();

            }
            con.Close();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT account_type FROM account WHERE account_id = @id";
            cmd.Parameters.AddWithValue("@id", account_id);
            con.Open();
            account_type = (int)cmd.ExecuteScalar();
            con.Close();
            if (account_type == 1)
            {

            }
            else
            {
                button9.Visible = false; // Make button1 invisible
                button6.Visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Display a confirmation message box
            DialogResult result = MessageBox.Show("Voulez-vous vraiment fermer l'application ?",
                "Confirmation de fermeture", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user confirms, close the application
            if (result == DialogResult.Yes)
            {
                // Close the main form, which will close the application
                Application.Exit();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            if (textBox7.Text == "")
            {
                MessageBox.Show("Please enter a name!");
                return;
            }


            SqlCommand command = con.CreateCommand();
            command.CommandText = "UPDATE account SET account_name = @name, account_dob = @dob, account_note = @notes, account_phone = @phone , doctor_specialty=@specialty , doctor_email = @email , CabinetName = @Cabinet , Cabinet_Address = @address WHERE account_id = @account_id";
            command.Parameters.AddWithValue("@name", textBox7.Text);
            command.Parameters.AddWithValue("@dob", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@phone", textBox9.Text);
            command.Parameters.AddWithValue("@notes", textBox11.Text);
            command.Parameters.AddWithValue("@account_id", account_id);
            command.Parameters.AddWithValue("@email", textBox2.Text);
            command.Parameters.AddWithValue("@address", textBox6.Text);
            command.Parameters.AddWithValue("@specialty", textBox1.Text);
            command.Parameters.AddWithValue("@Cabinet", textBox3.Text);

            con.Open();
            if (command.ExecuteNonQuery() > 0)
                MessageBox.Show("Account was updated!");
            else
                MessageBox.Show("Account was not updated!");
            con.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            PatientsProfiles patientProfiles = new PatientsProfiles(this.account_id);
            patientProfiles.ShowDialog();
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
            CreateReservation createReservation = new CreateReservation(account_id);
            createReservation.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            ViewReservations viewReservations = new ViewReservations(account_id);
            viewReservations.ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Hide();
            Medicaments medicament = new Medicaments(account_id);
            medicament.ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            Prescriptions pres = new Prescriptions(account_id);
            pres.ShowDialog();
            Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.account_type == 0)
            {
                //Secratery Pan
                Hide();
                SecretaryPanel secretaryPanel = new SecretaryPanel(this.account_id);
                secretaryPanel.ShowDialog();
                Show();
            }
            else if (this.account_type == 1)
            {
                Hide();
                DoctorPanel doctorPanel = new DoctorPanel(this.account_id);
                doctorPanel.ShowDialog();
                Show();
            }
        }
    }
}
