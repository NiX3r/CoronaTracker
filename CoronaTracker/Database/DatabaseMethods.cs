using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Utils;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database
{
    class DatabaseMethods
    {

        private static MySqlConnection connection;

        public static void SetupDatabase()
        {

            connection = DatabaseSecret.GetConnection();
            try
            {
                connection.Open();
                Debug.WriteLine("MySQL connection opened successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySQL connection opened unsuccessfully\nError: " + ex.Message);

            }

        }

        private static void LogLogIn(int GUser_ID, bool logIn)
        {
            var macAddr =
                (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();

            var command = new MySqlCommand("INSERT INTO LoginAttempts(LoginAttempts_DateTime, LoginAttempts_MAC, LoginAttempts_IP, LoginAttempts_IsSuccess, Employee_Employee_ID) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + macAddr + "', '" + externalIpString + "', " + logIn + ", " + GUser_ID + ");", connection);
            command.ExecuteNonQuery();

        }

        public static bool AddUser(String fullname, String email, int phone, String password)
        {

            var command = new MySqlCommand("SELECT Employee_ID FROM Employee WHERE Employee_Email='" + email + "';", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"INSERT INTO Employee(Employee_Fullname, Employee_Email, Employee_Phone, Employee_Pose, Employee_Password, Employee_Created) VALUES('{fullname}', '{email}', {phone}, 'user', '{password}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static String CheckVersion()
        {
            var command = new MySqlCommand("SELECT ProgramData_Value FROM ProgramData WHERE ProgramData_Key='version';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (reader.GetString(0).Equals(ProgramVariables.Version))
            {
                reader.Close();
                return "";
            }
            else
            {
                String output;
                reader.Close();
                command = new MySqlCommand("SELECT ProgramData_Value FROM ProgramData WHERE ProgramData_Key='download_link';", connection);
                reader = command.ExecuteReader();
                reader.Read();
                output = reader.GetString(0);
                reader.Close();
                return output;
            }
        }

        public static int LogIn(String email, String password)
        {
            
            var command = new MySqlCommand("SELECT Employee_ID FROM Employee WHERE Employee_Email='" + email + "';", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                // SELECT COUNT(LoginAttempts_ID) FROM LoginAttempts WHERE LoginAttempts_DateTime>DATE_SUB(NOW(), INTERVAL 1 HOUR) AND LoginAttempts_IsSuccess=false;
                int ID = reader.GetInt32(0);
                reader.Close();
                command = new MySqlCommand($"SELECT COUNT(LoginAttempts_ID) FROM LoginAttempts WHERE LoginAttempts_DateTime>DATE_SUB(NOW(), INTERVAL 1 HOUR) AND LoginAttempts_IsSuccess=FALSE AND Employee_Employee_ID={ID};", connection);
                reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) < 3)

                {
                    reader.Close();
                    command = new MySqlCommand("SELECT Employee_ID,Employee_Fullname,Employee_ProfileURL FROM Employee WHERE Employee_Email='" + email + "' AND Employee_Password='" + password + "';", connection);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        ProgramVariables.ID = reader.GetInt32(0);
                        ProgramVariables.Fullname = reader.GetString(1);
                        ProgramVariables.ProfileURL = reader.GetString(2);
                        reader.Close();
                        LogLogIn(ID, true);
                        return 1;
                    }
                    else
                    {
                        reader.Close();
                        LogLogIn(ID, false);
                        return -1;
                    }
                }
                else
                {
                    reader.Close();
                    return -2;
                }
            }
            else
            {
                reader.Close();
                return 0;
            }

        }

    }
}
