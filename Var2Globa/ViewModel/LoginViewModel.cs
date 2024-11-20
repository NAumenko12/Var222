using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Var2Globa.View;

namespace Var2Globa.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _loginText;
        private string _password;
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Var22; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";

        public LoginViewModel()
        {
            AuthorizeCommand = new RelayCommand(Authorize);
        }

        public string LoginText
        {
            get => _loginText;
            set
            {
                _loginText = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand AuthorizeCommand { get; }

        private void Authorize(object parameter)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        p.ID_Сотрудника, 
                        p.Логин, 
                        p.Паролб, 
                        p.ФИО, 
                        r.Название
                    FROM 
                        Сотрудник p
                    INNER JOIN 
                        Должность r ON p.Должность = r.ID_должности
                    WHERE 
                        p.Логин = @Логин AND p.Паролб = @Пароль";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Логин", LoginText);
                    command.Parameters.AddWithValue("@Пароль", Password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idПользователя = reader.GetInt32(0);
                            string логин = reader.GetString(1);
                            string пароль = reader.GetString(2);
                            string фио = reader.GetString(3);
                            string названиеРоли = reader.GetString(4);

                            // Устанавливаем CurrentUserId в UserService
                            

                            // Вывод информации о пользователе и его роли
                            MessageBox.Show($"Добро пожаловать, {фио}!\n Должность: {названиеРоли}", "Успешная авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Переход в магазин с передачей информации о пользователе
                            NavigateToOrder();
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void NavigateToOrder()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var shopWindow = new OrderWindow();
                mainWindow.MainContent.Content = shopWindow;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
