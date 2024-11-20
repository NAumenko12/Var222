﻿using System;
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
    public class OrderViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Заказ> _заявки;
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Var22; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";

        public ObservableCollection<Заказ> Заявки
        {
            get => _заявки;
            set
            {
                _заявки = value;
                OnPropertyChanged(nameof(Заявки));
            }
        }

        public ICommand ShowStatisticsCommand { get; set; }
        public ICommand AddOrderCommand { get; set; }
        public ICommand GoHomeNavigateCommand { get; set; }

        public OrderViewModel()
        {
            // Загрузка данных из базы данных
            LoadData();

            // Инициализация команд
            ShowStatisticsCommand = new RelayCommand(ShowStatistics);
            AddOrderCommand = new RelayCommand(AddOrder);
            GoHomeNavigateCommand = new RelayCommand(GoHome);
        }

        private void LoadData()
        {
            // Загрузка заявок из базы данных
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        z.ID_заказа, 
                        k.имя, 
                        k.Телефон, 
                        p.адрес, 
                        t.Название, 
                        z.Дата_исполнения, 
                        z.Статус_заказа,
                        z.Комментарий
                    FROM 
                        Заказ z
                    JOIN 
                        Клиент k ON z.Клиент = k.ID_клиента
                    JOIN 
                        Помещение p ON z.Помещение = p.ID_помещение
                    JOIN 
                        Тип_уборки t ON z.Тип_уборки = t.ID_тип";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ObservableCollection<Заказ> заявки = new ObservableCollection<Заказ>();

                        while (reader.Read())
                        {
                            Заказ заказ = new Заказ
                            {
                                ID_заказа = reader.GetInt32(0),
                                Клиент1 = new Клиент
                                {
                                    имя = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    Телефон = reader.IsDBNull(2) ? null : reader.GetString(2)
                                },
                                Помещение1 = new Помещение
                                {
                                    адрес = reader.IsDBNull(3) ? null : reader.GetString(3)
                                },
                                Тип_уборки1 = new Тип_уборки
                                {
                                    Название = reader.IsDBNull(4) ? null : reader.GetString(4)
                                },
                                Дата_исполнения = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                Статус_заказа = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Комментарий = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };

                            заявки.Add(заказ);
                        }

                        Заявки = заявки;
                    }
                }
            }
        }

        private void ShowStatistics(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var shopWindow = new StatisticWindow();
                mainWindow.MainContent.Content = shopWindow;
            }
        }

        private void AddOrder(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var shopWindow = new AddOrder();
                mainWindow.MainContent.Content = shopWindow;
            }
        }

        private void GoHome(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var shopWindow = new LoginWindow();
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