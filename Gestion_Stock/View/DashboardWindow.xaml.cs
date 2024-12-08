using Gestion_Stock.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;

namespace Gestion_Stock.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly AppDbContext _context;

        public DashboardWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            PieChart.Series = new SeriesCollection();

            ShowClients();
        }

        private void ClosePieChart()
        {
            PieChart.Visibility = Visibility.Collapsed;
            ShowClients();
        }

        private void ShowClients()
        {
            var clients = _context.Clients
                                  .Select(client => new Client
                                  {
                                      Id = client.Id,
                                      Nom = client.Nom,
                                      Prenom = client.Prenom,
                                      Adresse = client.Adresse
                                  }).ToList(); 

            ClientDataGrid.ItemsSource = clients; 
            SetVisibility(ClientDataGrid); 
        }

        private void ShowProduits()
        {
            List<Produit> produits = _context.Produits.Include(p => p.Categorie).ToList();
            ProduitDataGrid.ItemsSource = produits;
            SetVisibility(ProduitDataGrid);
        }

        private void ShowCategories()
        {
            List<Categorie> categories = _context.Categories.ToList();
            CategorieDataGrid.ItemsSource = categories;
            SetVisibility(CategorieDataGrid);
        }

        private void ShowOrders()
        {
            List<Order> orders = _context.Orders.Include(o => o.Client).Include(o => o.Product).ToList();
            OrderDataGrid.ItemsSource = orders;
            SetVisibility(OrderDataGrid);
        }

        private void ShowPieChart()
        {
            var produits = _context.Produits.Include(p => p.Categorie).ToList();

            PieChart.Series.Clear();
            foreach (var produit in produits)
            {
                PieChart.Series.Add(new PieSeries
                {
                    Title = produit.Nom,
                    Values = new ChartValues<decimal> { produit.QuantiteProduct },
                    DataLabels = true
                });
            }

            SetVisibility(PieChart);
            ClosePieChartButton.Visibility = Visibility.Visible; 
        }

        private void ClosePieChartButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePieChart();
            ClosePieChartButton.Visibility = Visibility.Collapsed; 
        }

        private void SetVisibility(UIElement visibleElement)
        {
            // Cache tout d'abord tous les éléments
            ClientDataGrid.Visibility = Visibility.Collapsed;
            ProduitDataGrid.Visibility = Visibility.Collapsed;
            CategorieDataGrid.Visibility = Visibility.Collapsed;
            OrderDataGrid.Visibility = Visibility.Collapsed;
            PieChart.Visibility = Visibility.Collapsed;
            visibleElement.Visibility = Visibility.Visible;
        }
        
        private void ShowClientsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowClients();
        }

        private void ShowProduitsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowProduits();
        }

        private void ShowCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCategories();
        }

        private void ShowOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOrders();
        }

        private void ShowPieChartButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPieChart();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientDataGrid.Visibility == Visibility.Visible)
            {
                var newClient = new Client
                {
                    Nom = "Nouveau",
                    Prenom = "Client",
                    Adresse = "Adresse"
                };

                _context.Clients.Add(newClient);
                _context.SaveChanges();
                ShowClients();
            }
            else if (ProduitDataGrid.Visibility == Visibility.Visible)
            {
                var categorie = _context.Categories.FirstOrDefault();
                if (categorie == null)
                {
                    MessageBox.Show("Aucune catégorie disponible pour le produit.");
                    return;
                }

                var newProduit = new Produit
                {
                    Nom = "Produit Test",
                    QuantiteProduct = 10,
                    Prix = 100,
                    CategorieId = categorie.Id 
                };

                _context.Produits.Add(newProduit);
                _context.SaveChanges();
                ShowProduits();
            }
            else if (CategorieDataGrid.Visibility == Visibility.Visible)
            {
                var newCategorie = new Categorie
                {
                    Nom = "Nouvelle Catégorie"
                };

                _context.Categories.Add(newCategorie);
                _context.SaveChanges();
                ShowCategories();
            }
            else if (OrderDataGrid.Visibility == Visibility.Visible)
            {
                var firstClient = _context.Clients.FirstOrDefault();
                var firstProduct = _context.Produits.FirstOrDefault();

                if (firstClient == null || firstProduct == null)
                {
                    MessageBox.Show("Impossible d'ajouter une commande : aucun client ou produit valide.");
                    return;
                }

                var newOrder = new Order
                {
                    QuantiteOrder = 1,
                    DateCommande = DateTime.Now,
                    Status = "En attente",
                    ClientId = firstClient.Id,
                    ProductId = firstProduct.Id
                };

                try
                {
                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();
                    ShowOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde de la commande : {ex.Message}");
                }
            }
        }

        private void ClientDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is Client client)
            {
                if (!ValidateRow(client))
                {
                    e.Cancel = true; 
                    return;
                }

                try
                {
                    if (client.Id == 0)
                    {
                        _context.Clients.Add(client);
                    }
                    else
                    {
                        _context.Clients.Update(client); 
                    }
                    _context.SaveChanges(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool changesSaved = false;
            foreach (var item in ClientDataGrid.Items)
            {
                if (item is Client client)
                {
                    var entry = _context.Entry(client);

                    if (entry.State == EntityState.Modified)
                    {
                        _context.Clients.Update(client);
                        changesSaved = true;
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        _context.Clients.Add(client);
                        changesSaved = true;
                    }
                }
            }
            if (changesSaved)
            {
                try
                {
                    _context.SaveChanges();
                    MessageBox.Show("Modifications sauvegardées avec succès.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Aucune modification à sauvegarder.");
            }
        }

        private void ProduitDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is Produit produit)
            {
                if (!ValidateRow(produit))
                {
                    e.Cancel = true; 
                    return;
                }

                try
                {
                    if (produit.Id == 0)
                    {
                        _context.Produits.Add(produit); 
                    }
                    else
                    {
                        _context.Produits.Update(produit); 
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CategorieDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is Categorie categorie)
            {
                if (!ValidateRow(categorie))
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    if (categorie.Id == 0)
                    {
                        _context.Categories.Add(categorie);
                    }
                    else
                    {
                        _context.Categories.Update(categorie);
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OrderDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is Order order)
            {
                if (!ValidateRow(order))
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    if (order.Id == 0)
                    {
                        _context.Orders.Add(order);
                    }
                    else
                    {
                        _context.Orders.Update(order);
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateRow<T>(T row)
        {
            if (row == null)
            {
                MessageBox.Show("Aucune ligne sélectionnée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(row);

                // Vérifie les types simples (string, int, decimal, etc.)
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    MessageBox.Show($"Le champ '{property.Name}' est requis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientDataGrid.Visibility == Visibility.Visible && ClientDataGrid.SelectedItem is Client selectedClient)
            {
                _context.Clients.Remove(selectedClient);
                _context.SaveChanges();
                ShowClients();
            }
            else if (ProduitDataGrid.Visibility == Visibility.Visible && ProduitDataGrid.SelectedItem is Produit selectedProduit)
            {
                _context.Produits.Remove(selectedProduit);
                _context.SaveChanges();
                ShowProduits();
            }
            else if (CategorieDataGrid.Visibility == Visibility.Visible && CategorieDataGrid.SelectedItem is Categorie selectedCategorie)
            {
                _context.Categories.Remove(selectedCategorie);
                _context.SaveChanges();
                ShowCategories();
            }
            else if (OrderDataGrid.Visibility == Visibility.Visible && OrderDataGrid.SelectedItem is Order selectedOrder)
            {
                _context.Orders.Remove(selectedOrder);
                _context.SaveChanges();
                ShowOrders();
            }
        }

        #region Exportation

        private object GetDataForExport()
        {
            if (ClientDataGrid.Visibility == Visibility.Visible)
            {
                return ClientDataGrid.ItemsSource.Cast<Client>().ToList();
            }
            else if (ProduitDataGrid.Visibility == Visibility.Visible)
            {
                return ProduitDataGrid.ItemsSource.Cast<Produit>().ToList();
            }
            else if (CategorieDataGrid.Visibility == Visibility.Visible)
            {
                return CategorieDataGrid.ItemsSource.Cast<Categorie>().ToList();
            }
            else if (OrderDataGrid.Visibility == Visibility.Visible)
            {
                return OrderDataGrid.ItemsSource.Cast<Order>().ToList();
            }

            return null; // Aucun DataGrid actif
        }

        private void ExportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var dataToExport = GetDataForExport();
            if (dataToExport != null)
            {
                var csv = ConvertToCsv(dataToExport);
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = "export.csv"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, csv, Encoding.UTF8);
                    MessageBox.Show("Exportation CSV réussie", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Aucune donnée à exporter", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ConvertToCsv(object data)
        {
            var stringBuilder = new StringBuilder();

            // Vérifiez si les données sont une liste d'objets
            if (data is IEnumerable<object> items)
            {
                var firstItem = items.FirstOrDefault();
                if (firstItem != null)
                {
                    // Récupérer les propriétés de l'objet
                    var properties = firstItem.GetType().GetProperties();

                    // Ajouter les en-têtes CSV (noms des propriétés)
                    stringBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

                    // Ajouter les lignes de données
                    foreach (var item in items)
                    {
                        stringBuilder.AppendLine(string.Join(",", properties.Select(p => p.GetValue(item)?.ToString())));
                    }
                }
                else
                {
                    MessageBox.Show("Aucun élément trouvé dans les données à exporter.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Les données à exporter ne sont pas valides.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return stringBuilder.ToString();
        }


        private void ExportJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var dataToExport = GetDataForExport();
            if (dataToExport != null)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(dataToExport, Formatting.Indented);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "JSON files (*.json)|*.json",
                        FileName = "export.json"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, json);
                        MessageBox.Show("Exportation JSON réussie", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (JsonSerializationException ex)
                {
                    MessageBox.Show($"Erreur lors de la sérialisation JSON: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Aucune donnée à exporter", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
