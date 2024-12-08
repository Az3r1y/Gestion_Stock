using System.ComponentModel;

public class Client : INotifyPropertyChanged
{
    private string _nom;
    private string _prenom;
    private string _adresse;

    public int Id { get; set; }

    public string Nom
    {
        get => _nom;
        set
        {
            if (_nom != value)
            {
                _nom = value;
                OnPropertyChanged(nameof(Nom)); // Notifie les changements
            }
        }
    }

    public string Prenom
    {
        get => _prenom;
        set
        {
            if (_prenom != value)
            {
                _prenom = value;
                OnPropertyChanged(nameof(Prenom));
            }
        }
    }

    public string Adresse
    {
        get => _adresse;
        set
        {
            if (_adresse != value)
            {
                _adresse = value;
                OnPropertyChanged(nameof(Adresse));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
