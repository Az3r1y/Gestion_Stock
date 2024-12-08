using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Gestion_Stock.Models;
using Gestion_Stock.Views;
using Microsoft.EntityFrameworkCore;


namespace Gestion_Stock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsValidUser(username, password))
            {
                MessageBox.Show("Connexion réussie", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                var dashboardWindow = new DashboardWindow();
                dashboardWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool IsValidUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            using (var context = new AppDbContext())
            {
                var user = context.Users
                    .AsNoTracking() 
                    .FirstOrDefault(u => u.Username == username);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    return true;
                }

                return false;
            }
        }


        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            string enteredPasswordHash = HashPassword(enteredPassword);  
            return enteredPasswordHash == storedPasswordHash;  
        }


        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Le mot de passe ne peut pas être vide.");
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var b in bytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }



        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
        }

    }
}
