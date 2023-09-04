using System;
using System.Data;
using System.ServiceProcess;
using System.Data.SqlClient;
//using Oracle.ManagedDataAccess.Client;
using System.Net;
using System.IO;
using System.Timers;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json;

namespace MoveToDest
{
    public partial class Service1 : ServiceBase
    {
        long delay = 5000;
        protected string LogPath = ConfigurationManager.AppSettings["LogPath"];
        protected string MoveFilePathPath = ConfigurationManager.AppSettings["MovePath"];
        private SqlConnection AppConn = new SqlConnection(ConfigurationManager.AppSettings["Connection"]);
        //private OracleConnection AppConn = new OracleConnection(ConfigurationManager.AppSettings["Connection"]);
        //private OracleCommand Cmnd;
        //private OracleTransaction Trans;

        private SqlCommand Cmnd;
        private SqlTransaction Trans;
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteLog("Service started");


            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            try
            {

                delay = Int32.Parse(ConfigurationManager.AppSettings["IntervalInSeconds"]) * 1000;
            }
            catch
            {
                WriteLog("IntervalInSeconds key/value incorrect.");
            }


            if (delay < 5000)
            {
                WriteLog("Sleep time too short: Changed to default(5 secs).");
                delay = 5000;
            }

            Timer timer1 = new Timer();
            timer1.AutoReset = true;
            timer1.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer1.Interval = delay;
            timer1.Enabled = true;
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //write();
            Sendmsg();


        }

        private void write()
        {

            string sourcepath = ConfigurationManager.AppSettings["sourcepath"];
            WriteLog("Set source path");
            string[] sourcefiles = Directory.GetFiles(sourcepath);
            WriteLog("Get soutce path file");

            foreach (string childfile in sourcefiles)
            {
                WriteLog("Start to find file from loop");
                string sourceFileName = new FileInfo(childfile).Name;
                string destinationPath = ConfigurationManager.AppSettings["destinationPath"];

                string destinationFileName = sourceFileName;
                string sourceFile = Path.Combine(sourcepath, sourceFileName);
                WriteLog("Get file from source to destination");
                string destinationFile = Path.Combine(destinationPath, destinationFileName);
                WriteLog("Ready to copy");
                File.Copy(sourceFile, destinationFile, true);
                WriteLog("File copied");
                File.Delete(sourceFile);
                WriteLog("File deleted");
            }
        }
        //protected void Upload()
        //{

        //    string excelPath = ConfigurationManager.AppSettings["ExcelPath"];
        //    DirectoryInfo d = new DirectoryInfo(excelPath);
        //    FileInfo[] Files = d.GetFiles("*.xlsx");



        //    string str = "";
        //    WriteLog("ready to enter into loop");
        //    foreach (FileInfo file in Files)
        //    {
        //        str = file.Name;
        //        string sourceFilePath = ConfigurationManager.AppSettings["SourceFilePath"];
        //        string SourceFileName = Path.Combine(sourceFilePath, str);

        //        string destinationFilePath = ConfigurationManager.AppSettings["MovePath"];

        //        string destinationFileName = destinationFilePath + str;




        //        WriteLog("get file :" + str);
        //        String strConnection = ConfigurationManager.AppSettings["constr"];
        //        WriteLog("declare database connection");


        //        //string path = excelPath + str;

        //        //string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;Persist Security Info=False";
        //        //WriteLog("declare excel oledb connection");
        //        //OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

        //        //OleDbCommand cmd = new OleDbCommand("Select [Department No],[Department],[Emp No],[Name],[Date],[First],[Last] from [Table$]", excelConnection);

        //        //excelConnection.Open();
        //        //WriteLog("oledb open");
        //        //OleDbDataReader dReader;

        //        //dReader = cmd.ExecuteReader();

        //        //SqlBulkCopy sqlBulk = new SqlBulkCopy(strConnection);

        //        //sqlBulk.DestinationTableName = "tblAttLog";
        //        //WriteLog("ready to insert into database");
        //        //sqlBulk.WriteToServer(dReader);
        //        //WriteLog("data inserted");
        //        //excelConnection.Close();
        //        //WriteLog("Process completed");
        //        //File.Move(SourceFileName, destinationFileName);
        //        //WriteLog("File moved to bak folder");
        //        //excelConnection.Close();
        //        //cmd.Dispose();
        //    }
        //}
        public void Sendmsg()
        {
            WriteLog(" started");
            send_or_to_idra();
            WriteLog(" sent to idra");
            //WriteLog(" started policy data");
            //send_policy_to_idra();
            //WriteLog(" sent policy to idra");
            sendbirthdaysms();
            sendprsms();
            policyissues();
            policydue();
        }

        public void send_or_to_idra()
        {
            WriteLog(" process ");
            getPost();
            WriteLog(" process successfull ");
        }

        //public void send_policy_to_idra()
        //{
        //    WriteLog(" process policy ");
        //    authenticate_policy_api();
        //    WriteLog(" process policy successfull");

        //}

        //public static void authenticate_policy_api()
        //{
        //    string strurltest = string.Format("https://idra-ump.com/app/extern/v1/authenticate");
        //    //string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/authenticate");
        //    WebRequest requestObjPost = WebRequest.Create(strurltest);
        //    requestObjPost.Method = "POST";
        //    requestObjPost.ContentType = "application/json";
        //    string id = "50";
        //    string PostData = "{\"client_id\":\"astha\",\"client_secret\":\"FqHNiB8fMc\"}";
        //    //string PostData = "{\"client_id\":\"astha\",\"client_secret\":\"RmUjiGrGkJ\"}";
        //    using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
        //    {
        //        streamWriter.Write(PostData);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        var httpResponse = (HttpWebResponse)requestObjPost.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            var result2 = streamReader.ReadToEnd();
        //            Authenticate result = JsonConvert.DeserializeObject<Authenticate>(result2);
        //            string refresh_token_id = result.refresh_token;
        //            string access_token = result.access_token;
        //            string token_type = result.token_type;
        //            // Refresh_token(refresh_token_id);
        //            //Debug.WriteLine("Refresh Token:"+refresh_token_id.ToString());
        //            //Debug.WriteLine("Access Token:" + access_token.ToString());
        //            //Debug.WriteLine("Token Token:" + token_type.ToString());
                    
        //            refresh_token_method_policy(refresh_token_id, access_token, token_type);
        //        }
        //    }
        //    string LogPath = ConfigurationManager.AppSettings["LogPath"];
        //    string Msg = " policy api authentication successfull";
        //    FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs);
        //    sw.BaseStream.Seek(0, SeekOrigin.End);
        //    sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //    sw.Close();
        //    sw.Dispose();
        //    fs.Close();
        //    fs.Dispose();

        //}
        //public static void refresh_token_method_policy(string refresh_token_id, string access_token, string token_type)
        //{
        //    string strurltest = string.Format("https://idra-ump.com/app/extern/v1/refresh-token");

