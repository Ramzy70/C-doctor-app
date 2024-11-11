using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem
{
    internal class prescription
    {
        public int id, patient_id, doctor_id;
        public string prescription_lieu, patient_name, patient_phone, doctor_name, doctor_phone,doctor_specialty, doctor_email, CabinetName, Cabinet_Address;
        public DateTime prescription_date, patient_dob;

        public prescription(int id,int patient_id,int doctor_id, DateTime prescription_date,string prescription_lieu,string patient_name, DateTime patient_dob,string patient_phone,string doctor_name,string doctor_phone,string doctor_specialty,string doctor_email, string CabinetName, string Cabinet_Address)
        {
            this.id = id;
            this.patient_id = patient_id;
            this.doctor_id = doctor_id;
            this.doctor_email = doctor_email;
            this.doctor_specialty = doctor_specialty;
            this.CabinetName = CabinetName;
            this.patient_dob = patient_dob;
            this.doctor_name = doctor_name;
            this.doctor_phone = doctor_phone;
            this.Cabinet_Address = Cabinet_Address;
            this.patient_name = patient_name;
            this.patient_phone = patient_phone;
            this.prescription_date = prescription_date;
            this.prescription_lieu = prescription_lieu;
        }

        public override string ToString()
        {
            return id.ToString() + " : " + patient_name + " :" + " le " + prescription_date.Date.ToString() + " à " + prescription_lieu;
        }

    }
}
