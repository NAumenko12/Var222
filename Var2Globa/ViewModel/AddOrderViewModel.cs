using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Var2Globa.Model;
using Var2Globa.View;

namespace Var2Globa.ViewModel
{
    internal class AddOrderViewModel
    {
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Var22; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";

        private string _адресПомещения;
        private string _имяЗаявителя;
        private string _телефонЗаявителя;
        private Тип_помещения _выбранныйТипПомещения;
        private int _площадьПомещения;
        private Тип_уборки _выбранныйТипУборки;
        private Доп_услуги _выбраннаяДопУслуга;

        public string АдресПомещения
        {
            get => _адресПомещения;
            set
            {
                _адресПомещения = value;
                OnPropertyChanged(nameof(АдресПомещения));
            }
        }

        public string ИмяЗаявителя
        {
            get => _имяЗаявителя;
            set
            {
                _имяЗаявителя = value;
                OnPropertyChanged(nameof(ИмяЗаявителя));
            }
        }

        public string ТелефонЗаявителя
        {
            get => _телефонЗаявителя;
            set
            {
                _телефонЗаявителя = value;
                OnPropertyChanged(nameof(ТелефонЗаявителя));
            }
        }

        public Тип_помещения ВыбранныйТипПомещения
        {
            get => _выбранныйТипПомещения;
            set
            {
                _выбранныйТипПомещения = value;
                OnPropertyChanged(nameof(ВыбранныйТипПомещения));
            }
        }

        public int ПлощадьПомещения
        {
            get => _площадьПомещения;
            set
            {
                _площадьПомещения = value;
                OnPropertyChanged(nameof(ПлощадьПомещения));
            }
        }

        public Тип_уборки ВыбранныйТипУборки
        {
            get => _выбранныйТипУборки;
            set
            {
                _выбранныйТипУборки = value;
                OnPropertyChanged(nameof(ВыбранныйТипУборки));
            }
        }

        public Доп_услуги ВыбраннаяДопУслуга
        {
            get => _выбраннаяДопУслуга;
            set
            {
                _выбраннаяДопУслуга = value;
                OnPropertyChanged(nameof(ВыбраннаяДопУслуга));
            }
        }

        public ObservableCollection<Тип_помещения> ТипыПомещений { get; set; }
        public ObservableCollection<Тип_уборки> ТипыУборок { get; set; }
        public ObservableCollection<Доп_услуги> ДополнительныеУслуги { get; set; }

        public ICommand ДобавитьЗаявкуCommand { get; set; }

        public AddOrderViewModel()
        {
            // Загрузка данных из базы данных
            LoadData();

            // Инициализация команд
            ДобавитьЗаявкуCommand = new RelayCommand(ДобавитьЗаявку);
        }

