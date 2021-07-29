using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace ConnectedLayer
{
    public static class NpgDataConnection
    {
        #region Поточне налаштування підключення до БД

        public static NpgsqlConnection contn = new NpgsqlConnection(@"Server=localhost; Port=5432; User Id=postgres; Password=faratff11; Database=TestDB");

        /// <summary>
        /// Відкрити підключення
        /// </summary>
        public static void OpenConnection()
        {
            if (contn.State == System.Data.ConnectionState.Closed)
                contn.Open();
            else
                return;
        }

        /// <summary>
        /// Закрити підключення
        /// </summary>
        public static void CloseConnection()
        {
            if (contn.State == System.Data.ConnectionState.Open)
                contn.Close();
        }

        /// <summary>
        /// Передати строку підключення
        /// </summary>
        public static NpgsqlConnection GetConnection()
        {
            return contn;
        }

        #endregion

        #region Управління даними БД таблиці "СAR"

        /// <summary>
        ///  Метод загрузки даних в DataGridView із БД
        /// </summary>
        public static List<string[]> GetAll()
        {
            string sql = "SELECT * FROM cars ORDER BY id ASC";
            OpenConnection();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, contn);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (dr.Read())
            {
                data.Add(new string[2]);
                data[data.Count - 1][0] = dr[0].ToString();
                data[data.Count - 1][1] = dr[1].ToString();
            }
            dr.Close();
            CloseConnection();

            return data;
        }


        /// <summary>
        ///  Метод сортування даних в DataGridView 
        /// </summary>
        public static List<string[]> SortData() // int A, int B
        {
            int A = DataBankDll.sort1;
            int B = DataBankDll.sort2;

            string sql = "SELECT * FROM cars where id BETWEEN @A AND @B ORDER BY id ASC";
            OpenConnection();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, contn);

            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "@A";
            param.Value = A;
            param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
            cmd.Parameters.Add(param);

            NpgsqlParameter param2 = new NpgsqlParameter();
            param2.ParameterName = "@B";
            param2.Value = B;
            param2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
            cmd.Parameters.Add(param2);
            
            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<string[]> dataSort = new List<string[]>();

            while (dr.Read())
            {
                dataSort.Add(new string[2]);
                dataSort[dataSort.Count - 1][0] = dr[0].ToString();
                dataSort[dataSort.Count - 1][1] = dr[1].ToString();
            }
            dr.Close();
            CloseConnection();

            return dataSort;
        }

        /// <summary>
        ///  Метод для вставки даних з TextBox в БД
        /// </summary>
        public static void InsertFood(int id, double price)
        {
            string sql = string.Format("INSERT INTO cars VALUES (@id, @price)");
            OpenConnection();

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, contn))
            {
                NpgsqlParameter param = new NpgsqlParameter();
                param.ParameterName = "@id";
                param.Value = id;
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
                cmd.Parameters.Add(param);

                param = new NpgsqlParameter();
                param.ParameterName = "@price";
                param.Value = price;
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Double;
                cmd.Parameters.Add(param);

                cmd.ExecuteReader();
            }
        }


        /// <summary>
        ///  Метод для Редагування даних з TextBox в БД
        /// </summary>
        public static void UpdateFood(int id, double price)
        {
            string sql = string.Format("UPDATE cars SET price= @price  WHERE cars.id = @Id");
            OpenConnection();

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, contn))
            {
                NpgsqlParameter param = new NpgsqlParameter();
                param.ParameterName = "@id";
                param.Value = id;
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
                cmd.Parameters.Add(param);

                param = new NpgsqlParameter();
                param.ParameterName = "@price";
                param.Value = price;
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Double;
                cmd.Parameters.Add(param);

                cmd.ExecuteReader();
            }
        }


        /// <summary>
        ///  Метод для Видалення даних із DataGridView та БД
        /// </summary>
        public static void DeleteFood(int id)
        {
            string sql = string.Format("Delete from cars where id=@IdRows");
            OpenConnection();

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, contn))
            {
                NpgsqlParameter param = new NpgsqlParameter();
                param.ParameterName = "@IdRows";
                param.Value = id;
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
                cmd.Parameters.Add(param);

                cmd.ExecuteReader();
            }

            CloseConnection();
        }

        /// <summary>
        /// Загрузка "Форми редагування" (єдиний випадок із стат. value)
        /// <summary>
        public static void EditLoad()
        {
            string sql = string.Format("SELECT * FROM cars where id= '" + DataBankDll.Text + "'");
            OpenConnection();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, contn);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                DataBankDll.str1 = dr[0].ToString();
                DataBankDll.str2 = dr[1].ToString();
            } dr.Close();

            NpgDataConnection.CloseConnection();
        }

        #endregion
    }

    #region Cтатичні дані, що передадаються між формами

    /// <summary> 
    ///  Клас зберігає дані, що передадаються між формами
    /// </summary> 
    public static class DataBankDll
    {
        public static string Text = ""; // для Форми редагування "Вибірка даних із БД поточної стрічки"
        public static string str1 = ""; // для Форми редагування textbox1
        public static string str2 = ""; // для Форми редагування textbox2
        public static int sort1 = 0;    // для Форми сортування textbox1
        public static int sort2 = 0;    // для Форми сортування textbox2
    }
    #endregion

}