        //    //string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/refresh-token");
        //    WebRequest requestObjPost = WebRequest.Create(strurltest);
        //    requestObjPost.Method = "POST";
        //    requestObjPost.ContentType = "application/json";
        //    string refresh_token = refresh_token_id;
        //    string access_token_id = access_token;
        //    string token_type_id = token_type;
        //    string final = token_type + " " + access_token;
        //    //requestObjPost.Headers.Add("Authorization", "" + token_type + " " + access_token + "");
        //    requestObjPost.Headers.Add("Authorization", final);



        //    string PostData = "{\"refresh_token\":\"" + refresh_token + "\"}";
        //    using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
        //    {
        //        streamWriter.Write(PostData);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        var httpResponse_n = (HttpWebResponse)requestObjPost.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse_n.GetResponseStream()))
        //        {
        //            var result2 = streamReader.ReadToEnd();
        //            Authenticate result = JsonConvert.DeserializeObject<Authenticate>(result2);
        //            string access_token_id_new = result.access_token;
        //            //Original_Receipt(access_token_id_new);
        //           // Debug.WriteLine("new:" + access_token_id_new);
        //            Policy(access_token_id_new);
        //        }
        //    }
        //    string LogPath = ConfigurationManager.AppSettings["LogPath"];
        //    string Msg = " policy api refreash token successfull";
        //    FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs);
        //    sw.BaseStream.Seek(0, SeekOrigin.End);
        //    sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //    sw.Close();
        //    sw.Dispose();
        //    fs.Close();
        //    fs.Dispose();


        //}

        //protected static void Policy(string access_token_id_new)
        //{

        //    string LogPath = ConfigurationManager.AppSettings["LogPath"];
        //    string Msg = "";
        //    FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs);



        //    string strurltest = string.Format("https://idra-ump.com/app/extern/v1/policy");
        //    //string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/policy");
        //    WebRequest requestObjPost1 = WebRequest.Create(strurltest);
        //    requestObjPost1.Method = "POST";
        //    requestObjPost1.ContentType = "application/json";
        //    string access_token_id = access_token_id_new;
        //    //string final = "Bearer" + " " + access_token_id + "";
        //    string final = "Bearer" + " " + access_token_id + "";
        //    requestObjPost1.Headers.Add("Authorization", final);

        //    string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        //    SqlConnection con = new SqlConnection();
        //    con.ConnectionString = connectionString;
        //    con.Open();

        //    //string sql_find_pr = "select format(a.POLICY_NO,'0000000') as policyNumber,'ILB' as projectCode,upper(a.CLIENT_NAME) as policyHolderName,concat(upper(A.MADDRESS1), ', ', upper(A.MADDRESS2), ', ', upper(C.THN_NAME), ', ', upper(D.DISTRICT)) as [address],""' as postalCode,D.DISTRICT as district,iif(a.GENDER = '1', 'Male', iif(a.GENDER = '2', 'Female', a.GENDER)) as gender,a.CONTACT_NO as mobileNumber,iif(a.EMAIL_ID is null, ' ', a.EMAIL_ID) as email,format(a.DOB, 'yyyy-MM-dd') as dateOfBirth,format(a.COMM_DATE, 'yyyy-MM-dd') as policyStartDate,format(dateadd(year, a.term, a.COMM_DATE), 'yyyy-MM-dd') as policyEndDate,format(a.POLICY_DATE, 'yyyy-MM-dd') as riskStartDate,'EKOK' as policyType,e.PLAN_NAME as productName,format(a.PLAN_CODE, '00') as productCode,iif(a.payment_mood = 1, 'Yly', iif(a.payment_mood = 2, 'Hly', iif(a.payment_mood = 3, 'Qly', iif(a.payment_mood = 4, 'Mly', 'Sly')))) as premiumMode,cast(TERM as char) as term,convert(nvarchar, a.SUM_ASSURED) as assuredSum,cast(a.LIFE_PREMIUM as nvarchar) as lifePremium,cast((isnull(a.ACCIDENT_AMOUNT, 0) + isnull(a.CI_PREMIUM, 0) + isnull(a.HI_PREMIUM_NET, 0)) as nvarchar) as supplyPremium,cast((isnull(a.O_E_AMOUNT, 0) + isnull(a.F_E_AMOUNT, 0) + isnull(ME_AMOUNT, 0)) as nvarchar) as externalLoad,convert(nvarchar, a.TOT_PREMIUM) as totalPremium,iif(a.plan_code in (7, 9), ' ', format(a.next_prem_date, 'yyyy-MM-dd')) as nextPremiumDueDate,cast(a.NOS_INSTALLMENT as nchar) as noOfPaidInstallment,cast(a.tot_paid_amt as nvarchar) as totalPaidAmount,'""' as identificationType,iif(a.NID is null, ' ', a.nid) as identificationNumber,a.fa as agentId,b.CONTACT_NO as agentMobileNumber,upper(b.NAME) as agentName,iif(a.POLICY_STATUS = 'Inforce', '""', iif(a.POLICY_STATUS = 'Lapse', '""', a.POLICY_STATUS))[status]from tbl_ilb_policy_info as a,tbl_ilb_producer_info as b,tbl_ilb_thana as c,tbl_ilb_district as d,tbl_ilb_plan as ewhere A.fa = B.EMPIand A.MTHNCODE = C.thancodeand A.MDISTCODE = D.DISTCODEand a.plan_code = e.idand a.policy_status <> 'Cancel'";
        //    //SqlCommand pr_info = new SqlCommand(sql_find_pr, con);

        //    //Hashtable ht = new Hashtable();
        //    //ht.Add("policy_no", "All");
        //    //DataTable dt = ExecuteStoredProcedure("ilb_policy_info_idra", ht);

        //    string sql_api_data_primary = "exec [ilb_policy_info_idra]'ALL'";
        //    SqlCommand client_api_info_first = new SqlCommand(sql_api_data_primary, con);

        //    DataTable dt = new DataTable();
        //    client_api_info_first.CommandType = CommandType.Text;
        //    client_api_info_first.Connection = con;
        //    using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
        //    {
        //        dataAdapter.SelectCommand = client_api_info_first;
        //        dataAdapter.Fill(dt);
        //    }


        //    //DataTable dt_pr = new DataTable();

        //    //pr_info.CommandType = CommandType.Text;
        //    //pr_info.Connection = con;
        //    //using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
        //    //{
        //    //    dataAdapter.SelectCommand = pr_info;
        //    //    dataAdapter.Fill(dt_pr);
        //    //}
        //    int count_policy = dt.Rows.Count;

        //    while (count_policy > 0)
        //    {
        //        //string LogPath = ConfigurationManager.AppSettings["LogPath"];
        //        //string Msg = " policy data found successfully";
        //        //FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
        //        //StreamWriter sw = new StreamWriter(fs);
        //        Msg = ""+ count_policy + " policy data found successfully";
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //        sw.Close();
        //        sw.Dispose();
        //        fs.Close();
        //        fs.Dispose();

        //        string policy_no = dt.Rows[count_policy - 1]["policyNumber"].ToString();

        //        //  string pr_no = "0000000455";
        //        Msg = "policy no "+ policy_no +" found successfully";
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //        sw.Close();
        //        sw.Dispose();
        //        fs.Close();
        //        fs.Dispose();

        //        string sql_api_data = "exec [ilb_policy_info_idra]'" + policy_no + "'";
        //        SqlCommand client_api_info = new SqlCommand(sql_api_data, con);

