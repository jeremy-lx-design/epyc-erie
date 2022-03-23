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

namespace epyc_erie
{
    public sealed partial class MainPage : Page
    {

        List<item> cart = new List<item>();
        public double credit = 100.00;

        public MainPage()
        {
            this.InitializeComponent();
            lvProduits.ItemsSource = cart;
            //format credit to currency
            creditTag.Text = String.Format(new CultureInfo("en-US"), "{0:C}", credit);
            //désactiver le bouton supprimer
            btDelete.IsEnabled = false;

            //mettre à 0 le montant
            montant.Text = String.Format(new CultureInfo("en-US"), "{0:C}", 0);
        }
        //Bouton ajouté produit
        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            //Vérifier validité du nom du produit
            bool valNom = tbNom.Text.Trim() != "";

            //Vérifier validité de la qte
            bool valQte;
            try {   valQte = !tbQte.Text.Contains("_") && int.Parse(tbQte.Text.Trim()) <= 20 && int.Parse(tbQte.Text.Trim()) > 0;} 
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

                //mettre à jour le montant total
                double total = 0.0;
                foreach (item i in cart)
                {
                    total += i.Qte * i.Prix;
                }
                montant.Text = String.Format(new CultureInfo("en-CA"), "{0:C}", total);

                //mettre le montant en rouge si le montant total est supérieur au crédit
                if (total > credit)
                {
                    montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                else
                {
                    if (Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        montant.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                    else
                    {
                        montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                    }
                }

                //réinitialiser les champs
                tbNom.Text = "";
                tbQte.Text = "";
                tbPrix.Text = "";
                
            }
        }
        //Bouton ajouter crédit
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //afficher le dialog ajoutercredit
            ContentDialog dialog = new ajoutercredit(this);
            await dialog.ShowAsync();

            //mettre à jour le credit
            creditTag.Text = String.Format(new CultureInfo("en-CA"), "{0:C}", credit);

            //changer la couleur du montant en fonction du crédit
            if (credit > 0)
            {
                if (Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    montant.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                }
                else
                {
                    montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                }
            }
            else
            {
                montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
            }
        }
        //Bouton achat
        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            //sum of all items in cart
            double total = 0.0;
            foreach (item i in cart)
            {
                total += i.Prix * i.Qte;
            }

            //verify if credit is enough
            if (total > credit)
            {
                //Affichage d'un message d'erreur
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Pas assez d'argent",
                    Content = "Vous n'avez pas assez d'argent pour payer cette commande",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                //afficher le reste du credit
                creditTag.Text = String.Format(new CultureInfo("en-CA"), "{0:C}", credit - total);
                //vider le panier
                cart.Clear();
                //soustraire le total du credit
                credit -= total;

                //afficher le panier
                lvProduits.ItemsSource = null;
                lvProduits.ItemsSource = cart;

                //mettre à 0 le montant
                montant.Text = String.Format(new CultureInfo("en-CA"), "{0:C}", 0);

                //Afficher un message de confirmation
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Achat réussi",
                    Content = "Votre achat a été effectué avec succès",
                    CloseButtonText = "OK"
                };
            }
        }
        //Bouton supprimer
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            //Vérifier qu'il y a un item sélectionné
            if (lvProduits.SelectedItem != null)
            {
                //Supprimer l'item sélectionné
                cart.Remove((item)lvProduits.SelectedItem);
                lvProduits.ItemsSource = null;
                lvProduits.ItemsSource = cart;
                //mettre à jour le montant total après suppression de l'item sélectionné
                double total = 0.0;
                foreach (item i in cart)
                {
                    total += i.Prix * i.Qte;
                }
                montant.Text = String.Format(new CultureInfo("en-CA"), "{0:C}", total);

                //changer la couleur du montant en fonction du crédit
                if (credit > 0)
                {
                    if (Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        montant.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                    else
                    {
                        montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                    }
                }
                else
                {
                    montant.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
            }
        }

        private void lvProduits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //désactiver le bouton supprimer si aucun item n'est sélectionné
            if (lvProduits.SelectedItem == null)
            {
                btDelete.IsEnabled = false;
            }
            else
            {
                btDelete.IsEnabled = true;
            }
        }
    }
}
