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

        // Instance of MySQL connection
        // Have to be in instance to hide it from GitHub
        private static MySqlConnection connection;

        /// <summary>
        /// Function to prepare database connection
        /// </summary>
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

        public static void RefreshDatabaseConnection()
        {
            connection.Clone();
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

        /// <summary>
        /// Function to log attempt to log in
        /// </summary>
        /// <param name="GUser_ID"> variable for user id </param>
        /// <param name="logIn"> variable for log in statement </param>
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

        /// <summary>
        /// Function to get url address of lastest link to download
        /// </summary>
        /// <returns>
        /// return string for lastest url
        /// </returns>
        public static string GetLinkToLastestVersion()
        {
            var command = new MySqlCommand($"SELECT ProgramData.ProgramData_Value FROM ProgramData WHERE ProgramData.ProgramData_Key='download_link';", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                string s = reader.GetString(0);
                reader.Close();
                return s;
            }
            reader.Close();
            return null;
        }

        /// <summary>
        /// Function to check if patient exists by his id and personal number
        /// </summary>
        /// <param name="PatientID"> variable for user id </param>
        /// <param name="PatientFirstPersonalNumber"> variable for first part of personal number </param>
        /// <param name="PatientSecondPersonalNumber"> variable for second part of personal number </param>
        /// <returns>
        /// return true: patient exists
        /// return false: patient does not exists
        /// </returns>
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

        /// <summary>
        /// Function to check if patient is vaccinate
        /// </summary>
        /// <param name="PatientID"> variable for patient id </param>
        /// <returns>
        /// return true: patient is vaccinate
        /// return false: patient is not vaccinate
        /// </returns>
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

        /// <summary>
        /// Function to get number of patients
        /// </summary>
        /// <returns>
        /// return number of patients
        /// </returns>
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

        /// <summary>
        /// Function to get number of vaccinate patients
        /// </summary>
        /// <returns>
        /// return number of vaccinate patients
        /// </returns>
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

        /// <summary>
        /// Function to get number of all time infections
        /// </summary>
        /// <returns>
        /// return number of all time infections
        /// </returns>
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

        /// <summary>
        /// Function to get number of todays confirmed patients
        /// </summary>
        /// <returns>
        /// return number of todays confirmed patients
        /// </returns>
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

        /// <summary>
        /// Function to get patient id by his personal number
        /// </summary>
        /// <param name="first"> variable for first part of personal number </param>
        /// <param name="second"> variable for second part of personal number </param>
        /// <returns>
        /// return id of patient
        /// </returns>
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

        /// <summary>
        /// Function to get vaccine name by id
        /// </summary>
        /// <param name="id"> variable for id of vaccine </param>
        /// <returns>
        /// return name of vaccine
        /// </returns>
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

        /// <summary>
        /// Function to get employee name by id
        /// </summary>
        /// <param name="id"> variable for id of employee </param>
        /// <returns>
        /// return name of employeee
        /// </returns>
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

        /// <summary>
        /// Function to get employee role / pose by email
        /// </summary>
        /// <param name="email"> variable for email of employee </param>
        /// <returns>
        /// return role / pose of employee
        /// </returns>
        public static string GetEmployeeRoleByEmail(string email)
        {
            string output = "";
            var command = new MySqlCommand($"SELECT Employee.Employee_Pose FROM Employee WHERE Employee.Employee_Email='{email}';", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
                output = reader.GetString(0);
            reader.Close();
            return output;
        }

        /// <summary>
        /// Function to check if logged employee has permit
        /// to change pose of other employees
        /// </summary>
        /// <returns>
        /// return true: has permit
        /// return false: has not permit
        /// </returns>
        public static bool HasEmployeePermitChangePose()
        {
            bool output = false;
            var command = new MySqlCommand($"SELECT Employee.Employee_Pose FROM Employee WHERE Employee.Employee_ID={ProgramVariables.ID};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
                if (reader.GetString(0).Equals("developer") || reader.GetString(0).Equals("leader"))
                    output = true;
            reader.Close();
            return output;
        }

        /// <summary>
        /// Function to check if computer has auto login
        /// </summary>
        /// <returns>
        /// return 0: has not auto login
        /// return number: id of employee
        /// </returns>
        public static int HasAutoLogin()
        {
            string mac = GetMACAddress();
            string ip = GetIPAddress();

            int output = 0;
            var command = new MySqlCommand($"SELECT Employee_Employee_ID FROM AutoLoginSession WHERE AutoLoginSession_MAC='{mac}' AND AutoLoginSession_IP='{ip}';", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
                output = reader.GetInt16(0);
            reader.Close();
            return output;
        }

        /// <summary>
        /// Function to get employee instance by ID
        /// </summary>
        /// <param name="ID"> variable for ID of employee </param>
        /// <returns>
        /// return null: employee with that ID does not exists
        /// return instance: specific employee
        /// </returns>
        public static EmployeeInstance GetEmployeeByID(int ID)
        {
            EmployeeInstance output = null;
            var command = new MySqlCommand($"SELECT Employee_Fullname,Employee_ProfileURL,Employee_Phone FROM Employee WHERE Employee.Employee_ID={ID};", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
                output = new EmployeeInstance(reader.GetString(0), reader.IsDBNull(1) ? "" : reader.GetString(1), reader.GetInt32(2));
            reader.Close();
            return output;
        }

        /// <summary>
        /// Function to get vaccine action instance by ID
        /// </summary>
        /// <param name="patientID"> variable for ID of vaccine action </param>
        /// <returns>
        /// return null: patient vaccine with that ID does not exists
        /// return instance: specific vaccine action
        /// </returns>
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

        /// <summary>
        /// Function to get patient instance by ID
        /// </summary>
        /// <param name="id"> variable for ID of patient </param>
        /// <returns>
        /// return null: patient with that ID does not exists
        /// return instance: specific patient
        /// </returns>
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

        /// <summary>
        /// Function to get vaccinate instance by patient ID
        /// </summary>
        /// <param name="patientId"> variable for ID of patient</param>
        /// <returns>
        /// return null: vaccinate with that ID does not exists
        /// return instance: specific vaccinate
        /// </returns>
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

        /// <summary>
        /// Function to get vaccinate type instance by name
        /// </summary>
        /// <param name="name"> variable for name of vaccine type </param>
        /// <returns>
        /// return null: vaccine type with that name does not exists
        /// return instance: specific vaccine type
        /// </returns>
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

        /// <summary>
        /// Function to get all vaccinate types
        /// </summary>
        /// <returns>
        /// return list of vaccine types
        /// </returns>
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

        /// <summary>
        /// Function to get all patients
        /// </summary>
        /// <returns>
        /// return list of patients
        /// </returns>
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

        /// <summary>
        /// Function to get all finds of patient by personal number
        /// </summary>
        /// <param name="firstCode"> variable for first part of personal number </param>
        /// <param name="secondCode"> variable for seconds part of personal number </param>
        /// <returns>
        /// return list of finds
        /// </returns>
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

        /// <summary>
        /// Function to get last five months finds number
        /// </summary>
        /// <returns>
        /// return list of last five months finds
        /// </returns>
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

        /// <summary>
        /// Function to add auto login session
        /// </summary>
        /// <returns>
        /// return true: auto login successfully added
        /// return false: auto login session already exists
        /// </returns>
        public static bool AddAutoLoginSession()
        {
            string mac = GetMACAddress();
            string ip = GetIPAddress();

            var command = new MySqlCommand($"SELECT AutoLoginSession_ID FROM AutoLoginSession WHERE AutoLoginSession_MAC='{mac}' AND AutoLoginSession_IP='{ip}';", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            string query = $"INSERT INTO AutoLoginSession(AutoLoginSession_MAC, AutoLoginSession_IP, AutoLoginSession_StartDate, Employee_Employee_ID) VALUES('{mac}', '{ip}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {ProgramVariables.ID});";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Function to get public ip address as string
        /// </summary>
        /// <returns>
        /// return ip address
        /// </returns>
        private static string GetIPAddress()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(externalIpString).ToString();
        }

        /// <summary>
        /// Function to get mac address as string
        /// </summary>
        /// <returns>
        /// return mac address
        /// </returns>
        private static string GetMACAddress()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

        /// <summary>
        /// Function to add patient vaccine
        /// </summary>
        /// <param name="first"> variable for first date of vaccinate </param>
        /// <param name="second"> variable for second date of vaccinate </param>
        /// <param name="type"> variable for vaccine type id </param>
        /// <param name="patient"> variable for patient id </param>
        /// <param name="employee"> variable for employee id </param>
        /// <returns>
        /// return true: patient vaccine successfully added
        /// return false: patient already vaccinated
        /// </returns>
        public static bool AddPatientVaccine(DateTime first, DateTime second, int type, int patient, int employee)
        {
            var command = new MySqlCommand("SELECT VaccineAction_ID FROM VaccineAction WHERE Patient_Patient_ID=" + patient + ";", connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return false;
            }
            reader.Close();
            string query = $"INSERT INTO VaccineAction(VaccineAction_FirstDate, VaccineAction_SecondDate, VaccineType_VaccineType_ID, Patient_Patient_ID, Employee_Employee_ID) VALUES('{first.ToString("yyyy-MM-dd HH:mm:ss")}', '{second.ToString("yyyy-MM-dd HH:mm:ss")}', {type}, {patient}, {employee});";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Function to add user
        /// </summary>
        /// <param name="fullname"> variable for user fullname </param>
        /// <param name="email"> variable for user email </param>
        /// <param name="phone"> variable for user phone </param>
        /// <param name="password"> variable for password </param>
        /// <returns>
        /// return true: user successfully added
        /// return false: user with that email already exists
        /// </returns>
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
            command = new MySqlCommand($"INSERT INTO Employee(Employee_Fullname, Employee_Email, Employee_Phone, Employee_Pose, Employee_Password, Employee_Created) VALUES('{fullname}', '{email}', {phone}, 'guest', '{password}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');", connection);
            command.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Function to add patient find
        /// </summary>
        /// <param name="firstCode"> variable for first part of personal number </param>
        /// <param name="secondCode"> variable for second part of personal number </param>
        /// <returns>
        /// return true: patient find successfully added
        /// return false: patient with that personal code does not exists
        /// </returns>
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

        /// <summary>
        /// Function to add patient
        /// </summary>
        /// <param name="personNumberFirst"> variable for first part of personal number </param>
        /// <param name="personNumberSecond"> variable for second part of personal number </param>
        /// <param name="fullname"> variable for fullname </param>
        /// <param name="email"> variable for email </param>
        /// <param name="phone"> variable for phone </param>
        /// <param name="insurance"> variable for insurance code </param>
        /// <param name="description"> variable for description </param>
        /// <returns>
        /// return true: patient successfully added
        /// return false: patient with that personal number already exists
        /// </returns>
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

        /// <summary>
        /// Function to add vaccine type
        /// </summary>
        /// <param name="name"> variable for name </param>
        /// <param name="description"> variable for description </param>
        /// <returns>
        /// return true: vaccine type successfulle added
        /// return false: vaccine type with that name already exists
        /// </returns>
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

        /// <summary>
        /// Function to remove vaccine type by name
        /// </summary>
        /// <param name="name"> variable for name</param>
        /// <returns>
        /// return true: vaccine type successfully removed
        /// return false: vaccine type with that name does not exists
        /// </returns>
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

        /// <summary>
        /// Function to remove find by ID
        /// </summary>
        /// <param name="ID"> variable for find id</param>
        /// <returns>
        /// return true: find successfully removed
        /// return false: find with that ID does not exists
        /// </returns>
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

        /// <summary>
        /// Function to remove patient by personal number
        /// </summary>
        /// <param name="personNumberFirst"> variable for first part of personal number </param>
        /// <param name="personNumberSecond"> variable for second part of personal number </param>
        /// <returns>
        /// return true: patient successfully removed
        /// return false: patient with that personal number does not exists
        /// </returns>
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

        /// <summary>
        /// Function to edit currently logged employee
        /// </summary>
        /// <param name="profileURL"> variable for profile picture url </param>
        /// <param name="phone"> variable for phone </param>
        /// <returns>
        /// return true: employee successfully edited
        /// return false: emplyoee with that ID does not exists
        /// </returns>
        public static bool EditEmployeeInfo(String profileURL, int phone)
        {
            var command = new MySqlCommand("SELECT Employee_ID FROM Employee WHERE Employee_ID=" + ProgramVariables.ID + ";", connection);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return false;
            }

            reader.Close();
            command = new MySqlCommand($"UPDATE Employee SET Employee_ProfileURL='{profileURL}',Employee_Phone={phone} WHERE Employee_ID={ProgramVariables.ID};", connection);
            command.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Function to edit vaccine type by name
        /// </summary>
        /// <param name="name"> variable for name </param>
        /// <param name="description"> variable for description </param>
        /// <returns>
        /// return true: vaccine type successfully edited
        /// return false: vaccine type with that name does not exists
        /// </returns>
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

        /// <summary>
        /// Function to edit patient by personal number
        /// </summary>
        /// <param name="personalNumberFirst"> variable for first part of personal number </param>
        /// <param name="personalNumberSecond"> variable for second part of personal number </param>
        /// <param name="fullname"> variable for fullname </param>
        /// <param name="email"> variable for email </param>
        /// <param name="phone"> variable for phone </param>
        /// <param name="insurance"> variable for insurance code </param>
        /// <param name="description"> variable for description </param>
        /// <returns>
        /// return true: patient successfully edited
        /// return false: patient with that personal number does not exists
        /// </returns>
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

        /// <summary>
        /// Function to check version and possible return url with lastest release
        /// </summary>
        /// <returns>
        /// return "": version matches with lastest version
        /// return string: lastest url release
        /// </returns>
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

        /// <summary>
        /// Function to update employee pose by email
        /// </summary>
        /// <param name="email"> variable for email </param>
        /// <param name="pose"> variable for pose </param>
        /// <returns>
        /// return 1: employee pose successfully updated
        /// return 2: employee with that email does not exists
        /// </returns>
        public static int UpdatePoseByEmail(String email, String pose)
        {
            var command = new MySqlCommand($"SELECT Employee_ID FROM Employee WHERE Employee_Email='{email}';", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                command = new MySqlCommand($"UPDATE Employee SET Employee_Pose='{pose}' WHERE Employee_Email='{email}'", connection);
                command.ExecuteNonQuery();
                return 1;
            }
            else
            {
                reader.Close();
                return 2;
            }
        }

        /// <summary>
        /// Function to update current logged employee
        /// </summary>
        /// <param name="oldPassword"> variable for old password </param>
        /// <param name="newPassword"> variable for new password </param>
        /// <returns>
        /// return 1: password successfully updated
        /// return 2: employee with that ID and old password does not exists
        /// </returns>
        public static int UpdatePassword(String oldPassword, String newPassword)
        {
            var command = new MySqlCommand($"SELECT Employee_ID FROM Employee WHERE Employee_Password='{oldPassword}' AND Employee_ID={ProgramVariables.ID};", connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                command = new MySqlCommand($"UPDATE Employee SET Employee_Password='{newPassword}' WHERE Employee_ID={ProgramVariables.ID}", connection);
                command.ExecuteNonQuery();
                return 1;
            }
            else
            {
                reader.Close();
                return 2;
            }
        }

        /// <summary>
        /// Function to log in and add login attempt
        /// </summary>
        /// <param name="email"> variable for email </param>
        /// <param name="password"> variable for password </param>
        /// <returns>
        /// return 1: successfully logged in
        /// return 0: employee with that email does not exists
        /// return -1: employee with that email or password does not exists
        /// return -2: employee account was temporary banned
        /// </returns>
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
                    ProgramVariables.ProfileURL = reader.IsDBNull(2) ? "" : reader.GetString(2);
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