        //        DataTable dt_api = new DataTable();
        //        client_api_info.CommandType = CommandType.Text;
        //        client_api_info.Connection = con;
        //        using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
        //        {
        //            dataAdapter.SelectCommand = client_api_info;
        //            dataAdapter.Fill(dt_api);
        //        }

        //        Msg = "policy data load successful";
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //        sw.Close();
        //        sw.Dispose();
        //        fs.Close();
        //        fs.Dispose();

        //        //Hashtable ht_policy = new Hashtable();
        //        //ht_policy.Add("policy_no", policy_no);
        //        //DataTable dt_api = ExecuteStoredProcedure("ilb_policy_info_idra", ht_policy);
        //        string policyNumber = dt_api.Rows[0]["policyNumber"].ToString();
        //        string projectCode = dt_api.Rows[0]["projectCode"].ToString();
        //        string policyHolderName = dt_api.Rows[0]["policyHolderName"].ToString();
        //        string dateOfBirth = dt_api.Rows[0]["dateOfBirth"].ToString();
        //        string policyStartDate = dt_api.Rows[0]["policyStartDate"].ToString();
        //        string policyEndDate = dt_api.Rows[0]["policyEndDate"].ToString();
        //        string riskStartDate = dt_api.Rows[0]["riskStartDate"].ToString();
        //        string term = dt_api.Rows[0]["term"].ToString();
        //        string assuredSum = dt_api.Rows[0]["assuredSum"].ToString();
        //        string totalPremium = dt_api.Rows[0]["totalPremium"].ToString();
        //        string noOfPaidInstallment = dt_api.Rows[0]["noOfPaidInstallment"].ToString();
        //        string totalPaidAmount = dt_api.Rows[0]["totalPaidAmount"].ToString();
        //        string status = dt_api.Rows[0]["status"].ToString();
        //        string email = dt_api.Rows[0]["email"].ToString();
        //        string address = dt_api.Rows[0]["address"].ToString();
        //        string postalCode = dt_api.Rows[0]["postalCode"].ToString();
        //        string district = dt_api.Rows[0]["district"].ToString();
        //        string gender = dt_api.Rows[0]["gender"].ToString();
        //        string mobileNumber = dt_api.Rows[0]["mobileNumber"].ToString();
        //        string policyType = dt_api.Rows[0]["policyType"].ToString();
        //        string productName = dt_api.Rows[0]["productName"].ToString();
        //        string productCode = dt_api.Rows[0]["productCode"].ToString();
        //        string premiumMode = dt_api.Rows[0]["premiumMode"].ToString();
        //        string lifePremium = dt_api.Rows[0]["lifePremium"].ToString();
        //        string supplyPremium = dt_api.Rows[0]["supplyPremium"].ToString();
        //        string externalLoad = dt_api.Rows[0]["externalLoad"].ToString();
        //        string nextPremiumDueDate = dt_api.Rows[0]["nextPremiumDueDate"].ToString();
        //        string identificationType = dt_api.Rows[0]["identificationType"].ToString();
        //        string identificationNumber = dt_api.Rows[0]["identificationNumber"].ToString();
        //        string agentId = dt_api.Rows[0]["agentId"].ToString();
        //        string agentMobileNumber = dt_api.Rows[0]["agentMobileNumber"].ToString();
        //        string agentName = dt_api.Rows[0]["agentName"].ToString();
        //        decimal sumAtRisk = 1200;
        //        string policyOption = null;
        //        string surrenderDate = "2022-11-01";

        //        //string PostData = "{\"refresh_token\":\"'" + refresh_token_id + "'\"}";
        //        //string PostData = "{\"header\":\"'" + access_token + "'\",\"refresh_token\":\"'"+ refresh_token_id + "'\"}";
        //        //string PostData = "{\"orId\":\"'" + orId + "'\",\"orSerialNumber\":\"'"+ orSerialNumber + "'\",\"policyNumber\":\"'"+ policyNumber + "'\",\"projectCode\":\"'"+ projectCode + "'\",\"officeBranchCode\":\"'"+ officeBranchCode + "'\",\"officeBranchName\":\"'"+ officeBranchName + "'\",\"orType\":\"'"+ orType + "'\",\"orDate\":\"'"+ orDate + "'\",\"dueDate\":\"'"+ dueDate + "'\",\"fromInstallment\":\"'"+ fromInstallment + "'\",\"toInstallment\":\"'"+ toInstallment + "'\",\"premiumUnitAmount\":\"'"+ premiumUnitAmount + "'\",\"totalPremiumAmount\":\"'"+ totalPremiumAmount + "'\",\"lateFee\":\"'"+ lateFee + "'\",\"suspenseAmount\":\"'"+ suspenseAmount + "'\",\"others\":\"'"+ others + "'\",\"totalPayableAmount\":\"'" + totalPayableAmount + "'\",\"modeOfPayment\":\"'" + modeOfPayment + "'\",\"paymentDetail\":\"'" + paymentDetail + "'\",\"prId\":\"'" + prId + "'\",\"prDate\":\"'" + prDate + "'\",\"nextPremiumDueDate\":\"'" + nextPremiumDueDate + "'\",\"totalPremiumPaidSoFar\":\"'" + totalPremiumPaidSoFar + "'\",\"premiumMode\":\"'" + premiumMode + "'\",\"depositDate\":\"'" + depositDate + "'\",\"depositedToBank\":\"'" + depositedToBank + "'\",\"depositedToBranch\":\"'" + depositedToBranch + "'\",\"depositedToAccountNumber\":\"'" + depositedToAccountNumber + "'\",\"mfs\":\"'" + mfs + "'\",\"mfsAccountNumber\":\"'" + mfsAccountNumber + "'\",\"agentName\":\"'" + agentName + "'\",\"agentId\":\"'" + agentId + "'\",\"productCode\":\"'" + productCode + "'\",\"riskStartDate\":\"'" + riskStartDate + "'\",\"dateOfBirth\":\"'" + dateOfBirth + "'\"}";
        //        string PostData = "{\"policyNumber\":\"" + policyNumber + "\",\"projectCode\":\"" + projectCode + "\",\"policyHolderName\":\"" + policyHolderName + "\",\"dateOfBirth\":\"" + dateOfBirth + "\",\"policyStartDate\":\"" + policyStartDate + "\",\"policyEndDate\":\"" + policyEndDate + "\",\"riskStartDate\":\"" + riskStartDate + "\",\"term\":\"" + term + "\",\"assuredSum\":\"" + assuredSum + "\",\"totalPremium\":\"" + totalPremium + "\",\"noOfPaidInstallment\":\"" + noOfPaidInstallment + " \",\"totalPaidAmount\":\"" + totalPaidAmount + "\",\"status\":\"" + status + "\",\"email\":\"" + email + "\",\"address\":\"" + address + "\",\"postalCode\":\"" + postalCode + "\",\"district\":\"" + district + "\",\"gender\":\"" + gender + "\",\"mobileNumber\":\"" + mobileNumber + "\",\"policyType\":\"" + policyType + "\",\"productName\":\"" + productName + "\",\"productCode\":\"" + productCode + "\",\"premiumMode\":\"" + premiumMode + "\",\"lifePremium\":\"" + lifePremium + "\",\"supplyPremium\":\"" + supplyPremium + "\",\"externalLoad\":\"" + externalLoad + "\",\"nextPremiumDueDate\":\"" + nextPremiumDueDate + "\",\"identificationType\":\"" + identificationType + "\",\"identificationNumber\":\"" + identificationNumber + "\",\"agentId\":\"" + agentId + "\",\"agentMobileNumber\":\"" + agentMobileNumber + "\",\"agentName\":\"" + agentName + "\",\"sumAtRisk\":\"" + sumAtRisk + "\",\"policyOption\":\"" + policyOption + "\",\"surrenderDate\":\"" + surrenderDate + "\"}";

