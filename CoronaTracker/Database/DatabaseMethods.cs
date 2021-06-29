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
using System.Windows;

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

        public static bool IsPatientExist(int PatientID, int PatientFirstPersonalNumber, int PatientSecondPersonalNumber)
        {
            var command = new MySqlCommand($"SELECT Patient.Patient_ID FROM Patient WHERE Patient.Patient_ID={PatientID} AND Patient.Patient_PersonalNumberFirst={PatientFirstPersonalNumber} AND Patient.Patient_PersonalNumberSecond={PatientSecondPersonalNumber};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        public static bool IsPatientVaccinate(int PatientID)
        {
            var command = new MySqlCommand($"SELECT VaccineAction.VaccineAction_ID FROM VaccineAction WHERE VaccineAction.Patient_Patient_ID={PatientID};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        public static int GetPatientsCount()
        {
            int output = 0;
            var command = new MySqlCommand("SELECT COUNT(Patient.Patient_ID) FROM Patient;", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetInt32(0);
            reader.Close();
            return output;
        }

        public static int GetVacinnatePatientsCount()
        {
            int output = 0;
            var command = new MySqlCommand("SELECT COUNT(VaccineAction.VaccineAction_ID) FROM VaccineAction;", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetInt32(0);
            reader.Close();
            return output;
        }

        public static int GetInfectionCount()
        {
            int output = 0;
            var command = new MySqlCommand("SELECT COUNT(Infection.Infection_ID) FROM Infection;", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetInt32(0);
            reader.Close();
            return output;
        }

        public static int GetConfirmedPatientsCount()
        {
            int output = 0;
            var command = new MySqlCommand($"SELECT COUNT(Infection.Infection_ID) FROM Infection WHERE Infection.Infection_Found > '{DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss")}' AND Infection.Infection_Found < '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetInt32(0);
            reader.Close();
            return output;
        }

        public static int GetPatientIDByPersonalNumber(string first, string second)
        {
            int output = 0;
            var command = new MySqlCommand($"SELECT Patient.Patient_ID FROM Patient WHERE Patient.Patient_PersonalNumberFirst='{first}' AND Patient.Patient_PersonalNumberSecond='{second}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetInt32(0);
            reader.Close();
            return output;
        }

        public static string GetVaccineTypeString(int id)
        {
            string output = null;
            var command = new MySqlCommand($"SELECT VaccineType.VaccineType_Name FROM VaccineType WHERE VaccineType.VaccineType_ID={id};", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            output = reader.GetString(0);
            reader.Close();
            return output;
        }

        public static string GetEmployeeString(int id)
        {
            string output = "";
            var command = new MySqlCommand($"SELECT Employee.Employee_Fullname FROM Employee WHERE Employee.Employee_ID={id};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
                output = reader.GetString(0);
            reader.Close();
            return output;
        }

        public static PatientVaccineAction GetPatientVaccine(int patientID)
        {
            PatientVaccineAction output;
            var command = new MySqlCommand($"SELECT * FROM VaccineAction WHERE VaccineAction.Patient_Patient_ID={patientID};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                output = new PatientVaccineAction(reader.GetDateTime(1), reader.GetDateTime(2), reader.GetInt32(3), "", reader.GetInt32(5), "");
                reader.Close();
                output.VaccineTypeString = GetVaccineTypeString(output.VaccineType);
                output.EmployeeString = GetEmployeeString(output.Employee);
                return output;
            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public static PatientInstance GetPatient(int id)
        {
            PatientInstance output;
            var command = new MySqlCommand($"SELECT * FROM Patient WHERE Patient.Patient_ID={id};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                output = new PatientInstance(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7));
                reader.Close();
                return output;
            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public static VaccinateInstance GetVaccinate(int patientId)
        {
            VaccinateInstance output;
            var command = new MySqlCommand($"SELECT * FROM VaccineAction WHERE Patient_Patient_ID={patientId};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                output = new VaccinateInstance(reader.GetDateTime(1), reader.GetDateTime(2), reader.GetString(3));
                reader.Close();
                return output;
            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public static VaccineTypeInstance GetVaccineType(string name)
        {
            VaccineTypeInstance output;
            var command = new MySqlCommand($"SELECT * FROM VaccineType WHERE VaccineType.VaccineType_Name='{name}';", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                output = new VaccineTypeInstance(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                reader.Close();
                return output;
            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public static List<VaccineTypeInstance> GetVaccineTypes()
        {
            List<VaccineTypeInstance> output = new List<VaccineTypeInstance>();
            var command = new MySqlCommand("SELECT * FROM VaccineType;", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(new VaccineTypeInstance(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            return output;
        }

        public static List<PatientInstance> GetPatients()
        {
            List<PatientInstance> output = new List<PatientInstance>();
            var command = new MySqlCommand("SELECT * FROM Patient;", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(new PatientInstance(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7)));
            }
            reader.Close();
            return output;
        }

        public static List<FindsInstance> GetFinds(String firstCode, String secondCode)
        {
            List<FindsInstance> output = new List<FindsInstance>();
            var command = new MySqlCommand($"SELECT Infection.Infection_ID, Infection.Infection_Found, Employee.Employee_Fullname, Patient.Patient_PersonalNumberFirst, Patient.Patient_PersonalNumberSecond FROM ((Infection INNER JOIN Employee ON Infection.Employee_Employee_ID = Employee.Employee_ID) INNER JOIN Patient ON Infection.Patient_Patient_ID = Patient.Patient_ID) WHERE Patient.Patient_PersonalNumberFirst='{firstCode}' AND Patient.Patient_PersonalNumberSecond='{secondCode}';", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(new FindsInstance(reader.GetInt32(0), reader.GetDateTime(1), reader.GetString(2)));
            }
            reader.Close();
            return output;
        }

        public static List<int> GetInfections()
        {
            List<int> output = new List<int>();

            DateTime dt = DateTime.Now.AddMonths(-5);
            DateTime now = DateTime.Now;

            while (true)
            {
                var command = new MySqlCommand($"SELECT COUNT(Infection.Infection_Found) FROM Infection WHERE Infection.Infection_Found > '{dt.ToString("yyyy-MM-dd HH:mm:ss")}' AND Infection.Infection_Found < '{dt.AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss")}';", connection);
                var reader = command.ExecuteReader();
                reader.Read();
                output.Add(reader.GetInt32(0));
                reader.Close();

                dt = dt.AddMonths(1);
                if (DateTime.Compare(dt, now) > 0)
                {
                    break;
                }
            }

            return output;
        }

        public static bool AddPatientVaccine(DateTime first, DateTime second, int type, int patient, int employee)
        {
            string query = $"INSERT INTO VaccineAction(VaccineAction_FirstDate, VaccineAction_SecondDate, VaccineType_VaccineType_ID, Patient_Patient_ID, Employee_Employee_ID) VALUES('{first.ToString("yyyy-MM-dd HH:mm:ss")}', '{second.ToString("yyyy-MM-dd HH:mm:ss")}', {type}, {patient}, {employee});";
            var command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
            return true;
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

        public static bool AddFinds(String firstCode, String secondCode)
        {
            int id;
            var command = new MySqlCommand("SELECT Patient_ID FROM Patient WHERE Patient_PersonalNumberFirst='" + firstCode + "' AND Patient_PersonalNumberSecond='" + secondCode + "';", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            id = reader.GetInt32(0);
            reader.Close();
            command = new MySqlCommand($"INSERT INTO Infection(Infection_Found, Patient_Patient_ID, Employee_Employee_ID) VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {id}, {ProgramVariables.ID});", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool AddPatient(String personNumberFirst, String personNumberSecond, String fullname, String email, int phone, int insurance, String description = "")
        {
            var command = new MySqlCommand("SELECT Patient_ID FROM Patient WHERE Patient_PersonalNumberFirst='" + personNumberFirst + "' AND Patient_PersonalNumberSecond='" + personNumberSecond + "';", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"INSERT INTO Patient(Patient_PersonalNumberFirst, Patient_PersonalNumberSecond, Patient_Fullname, Patient_Email, Patient_Phone, Patient_InsuranceCode, Patient_Description, Patient_Created) VALUES('{personNumberFirst}', '{personNumberSecond}', '{fullname}', '{email}', {phone}, {insurance}, '{description}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool AddVaccineType(String name, String description = "")
        {
            var command = new MySqlCommand("SELECT VaccineType_ID FROM VaccineType WHERE VaccineType_Name='" + name + "';", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"INSERT INTO VaccineType(VaccineType_Name, VaccineType_Description) VALUES('{name}', '{description}');", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool RemoveVaccineType(String name)
        {
            var command = new MySqlCommand("SELECT VaccineType_ID FROM VaccineType WHERE VaccineType_Name='" + name + "';", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"DELETE FROM VaccineType WHERE VaccineType_Name='{name}';", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool RemoveFinds(int ID)
        {
            var command = new MySqlCommand("SELECT Infection_ID FROM Infection WHERE Infection_ID=" + ID + ";", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"DELETE FROM Infection WHERE Infection_ID={ID};", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool RemovePatient(String personNumberFirst, String personNumberSecond)
        {
            var command = new MySqlCommand($"SELECT Patient_ID FROM Patient WHERE Patient_PersonalNumberFirst='{personNumberFirst}' AND Patient_PersonalNumberSecond='{personNumberSecond}';", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"DELETE FROM Patient WHERE Patient_PersonalNumberFirst='{personNumberFirst}' AND Patient_PersonalNumberSecond='{personNumberSecond}';", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool EditVaccineType(String name, String description = "")
        {
            var command = new MySqlCommand("SELECT VaccineType_ID FROM VaccineType WHERE VaccineType_Name='" + name + "';", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"UPDATE VaccineType SET VaccineType_Description='{description}' WHERE VaccineType_Name='{name}';", connection);
            command.ExecuteNonQuery();
            return true;
        }

        public static bool EditPatient(String personalNumberFirst, String personalNumberSecond, String fullname, String email, int phone, int insurance, String description = "")
        {
            var command = new MySqlCommand("SELECT Patient_ID FROM Patient WHERE Patient_PersonalNumberFirst='" + personalNumberFirst + "' AND Patient_PersonalNumberSecond='" + personalNumberSecond + "';", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"UPDATE Patient SET Patient_Fullname='{fullname}', Patient_Email='{email}', Patient_Phone={phone}, Patient_InsuranceCode={insurance}, Patient_Description='{description}' WHERE Patient_PersonalNumberFirst='{personalNumberFirst}' AND Patient_PersonalNumberSecond='{personalNumberSecond}';", connection);
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
                int first, second;
                first = second = -1;
                int ID = reader.GetInt32(0);
                reader.Close();
                command = new MySqlCommand($"SELECT LoginAttempts_ID FROM LoginAttempts WHERE LoginAttempts_DateTime>DATE_SUB(NOW(), INTERVAL 1 HOUR) AND LoginAttempts_IsSuccess=FALSE AND Employee_Employee_ID={ID};", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if(first == -1)
                    {
                        first = reader.GetInt32(0);
                    }
                    else if(second == -1)
                    {
                        second = reader.GetInt32(0);
                    }
                    else
                    {
                        int third = reader.GetInt32(0);
                        if(((first + 2) == third) && ((second + 1) == third))
                        {
                            reader.Close();
                            return -2;
                        }
                        else
                        {
                            first = second;
                            second = -1;
                        }
                    }
                }
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
                return 0;
            }

        }

    }
}