        private void LoadData()
        {
            // Загрузка типов помещений, типов уборок и дополнительных услуг из базы данных
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Загрузка типов помещений
                string queryТипыПомещений = "SELECT ID_тип_помещения, НАзвание FROM Тип_помещения";
                using (SqlCommand command = new SqlCommand(queryТипыПомещений, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ТипыПомещений = new ObservableCollection<Тип_помещения>();
                        while (reader.Read())
                        {
                            ТипыПомещений.Add(new Тип_помещения
                            {
                                ID_тип_помещения = reader.GetInt32(0),
                                Название = reader.GetString(1)
                            });
                        }
                    }
                }

                // Загрузка типов уборок
                string queryТипыУборок = "SELECT ID_тип, Название FROM Тип_уборки";
                using (SqlCommand command = new SqlCommand(queryТипыУборок, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ТипыУборок = new ObservableCollection<Тип_уборки>();
                        while (reader.Read())
                        {
                            ТипыУборок.Add(new Тип_уборки
                            {
                                ID_тип = reader.GetInt32(0),
                                Название = reader.GetString(1)
                            });
                        }
                    }
                }

                // Загрузка дополнительных услуг
                string queryДопУслуги = "SELECT ID_доп_услуги, Название, Стоимость FROM Доп_услуги";
                using (SqlCommand command = new SqlCommand(queryДопУслуги, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ДополнительныеУслуги = new ObservableCollection<Доп_услуги>();
                        while (reader.Read())
                        {
                            ДополнительныеУслуги.Add(new Доп_услуги
                            {
                                ID_доп_услуги = reader.GetInt32(0),
                                Название = reader.GetString(1),
                                Стоимость = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
        }

        private void ДобавитьЗаявку(object parametr)
        {
            // Логика добавления новой заявки в базу данных
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Добавление клиента
                string queryДобавитьКлиента = @"
            INSERT INTO Клиент (имя, Телефон)
            VALUES (@ИмяЗаявителя, @ТелефонЗаявителя);
            SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(queryДобавитьКлиента, connection))
                {
                    command.Parameters.AddWithValue("@ИмяЗаявителя", ИмяЗаявителя);
                    command.Parameters.AddWithValue("@ТелефонЗаявителя", ТелефонЗаявителя);
                    int idКлиента = Convert.ToInt32(command.ExecuteScalar());

                    // Добавление помещения
                    string queryДобавитьПомещение = @"
                INSERT INTO Помещение (адрес, Площадь, Тип_помещения)
                VALUES (@АдресПомещения, @ПлощадьПомещения, @ТипПомещения);
                SELECT SCOPE_IDENTITY();";
                    using (SqlCommand commandПомещение = new SqlCommand(queryДобавитьПомещение, connection))
                    {
                        commandПомещение.Parameters.AddWithValue("@АдресПомещения", АдресПомещения);
                        commandПомещение.Parameters.AddWithValue("@ПлощадьПомещения", ПлощадьПомещения);
                        commandПомещение.Parameters.AddWithValue("@ТипПомещения", ВыбранныйТипПомещения.ID_тип_помещения);
                        int idПомещения = Convert.ToInt32(commandПомещение.ExecuteScalar());

                        // Добавление заявки
                        string queryДобавитьЗаявку = @"
                    INSERT INTO Заказ (Клиент, Тип_уборки, Дата_исполнения, Помещение, Статус_заказа)
                    VALUES (@Клиент, @ТипУборки, @ДатаИсполнения, @Помещение, @СтатусЗаказа);
                    SELECT SCOPE_IDENTITY();";
                        using (SqlCommand commandЗаявку = new SqlCommand(queryДобавитьЗаявку, connection))
                        {
                            commandЗаявку.Parameters.AddWithValue("@Клиент", idКлиента);
                            commandЗаявку.Parameters.AddWithValue("@ТипУборки", ВыбранныйТипУборки.ID_тип);
                            commandЗаявку.Parameters.AddWithValue("@ДатаИсполнения", DateTime.Now);
                            commandЗаявку.Parameters.AddWithValue("@Помещение", idПомещения);
                            commandЗаявку.Parameters.AddWithValue("@СтатусЗаказа", "Новый");
                            int idЗаявки = Convert.ToInt32(commandЗаявку.ExecuteScalar());

                            // Добавление дополнительных услуг
                            if (ВыбраннаяДопУслуга != null)
                            {
                                string queryДобавитьДопУслуги = @"
                            INSERT INTO Состав_доп_услуги (ID_заказа, ID_доп_услуги)
                            VALUES (@IDЗаявки, @IDДопУслуги);";
                                using (SqlCommand commandДопУслуги = new SqlCommand(queryДобавитьДопУслуги, connection))
                                {
                                    commandДопУслуги.Parameters.AddWithValue("@IDЗаявки", idЗаявки);
                                    commandДопУслуги.Parameters.AddWithValue("@IDДопУслуги", ВыбраннаяДопУслуга.ID_доп_услуги);
                                    commandДопУслуги.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var shopWindow = new OrderWindow();
                mainWindow.MainContent.Content = shopWindow;
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