        //        using (var streamWriter = new StreamWriter(requestObjPost1.GetRequestStream()))
        //        {
        //            streamWriter.Write(PostData);
        //            streamWriter.Flush();
        //            streamWriter.Close();

        //            var httpResponse_n = (HttpWebResponse)requestObjPost1.GetResponse();
        //            using (var streamReader = new StreamReader(httpResponse_n.GetResponseStream()))
        //            {
        //                var result2 = streamReader.ReadToEnd();
        //                //Status result = JsonConvert.DeserializeObject<Status>(result2);
        //                //Debug.WriteLine("status:" + result.status);
        //                //Debug.WriteLine("code:" + result.code);
        //                //Debug.WriteLine("message:" + result.message);

        //            }
        //        }

        //        Msg = "policy no " + policy_no + " posted successfully";
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //        sw.Close();
        //        sw.Dispose();
        //        fs.Close();
        //        fs.Dispose();

        //        string sql_update_policy = "update tbl_ilb_policy_info set idra_flag = 1 where policy_no = '" + policy_no + "'";

        //        SqlCommand cmd = new SqlCommand(sql_update_policy, con);
        //        cmd.ExecuteNonQuery();

        //        Msg = "policy no " + policy_no + " updated successfully";
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //        sw.Close();
        //        sw.Dispose();
        //        fs.Close();
        //        fs.Dispose();

        //        count_policy = count_policy - 1;

        //    }

        //    //string LogPath = ConfigurationManager.AppSettings["LogPath"];
        //    //string Msg = " policy sent successfull to idra";
        //    //FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
        //    //StreamWriter sw = new StreamWriter(fs);
        //    Msg = " policy sent successfull to idra";
        //    sw.BaseStream.Seek(0, SeekOrigin.End);
        //    sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
        //    sw.Close();
        //    sw.Dispose();
        //    fs.Close();
        //    fs.Dispose();

        //}

        //public static DataTable ExecuteStoredProcedure(string storedProcedureName, Hashtable parameters)
        //{
        //    OpenAppConnection();
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand(storedProcedureName, AppConn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandTimeout = 300;// 5 mins
        //                                 //if (Trans == null)
        //                                 //{
        //                                 //    Cmnd = new SqlCommand();
        //                                 //    Cmnd.Connection = AppConn;
        //                                 //}
        //                                 //Cmnd.CommandType = CommandType.StoredProcedure;
        //                                 //Cmnd.CommandText = storedProcedureName;

        //        if (parameters != null && parameters.Count > 0)
        //        {
        //            foreach (string parametername in parameters.Keys)
        //            {
        //                SqlParameter param = new SqlParameter("@" + parametername, parameters[parametername]);
        //                cmd.Parameters.Add(param);

        //            }
        //        }
        //        //Cmnd.ExecuteNonQuery();
        //        //Cmnd.Parameters.Clear();
        //        //Cmnd.CommandType = CommandType.Text;

        //        DataSet ds = new DataSet();

        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.SelectCommand = cmd;

        //        adp.Fill(ds);


        //        return ds.Tables[0];


        //    }
        //    catch (SqlException Ex)
        //    {
        //        if (Trans != null)
        //        {
        //            Trans.Rollback();
        //            Trans = null;
        //        }
        //        throw Ex;
        //    }
        //    catch (Exception Ex)
        //    {
        //        //if (Trans != null)
        //        //{
        //        //    Trans.Rollback();
        //        //    Trans = null;
        //        //}

        //        throw Ex;
        //    }
        //    finally
        //    {
        //        //if (Trans == null)
        //        //{
        //        CloseAppConnection();
        //        // }

        //    }

        //}

        public static void getPost()
            {
                //POST
                string strurltest = string.Format("https://idra-ump.com/app/extern/v1/authenticate");
                //string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/authenticate");
                WebRequest requestObjPost = WebRequest.Create(strurltest);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";
                string id = "50";
                string PostData = "{\"client_id\":\"astha\",\"client_secret\":\"FqHNiB8fMc\"}";
                //string PostData = "{\"client_id\":\"astha\",\"client_secret\":\"RmUjiGrGkJ\"}";
                using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
                {
                    streamWriter.Write(PostData);
                    streamWriter.Flush();
                    streamWriter.Close();
                    var httpResponse = (HttpWebResponse)requestObjPost.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result2 = streamReader.ReadToEnd();
                        Authenticate result = JsonConvert.DeserializeObject<Authenticate>(result2);
                        string refresh_token_id = result.refresh_token;
                        string access_token = result.access_token;
                        string token_type = result.token_type;
                        // Refresh_token(refresh_token_id);
                        Refresh_token_method(refresh_token_id, access_token, token_type);


                    }
                }
                string LogPath = ConfigurationManager.AppSettings["LogPath"];
                string Msg = " get post successfull";
                FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();
     
               // WriteLog(" process successfull ");
        }
        private static void Refresh_token_method(string refresh_token_id, string access_token, string token_type)
        {
            string strurltest = string.Format("https://idra-ump.com/app/extern/v1/refresh-token");

            // string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/refresh-token");
            WebRequest requestObjPost = WebRequest.Create(strurltest);
            requestObjPost.Method = "POST";
            requestObjPost.ContentType = "application/json";
            string refresh_token = refresh_token_id;
            string access_token_id = access_token;
            string token_type_id = token_type;
            string final = token_type + " " + access_token;
            //requestObjPost.Headers.Add("Authorization", "" + token_type + " " + access_token + "");
            requestObjPost.Headers.Add("Authorization", final);



            string PostData = "{\"refresh_token\":\"" + refresh_token + "\"}";
            using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
            {
                streamWriter.Write(PostData);
                streamWriter.Flush();
                streamWriter.Close();
                var httpResponse_n = (HttpWebResponse)requestObjPost.GetResponse();
                using (var streamReader = new StreamReader(httpResponse_n.GetResponseStream()))
                {
                    var result2 = streamReader.ReadToEnd();
                    Authenticate result = JsonConvert.DeserializeObject<Authenticate>(result2);
                    string access_token_id_new = result.access_token;
                    Original_Receipt(access_token_id_new);

                }
            }
                string LogPath = ConfigurationManager.AppSettings["LogPath"];
                string Msg = " refresh token successfull";
                FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();

            }
        private static void Original_Receipt(string access_token_id_new)
        {
            string strurltest = string.Format("https://idra-ump.com/app/extern/v1/original-receipt");
            // string strurltest = string.Format("https://idra-ump.com/test/app/extern/v1/original-receipt");
            WebRequest requestObjPost = WebRequest.Create(strurltest);
            requestObjPost.Method = "POST";
            requestObjPost.ContentType = "application/json";
            string access_token_id = access_token_id_new;
            requestObjPost.Headers.Add("Authorization", "" + "Bearer" + " " + access_token_id + "");
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = connectionString;
            con.Open();

            string sql_find_pr = "select ROW_NUMBER()over  (order by pr_no) as sl, PR_NO from tbl_ilb_collection_main where isnull(idra_flag,0) = 0";
            SqlCommand pr_info = new SqlCommand(sql_find_pr, con);

            DataTable dt_pr = new DataTable();

            pr_info.CommandType = CommandType.Text;
            pr_info.Connection = con;
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                dataAdapter.SelectCommand = pr_info;
                dataAdapter.Fill(dt_pr);
            }
            int count_pr = dt_pr.Rows.Count;

