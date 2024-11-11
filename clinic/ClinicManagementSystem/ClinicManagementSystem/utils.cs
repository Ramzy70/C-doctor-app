using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;


namespace ClinicManagementSystem
{
    public class utils
    {
        public static string hashPassword(string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            byte[] password_bytes = Encoding.ASCII.GetBytes(password);
            byte[] encrypted_bytes = sha1.ComputeHash(password_bytes);
            return Convert.ToBase64String(encrypted_bytes);
        }

        public static Dictionary<int, string> getSlots()
        {
            Dictionary<int, string> slots = new Dictionary<int, string>();
            slots.Add(1, "Slot 1: From 8:00 AM to 8:30 AM");
            slots.Add(2, "Slot 6: From 8:30 AM to 9:00 AM");
            slots.Add(3, "Slot 7: From 9:00 AM to 9:30 AM");
            slots.Add(4, "Slot 8: From 9:30 AM to 10:00 AM");
            slots.Add(5, "Slot 9: From 10:00 AM to 10:30 AM");
            slots.Add(6, "Slot 10: From 10:30 AM to 11:00 AM");
            slots.Add(7, "Slot 1: From 11:00 AM to 11:30 AM");
            slots.Add(8, "Slot 6: From 11:30 AM to 12:00 AM");
            slots.Add(9, "Slot 7: From 12:00 AM to 12:30 AM");
            slots.Add(10, "Slot 8: From 12:30 AM to 01:00 PM");
            slots.Add(11, "Slot 9: From 01:00 PM to 01:30 PM");
            slots.Add(12, "Slot 10: From 01:30 PM to 02:00 PM");
            return slots;
        }
        public static void createAdmin(string password)
        {
            SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
            SqlCommand command = con.CreateCommand();

            command.CommandText = "INSERT INTO [user] (user_username, user_password) VALUES (@username, @password)";
            command.Parameters.AddWithValue("@username", "admin");
            command.Parameters.AddWithValue("@password", hashPassword(password));

            con.Open();

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }

            con.Close();
        }
    }
}
