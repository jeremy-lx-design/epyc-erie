using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Collections;
using System.Globalization;
using System.Net;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace epyc_erie
{
    /// <summary>
    /// Page unique de l'interface utilisateur.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        List<item> cart = new List<item>();

        public MainPage()
        {
            this.InitializeComponent();
            lvProduits.ItemsSource = cart;
        }


        public static int GetStatus(String url)
        {
            try
            {
                // Créer une requête HTTP
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                // Récupérer le code de status
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                int code = (int)myHttpWebResponse.StatusCode;

                // Fermer la requête http
                myHttpWebResponse.Close();

                // Retourner le code de status
                return code;
            }
            catch (WebException e)
            {
                return 999;
            }
            catch (Exception e)
            {
                return 999;
            }

            
        }

        /// <summary>
        /// Fonction de clic du boutton ajouter
        /// </summary>
        /// <param name="sender">Bouton cliqué</param>
        /// <param name="e">Évènement du clic</param>
        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            //Vérifier validité du nom du produit
            bool valNom = tbNom.Text.Trim() != "";

            //Vérifier validité de la qte
            bool valQte;
            try {   valQte = !tbQte.Text.Contains("_") && int.Parse(tbQte.Text.Trim()) < 20 && int.Parse(tbQte.Text.Trim()) > 0;} 
            catch { valQte = false;}

            //Vérifier validité du prix
            bool valPrix;
            try { valPrix = !tbPrix.Text.Contains("_") && double.Parse(tbPrix.Text.Trim(), CultureInfo.InvariantCulture) > 0.0; }
            catch { valPrix = false;}
            

            //Afficher des messages d'erreur, le cas échéant
            if (valNom) {   errorNom.Visibility = Visibility.Collapsed;} 
            else {          errorNom.Visibility= Visibility.Visible;}

            if (valQte) {   errorQte.Visibility = Visibility.Collapsed;}
            else {          errorQte.Visibility = Visibility.Visible;}

            if (valPrix) {  errorPrix.Visibility = Visibility.Collapsed;}
            else {          errorPrix.Visibility = Visibility.Visible;}

            //Ajouter l'item si tous les champs sont valides
            if(valNom && valPrix && valQte)
            {
                cart.Add(new item(tbNom.Text.Trim(), int.Parse(tbQte.Text.Trim()), double.Parse(tbPrix.Text.Trim(), CultureInfo.InvariantCulture)));
                lvProduits.ItemsSource = null;
                lvProduits.ItemsSource = cart;
            }
        }
    }
}