            while (count_pr > 0)
            {
                int pr_no = Convert.ToInt32(dt_pr.Rows[count_pr - 1]["PR_NO"].ToString());

                //  string pr_no = "0000000455";

                string sql_api_data = "exec [ilb_or_data_to_idra]" + pr_no + "";
                SqlCommand client_api_info = new SqlCommand(sql_api_data, con);

                DataTable dt_api = new DataTable();
                client_api_info.CommandType = CommandType.Text;
                client_api_info.Connection = con;
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    dataAdapter.SelectCommand = client_api_info;
                    dataAdapter.Fill(dt_api);
                }


                string orId = dt_api.Rows[0]["orid"].ToString();
                string orSerialNumber = dt_api.Rows[0]["orSerialNumber"].ToString();
                string policyNumber = dt_api.Rows[0]["policyNumber"].ToString();
                string projectCode = dt_api.Rows[0]["projectCode"].ToString();
                string officeBranchCode = dt_api.Rows[0]["officeBranchCode"].ToString();
                string officeBranchName = dt_api.Rows[0]["officeBranchName"].ToString();
                string orType = dt_api.Rows[0]["orType"].ToString();
                string orDate = dt_api.Rows[0]["orDate"].ToString();
                string dueDate = dt_api.Rows[0]["dueDate"].ToString();
                string fromInstallment = dt_api.Rows[0]["fromInstallment"].ToString();
                string toInstallment = dt_api.Rows[0]["toInstallment"].ToString();
                string premiumUnitAmount = dt_api.Rows[0]["premiumUnitAmount"].ToString();
                string totalPremiumAmount = dt_api.Rows[0]["totalPremiumAmount"].ToString();
                string lateFee = dt_api.Rows[0]["lateFee"].ToString();
                string suspenseAmount = dt_api.Rows[0]["suspenseAmount"].ToString();
                string others = dt_api.Rows[0]["others"].ToString();
                string totalPayableAmount = dt_api.Rows[0]["totalPayableAmount"].ToString();
                string modeOfPayment = dt_api.Rows[0]["modeOfPayment"].ToString();
                string paymentDetail = dt_api.Rows[0]["paymentDetail"].ToString();
                string prId = dt_api.Rows[0]["prId"].ToString();
                string prDate = dt_api.Rows[0]["prDate"].ToString();
                string nextPremiumDueDate = dt_api.Rows[0]["nextPremiumDueDate"].ToString();
                string totalPremiumPaidSoFar = dt_api.Rows[0]["totalPremiumPaidSoFar"].ToString();
                string premiumMode = dt_api.Rows[0]["premiumMode"].ToString();
                string depositDate = dt_api.Rows[0]["depositDate"].ToString();
                string depositedToBank = dt_api.Rows[0]["depositedToBank"].ToString();
                string depositedToBranch = dt_api.Rows[0]["depositedToBranch"].ToString();
                string depositedToAccountNumber = dt_api.Rows[0]["depositedToAccountNumber"].ToString();
                string mfs = dt_api.Rows[0]["mfs"].ToString();
                string mfsAccountNumber = dt_api.Rows[0]["mfsAccountNumber"].ToString();
                string agentName = dt_api.Rows[0]["agentName"].ToString();
                string agentId = dt_api.Rows[0]["agentId"].ToString();
                string productCode = dt_api.Rows[0]["productCode"].ToString();
                string riskStartDate = dt_api.Rows[0]["riskStartDate"].ToString();
                string dateOfBirth = dt_api.Rows[0]["dateOfBirth"].ToString();

