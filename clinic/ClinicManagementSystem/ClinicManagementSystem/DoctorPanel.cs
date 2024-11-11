using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clinic_Management_System;

namespace ClinicManagementSystem
{
    public partial class DoctorPanel : Form
    {
        int account_id;
        public DoctorPanel(int id)
        {
            InitializeComponent();
            account_id = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            EditProfile editProfile = new EditProfile(account_id);
            editProfile.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            ViewReservations viewReservations = new ViewReservations(account_id);
            viewReservations.ShowDialog();
            Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            PatientsProfiles patientProfiles = new PatientsProfiles(account_id);
            patientProfiles.ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            CreateReservation createReservation = new CreateReservation(account_id);
            createReservation.ShowDialog();
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
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
    }
}
