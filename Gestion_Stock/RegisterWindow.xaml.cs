using Gestion_Stock.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Gestion_Stock
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new AppDbContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Ce nom d'utilisateur est déjà utilisé", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

     
                string passwordHash = HashPassword(password);

      
                var users = new User
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    Role = "admin" 
                };

                context.Users.Add(users);
                context.SaveChanges();

                MessageBox.Show("Compte créé avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

          
                this.Close();
            }
        }

        private string HashPassword(string password)
        {
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

    }
}