                //string PostData = "{\"refresh_token\":\"'" + refresh_token_id + "'\"}";
                //string PostData = "{\"header\":\"'" + access_token + "'\",\"refresh_token\":\"'"+ refresh_token_id + "'\"}";
                //string PostData = "{\"orId\":\"'" + orId + "'\",\"orSerialNumber\":\"'"+ orSerialNumber + "'\",\"policyNumber\":\"'"+ policyNumber + "'\",\"projectCode\":\"'"+ projectCode + "'\",\"officeBranchCode\":\"'"+ officeBranchCode + "'\",\"officeBranchName\":\"'"+ officeBranchName + "'\",\"orType\":\"'"+ orType + "'\",\"orDate\":\"'"+ orDate + "'\",\"dueDate\":\"'"+ dueDate + "'\",\"fromInstallment\":\"'"+ fromInstallment + "'\",\"toInstallment\":\"'"+ toInstallment + "'\",\"premiumUnitAmount\":\"'"+ premiumUnitAmount + "'\",\"totalPremiumAmount\":\"'"+ totalPremiumAmount + "'\",\"lateFee\":\"'"+ lateFee + "'\",\"suspenseAmount\":\"'"+ suspenseAmount + "'\",\"others\":\"'"+ others + "'\",\"totalPayableAmount\":\"'" + totalPayableAmount + "'\",\"modeOfPayment\":\"'" + modeOfPayment + "'\",\"paymentDetail\":\"'" + paymentDetail + "'\",\"prId\":\"'" + prId + "'\",\"prDate\":\"'" + prDate + "'\",\"nextPremiumDueDate\":\"'" + nextPremiumDueDate + "'\",\"totalPremiumPaidSoFar\":\"'" + totalPremiumPaidSoFar + "'\",\"premiumMode\":\"'" + premiumMode + "'\",\"depositDate\":\"'" + depositDate + "'\",\"depositedToBank\":\"'" + depositedToBank + "'\",\"depositedToBranch\":\"'" + depositedToBranch + "'\",\"depositedToAccountNumber\":\"'" + depositedToAccountNumber + "'\",\"mfs\":\"'" + mfs + "'\",\"mfsAccountNumber\":\"'" + mfsAccountNumber + "'\",\"agentName\":\"'" + agentName + "'\",\"agentId\":\"'" + agentId + "'\",\"productCode\":\"'" + productCode + "'\",\"riskStartDate\":\"'" + riskStartDate + "'\",\"dateOfBirth\":\"'" + dateOfBirth + "'\"}";
                string PostData = "{\"orId\":\"" + orId + "\",\"orSerialNumber\":\"" + orSerialNumber + "\",\"policyNumber\":\"" + policyNumber + "\",\"projectCode\":\"" + projectCode + "\",\"officeBranchCode\":\"" + officeBranchCode + "\",\"officeBranchName\":\"" + officeBranchName + "\",\"orType\":\"" + orType + "\",\"orDate\":\"" + orDate + "\",\"dueDate\":\"" + dueDate + "\",\"fromInstallment\":\"" + fromInstallment + " \",\"toInstallment\":\"" + toInstallment + "\",\"premiumUnitAmount\":\"" + premiumUnitAmount + "\",\"totalPremiumAmount\":\"" + totalPremiumAmount + "\",\"lateFee\":\"" + lateFee + "\",\"suspenseAmount\":\"" + suspenseAmount + "\",\"others\":\"" + others + "\",\"totalPayableAmount\":\"" + totalPayableAmount + "\",\"modeOfPayment\":\"" + modeOfPayment + "\",\"paymentDetail\":\"" + paymentDetail + "\",\"prId\":\"" + prId + "\",\"prDate\":\"'" + prDate + "'\",\"nextPremiumDueDate\":\"" + nextPremiumDueDate + "\",\"totalPremiumPaidSoFar\":\"" + totalPremiumPaidSoFar + "\",\"premiumMode\":\"" + premiumMode + "\",\"depositDate\":\"" + depositDate + "\",\"depositedToBank\":\"" + depositedToBank + "\",\"depositedToBranch\":\"" + depositedToBranch + "\",\"depositedToAccountNumber\":\"" + depositedToAccountNumber + "\",\"mfs\":\"" + mfs + "\",\"mfsAccountNumber\":\"" + mfsAccountNumber + "\",\"agentName\":\"" + agentName + "\",\"agentId\":\"" + agentId + "\",\"productCode\":\"" + productCode + "\",\"riskStartDate\":\"" + riskStartDate + "\",\"dateOfBirth\":\"" + dateOfBirth + "\"}";
                using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
                {
                    streamWriter.Write(PostData);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse_n = (HttpWebResponse)requestObjPost.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse_n.GetResponseStream()))
                    {
                        var result2 = streamReader.ReadToEnd();
                        //Authenticate result = JsonConvert.DeserializeObject<Authenticate>(result2);


                    }
                }

                string sql_update_pr = "update tbl_ilb_collection_main set idra_flag = 1 where PR_NO = '" + pr_no + "'";

                SqlCommand cmd = new SqlCommand(sql_update_pr, con);
                cmd.ExecuteNonQuery();

                count_pr = count_pr - 1;

            }

                string LogPath = ConfigurationManager.AppSettings["LogPath"];
                string Msg = " OR sent successfully";
                FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();

            }
        public void sendbirthdaysms()
        {
            WriteLog("Birthday sms sending process started");
            string sql = "select  ROW_NUMBER() OVER (ORDER BY s.policy_no) as sl_no, s.policy_no, P.CONTACT_NO, p.client_name, s.dob_flag, s.dob from tbl_ilb_sms_policy s inner join tbl_ilb_policy_info p on S.POLICY_NO = P.POLICY_NO where DATEPART(MM, s.dob) = datepart(MM, getdate()) AND DATEPART(D, s.dob) = datepart(D, getdate()) and s.dob_flag = 0";
            WriteLog("sql query read 1");
            DataTable dt = GetDataTableByCommand(sql);
            WriteLog("query executed for datatable 2");
            int count = dt.Rows.Count;
            while (count > 0)
            {
                WriteLog("data found on datatable 3");
                int policy_no = Convert.ToInt32(dt.Rows[count - 1]["policy_no"].ToString());
                string mobile_no = dt.Rows[count - 1]["CONTACT_NO"].ToString();
                WriteLog("mobile no found for policy no " + policy_no + " and Mobile No " + mobile_no + "");
                string sms_body = @"শুভ জন্মদিন। সন্মানিত পলিসি গ্রাহক, আপনাকে আস্থা লাইফ পরিবারের পক্ষ থেকে জন্মদিনের শুভেচ্ছা। সুস্থ থাকুন, নিরাপদে থাকুন, আস্থা লাইফের সাথেই থাকুন। আস্থা লাইফ ইনস্যুরেন্স কোম্পানি লিমিটেড। ";
                WriteLog("sms body created");
                SendSMS(mobile_no, sms_body);
                WriteLog("sms through link successful 6");
                //SendSMS(mobile_no, sms_body);
                WriteLog("sms sent to " + mobile_no + "");
                string sql_update = "UPDATE tbl_ilb_sms_policy  SET dob_flag = 1   WHERE policy_no=" + policy_no + "";
                WriteLog("update query  " + policy_no + "");
                //AppConn.Open();
                ExecuteScalar(sql_update);
                //cmd.ExecuteNonQuery();
                WriteLog("status updated");
                string sql_update_previous = "UPDATE tbl_ilb_sms_policy  SET dob_flag = 0   WHERE DATEPART(MM,dob) < datepart(MM, getdate()) AND DATEPART(D,dob) < datepart(D, getdate())";
                ExecuteScalar(sql_update_previous);
                //OracleCommand cmd_priv = new OracleCommand(sql_update_previous, AppConn);
                //cmd_priv.ExecuteNonQuery();
                //AppConn.Close();
                WriteLog("Birthday SMS sent successfully");
                count = count - 1;
            }

        }
        public void sendprsms()
        {
            WriteLog("PR sms sending process started");

            string sql_pr = "select  ROW_NUMBER() OVER (ORDER BY s.policy_no) as sl_no, G.next_prem_date as PREM_DUE_DATE, C.PR_NO,d.sms_flag,c.policy_no,G.CLIENT_NAME,G.CONTACT_NO, pro.contact_no as contact_fa,  G.SUSPENSE_AMT as SUSPENSE_AMT, C.TOT_PREMIUM from tbl_ilb_collection_main c INNER JOIN tbl_ilb_policy_info g ON g.policy_no = c.policy_no inner join tbl_ilb_producer_info pro on pro.empid = g.fa INNER JOIN tbl_ilb_sms_policy s ON S.POLICY_NO =C.POLICY_NO INNER JOIN tbl_ilb_sms_pr d ON c.PR_NO = d.PR_NO and D.SMS_FLAG = 0";
            DataTable dt_pr = GetDataTableByCommand(sql_pr);
            int count = dt_pr.Rows.Count;
            WriteLog("Data found");
            while (count > 0)
            {
                string pr_no = dt_pr.Rows[count - 1]["PR_NO"].ToString();
                string policy_no = dt_pr.Rows[count - 1]["policy_no"].ToString();
                string Prem_due_date = dt_pr.Rows[count - 1]["PREM_DUE_DATE"].ToString();
                WriteLog("PR No " + pr_no + "");
                string pr_mobile = dt_pr.Rows[count - 1]["CONTACT_NO"].ToString();
                WriteLog("Mobile No " + pr_mobile + "");

                string fa_mobile = dt_pr.Rows[count - 1]["contact_fa"].ToString();
                WriteLog("Mobile No " + fa_mobile + "");

                string pr_amount = dt_pr.Rows[count - 1]["TOT_PREMIUM"].ToString();

                string suspense_amt = dt_pr.Rows[count - 1]["SUSPENSE_AMT"].ToString();

                string sms_body_pr = @"সন্মানিত পলিসি গ্রাহক, আপনার পলিসি নং " + policy_no + " এর জন্য  BDT " + pr_amount + " প্রিমিয়াম গৃহীত হয়েছে। আপনার অতিরিক্ত প্রিমিয়াম ব্যালান্স BDT " + suspense_amt + "। যা আপনার পরবর্তী প্রিমিয়ামের সাথে সমন্বয় করা হবে। আপনার পরবর্তী প্রিমিয়াম প্রদানের তারিখ " + Prem_due_date + "। আস্থা লাইফ পরিবারের সাথে থাকার জন্য ধন্যবাদ। Download your PR through : https://erp.amaderastha.com/Report_Viewer/rpt_viewer_pr_sheet_final.aspx?pr_no="+pr_no+"";

                string sms_body_pr_FA = @"সন্মানিত এফ এ, আপনার পলিসি নং " + policy_no + " এর জন্য  BDT " + pr_amount + " প্রিমিয়াম গৃহীত হয়েছে। আপনার অতিরিক্ত প্রিমিয়াম ব্যালান্স BDT " + suspense_amt + "। যা আপনার পরবর্তী প্রিমিয়ামের সাথে সমন্বয় করা হবে। আপনার পরবর্তী প্রিমিয়াম প্রদানের তারিখ " + Prem_due_date + "। আস্থা লাইফ পরিবারের সাথে থাকার জন্য ধন্যবাদ। Download your PR through : https://erp.amaderastha.com/Report_Viewer/rpt_viewer_pr_sheet_final.aspx?pr_no=" + pr_no + "";
                // string sms_body_pr = @"Respected PolicyHolder, We received BDT " + pr_amount + " Premium for  policy " + policy_no + " your next due date " + Prem_due_date + " Thank you for staying with Astha Life family. ";
                WriteLog("sms body created");
                SendSMS(pr_mobile, sms_body_pr);
                SendSMS(fa_mobile, sms_body_pr_FA);

                WriteLog("sms sent");
                string sql_pr_update = "UPDATE tbl_ilb_sms_pr SET SMS_FLAG = 1 WHERE pr_no='" + pr_no + "'";
                ExecuteScalar(sql_pr_update);
                //AppConn.Open();
                //OracleCommand cmd_pr = new OracleCommand(sql_pr_update, AppConn);
                //cmd_pr.ExecuteNonQuery();
                //AppConn.Close();
                WriteLog("PR Status Updated");
                WriteLog("PR SMS sent successfully");
                count = count - 1;
            }
        }
        private void policyissues()
        {
            WriteLog("policy issues sms sending process started");
            string sql_policy_issues = "select  ROW_NUMBER() OVER (ORDER BY a.policy_no) as sl_no,a.policy_no, pr.proposal_no, isnull(pr.form_no,0) as form_no,a.client_name,a.dob,a.contact_no, p.contact_no as contact_no_fa,b.policy_issue_flag, b.PREM_DUE_DATE, case when A.SUM_ASSURED <= 1000000 then 'silver'  else case when A.SUM_ASSURED between 1000001 and 2000000 then 'Gold' else case when A.SUM_ASSURED between 2000001 and 4000000 then  'Platinum' else 'Signature' end end end as policy_type  from tbl_ilb_policy_info a  inner join tbl_ilb_producer_info p on p.empid = a.fa left join tbl_ilb_profile pr on pr.proposal_no = a.proposal_no INNER JOIN tbl_ilb_sms_policy b ON a.policy_no = b.policy_no and B.policy_issue_flag = 0 where a.policy_status <> 'Cancel'";
            DataTable dt_policy_issue = GetDataTableByCommand(sql_policy_issues);
            int count = dt_policy_issue.Rows.Count;
            while (count > 0)
            {

                string policy_type = dt_policy_issue.Rows[count - 1]["policy_type"].ToString();

                string policy_no = dt_policy_issue.Rows[count - 1]["policy_no"].ToString();

                string prem_due_date = dt_policy_issue.Rows[count - 1]["PREM_DUE_DATE"].ToString();
                //WriteLog("PR No " + pr_no + "");
                string policy_mobile = dt_policy_issue.Rows[count - 1]["contact_no"].ToString();

                string fa_mobile = dt_policy_issue.Rows[count - 1]["contact_no_fa"].ToString();
                //WriteLog("Mobile No " + pr_mobile + "");
                string policy_name = dt_policy_issue.Rows[count - 1]["client_name"].ToString();

                string form_no = dt_policy_issue.Rows[count - 1]["form_no"].ToString();

                string proposal_no = dt_policy_issue.Rows[count - 1]["proposal_no"].ToString();

                string sms_body_policy = @"সন্মানিত পলিসি গ্রাহক, আপনার পলিসি  " + policy_no + "  গ্রহন করা হয়েছে এবং এখন আপনি কভারেজের অধীনে আছেন। Policy category : " + policy_type + " পরবর্তী প্রিমিয়াম প্রদানের তারিখ " + prem_due_date + "। ধন্যবাদ, আস্থা লাইফ।";

                string sms_body_fa = @"সন্মানিত এফ এ, আপনার পলিসি  " + policy_no + "  গ্রহন করা হয়েছে যার ফর্ম নংঃ '" + form_no + "'  এবং প্রপোজাল নংঃ '" + proposal_no + "'। Policy category : " + policy_type + "। ধন্যবাদ, আস্থা লাইফ।";
                // string sms_body_policy = @"Respected Policy owner your policy  " + policy_no + " is accepted and now you are under coverage. Policy category : " + policy_type + ". Next due date " + prem_due_date + " Thanking you, ASTHA LIFE";
                //WriteLog("sms body created");
                SendSMS(policy_mobile, sms_body_policy);
                SendSMS(fa_mobile, sms_body_fa);
                string sql_policy_issue_update = "UPDATE tbl_ilb_sms_policy SET policy_issue_flag  = 1 WHERE policy_no='" + policy_no + "'";
                ExecuteScalar(sql_policy_issue_update);
                //AppConn.Open();
                //OracleCommand cmd_policy_issue = new OracleCommand(sql_policy_issue_update, AppConn);
                //cmd_policy_issue.ExecuteNonQuery();
                //AppConn.Close();
                WriteLog("Policy issue Status Updated");
                WriteLog("Policy issue SMS sent successfully");
                count = count - 1;
            }
        }
        private void policydue()
        {
            WriteLog("policy due sms sending process started");
            string sql_policy_due = "select  ROW_NUMBER() OVER (ORDER BY a.policy_no) as sl_no,a.policy_no, case when a.payment_mood = 2 then iif(((a.nos_installment +1)  % 2) = 1 , a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0))) when a.payment_mood = 3 then iif(((a.nos_installment +1)  % 4) = 1, a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0)))  when a.payment_mood = 4 then iif(((a.nos_installment +1)  % 12) = 1, a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0))) else a.[tot_premium] end as tot_premium,a.client_name,a.dob,a.contact_no,b.prem_due_date_flag,b.PREM_DUE_DATE from tbl_ilb_policy_info a INNER JOIN tbl_ilb_sms_policy b ON a.policy_no=b.policy_no and B.prem_due_date_flag=0 and  datediff(day,b.PREM_DUE_DATE, getdate()) <= 7 and datediff(day,b.PREM_DUE_DATE, getdate()) > 0 where a.policy_status in ('Lapse','Inforce')";
            
      //      case when a.payment_mood = 2 then iif(((a.nos_installment +1)  % 2) = 1 , a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0)))
				  //when a.payment_mood = 3 then iif(((a.nos_installment +1)  % 4) = 1, a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0)))
				  //when a.payment_mood = 4 then iif(((a.nos_installment +1)  % 12) = 1, a.[tot_premium],(a.[tot_premium] - Isnull(a.[hi_premium_net], 0)))

      //            else a.[tot_premium] end as tot_premium



            DataTable dt_policy_due = GetDataTableByCommand(sql_policy_due);
            int count = dt_policy_due.Rows.Count;
            while (count > 0)
            {
                string policy_no = dt_policy_due.Rows[count - 1]["policy_no"].ToString();

                string tot_premium = dt_policy_due.Rows[count - 1]["TOT_PREMIUM"].ToString();
                //WriteLog("PR No " + pr_no + "");
                string policy_mobile = dt_policy_due.Rows[count - 1]["contact_no"].ToString();
                //WriteLog("Mobile No " + pr_mobile + "");
                string policy_name = dt_policy_due.Rows[count - 1]["client_name"].ToString();

                string prem_due_date = dt_policy_due.Rows[count - 1]["PREM_DUE_DATE"].ToString();


                string sms_body_policy = @"সন্মানিত পলিসি গ্রাহক,পরবর্তী প্রিমিয়াম BDT " + tot_premium + " ,পলিসি নং " + policy_no + " এর জন্য পরিশোধের তারিখ " + prem_due_date + "। আপনি যদি ইতিমধ্যে প্রিমিয়াম প্রদান করে থাকেন তাহলে এই এসএমএস আপনার ক্ষেত্রে প্রযোজ্য নয়। আস্থা লাইফ পরিবারের সাথে থাকার জন্য আপনাকে ধন্যবাদ।";

                //string sms_body_policy = @"Respected Policyholder, next pay date of premium  " + tot_premium + " for policy " + policy_no + " is " + prem_due_date + ". Please ignore if already paid. Thank you for staying with Astha Life Family.  ";
                //WriteLog("sms body created");
                SendSMS(policy_mobile, sms_body_policy);
                string sql_policy_due_update = "UPDATE tbl_ilb_sms_policy SET prem_due_date_flag  = 1 WHERE policy_no='" + policy_no + "'";
                ExecuteScalar(sql_policy_due_update);
                //AppConn.Open();
                //OracleCommand cmd_policy_due = new OracleCommand(sql_policy_due_update, AppConn);
                //cmd_policy_due.ExecuteNonQuery();
                //AppConn.Close();
                WriteLog("Policy duedate Status Updated");
                WriteLog("Policy due date SMS sent successfully");
                count = count - 1;
            }
        }
        public DataTable GetDataTableByCommand(string strSQL)
        {
            OpenAppConnection();

            try
            {
                SqlCommand cmd = new System.Data.SqlClient.SqlCommand(strSQL, AppConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 300;// 5 mins
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                AppConn.Close();
            }
        }

        public object ExecuteScalar(string strSQL)
        {
            OpenAppConnection();

            try
            {
                if (Trans == null)
                {
                    Cmnd = new SqlCommand(strSQL, AppConn);
                }
                else
                {
                    Cmnd.CommandText = strSQL;
                }

                Cmnd.CommandType = CommandType.Text;

                return Cmnd.ExecuteScalar();
            }
            catch (SqlException Ex)
            {
                if (Trans != null)
                {
                    Trans.Rollback();
                    Trans = null;
                }
                throw Ex;

            }
            catch (Exception Ex)
            {
                if (Trans != null)
                {
                    Trans.Rollback();
                    Trans = null;
                }
                throw Ex;

            }
            finally
            {
                if (Trans == null)
                {
                    CloseAppConnection();
                }

            }
        }

        //public object ExecuteScalar(string strSQL)
        //{
        //    OpenAppConnection();

        //    try
        //    {
        //        WriteLog(" Execute Scalar started");
        //        if (Trans == null)
        //        {
        //            Cmnd = new OracleCommand(strSQL, AppConn);
        //            WriteLog(" command created");
        //        }
        //        else
        //        {
        //            Cmnd.CommandText = strSQL;
        //            WriteLog("transaction not null");
        //        }

        //        Cmnd.CommandType = CommandType.Text;
        //        WriteLog(" command type defined");
        //        return Cmnd.ExecuteScalar();

        //    }
        //    catch (OracleException Ex)
        //    {
        //        if (Trans != null)
        //        {
        //            Trans.Rollback();
        //            Trans = null;
        //        }
        //        throw Ex;

        //    }

        //    finally
        //    {
        //        if (Trans == null)
        //        {
        //            CloseAppConnection();
        //            WriteLog(" database connection closed");
        //        }

        //    }
        //}
        private void OpenAppConnection()

        {
            string ConnectionString = ConfigurationManager.AppSettings["Connection"];
            if (!ConnectionString.Equals(""))
            {
                if (AppConn.State != ConnectionState.Open)
                {
                    AppConn.Open();
                    WriteLog(" database connection open");
                }
            }
        }
        private void CloseAppConnection()
        {
            if (AppConn.State == ConnectionState.Open)
            {
                AppConn.Close();
                WriteLog(" database connection close");
            }
        }
        protected void WriteLog(string Msg)
        {
            FileStream fs = new FileStream(LogPath + "\\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
        // for boomcast
        public void SendSMS(string sms_to, string sms_body)
        {
            var masking = "ASTHALIFE";
            var userName = "AsthaLifeInsurance";
            var password = "02e4fd431dd1399bc07006fb0c6ac25f";
            WriteLog("userid and password accepted");
            var MsgType = "TEXT";
            var receiver = sms_to;
            var message = sms_body;
            var client = new RestClient("http://api.boom-cast.com/boomcast/WebFramework/boomCastWebService/externalApiSendTextMessage.php?masking=" + masking + "&userName=" + userName + "&password=" + password + "&MsgType=" + MsgType + "&receiver=" + receiver + "&message=" + message + "");
            WriteLog("called api");
            var request = new RestRequest(Method.POST);
            WriteLog("posted api");
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            WriteLog("sent sms");
        }
        protected override void OnStop()
        {

            WriteLog("service stopped");
        }

}

public class Authenticate
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string token_type { get; set; }
    public string access_token_expire_in { get; set; }
    public string refresh_token_expire_in { get; set; }
}
public class Rootobject
{
    public string status { get; set; }
    public Datum[] data { get; set; }
    public string message { get; set; }
}
public class Datum
{
    public int id { get; set; }
    public string employee_name { get; set; }
    public int employee_salary { get; set; }
    public int employee_age { get; set; }
    public string profile_image { get; set; }
}
public class Status
    {
        public Boolean status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }
}
