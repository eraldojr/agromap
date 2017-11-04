using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
using AgroMap.Views;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionScreen : ContentPage
    {
        private CancellationTokenSource cts;
        private Boolean isRefreshing = false;
        private MainScreenMaster __menu;

        public InspectionScreen(MainScreenMaster menu)
        {
            InitializeComponent();
            InitComponents();
            __menu = menu;
            //LoadInspections();

        }

        protected override void OnAppearing()
        {
            LoadInspections();
        }

        private void InitComponents()
        {
            lbl_inspections.Text = Strings.AvailableInspections;
            //btn_cancel_sync.Text = Strings.CancelSync;
            btn_create.Text = Strings.NewInspection;

            if (UserService.LoadUserSession().Level == 0)
                ShowSuperOptions();
            //ListView
            list_view_inspections.ItemTapped += ListView_Tapped;
            list_view_inspections.HasUnevenRows = true;
            list_view_inspections.ItemTemplate = new DataTemplate(() => { return new InspectionCell(this); });            
            Command refreshCommand = new Command(() => LoadInspections());
            list_view_inspections.RefreshCommand = refreshCommand;
            lbl_syncing.Text = Strings.Syncing;
        }

        // Exibe as opções para usuários que são supervisores
        // Por enquanto, exibe apenas o botão para criar inspeções
        private void ShowSuperOptions()
        {
            btn_create.Text = Strings.NewInspection;
            super_options.IsVisible = true;
        }

        // Carrega inspeções do server e exibe na list_view
        // Se não houver conexão, exibe dados do armazenamento local
        private async void LoadInspections()
        {
            if (isRefreshing)
                return;
            isRefreshing = true;
            ShowAnimation();
            
            List<Inspection> list = await InspectionDAO.GetAll();
            list_view_inspections.ItemsSource = list;
            list_view_inspections.IsRefreshing = false;
            
            if (await InspectionService.SyncWithServer())
            {
                List<Inspection> __inspections = await InspectionDAO.GetAll();
                list_view_inspections.ItemsSource = __inspections;
            }
            //list_view_inspections.IsRefreshing = false;
            isRefreshing = false;
            HideAnimation();
            __menu.UpdateSyncLabel();
        }

        // Quando toca um item da list_view, vai para seus eventos
        private async void ListView_Tapped(object sender, ItemTappedEventArgs e)
        {
            Inspection i = e.Item as Inspection;
            await Navigation.PushAsync(new EventTabScreen(i));
        }

        // Exibe tela para criar nova inspeção
        private async void ShowNewInspectScreen()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }
            await Navigation.PushAsync(new NewInspectionScreen());
        }

        // Evento de clique do botão nova inspeção
        // Se não houver conexão, nega a ação
        private async void Btn_New_Inspection_Click(object sender, EventArgs e)
        {
            //var uuid = EventDAO.GetNewID();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    ShowNewInspectScreen();
                }
                else
                {
                    await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionScreen|Btn_New_Inspection(): " + err.Message);
            }
        }

        // Quando a opção Edit, do menu de contexto da list_view é clicada
        // Captura a inspeção selecionada e envia para a tela de edição
        // Se não houver conexão, nega a ação
        public async void ListView_Inspection_Edit(Inspection inspection)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }

            try
            {
                int logged_id = UserService.GetLoggedUserId();
                if (inspection.supervisor == logged_id)
                {
                    await Navigation.PushAsync(new NewInspectionScreen(inspection));
                }
                else
                {
                    await DisplayAlert(Strings.Unauthorized, Strings.MustBeSupervisor, Strings.OK);
                    return;
                }

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionScreen|ListView_Inspection_Delete:: " + err.Message);
            }
        }

        // Quando a opção Edit, do menu de contexto da list_view é clicada
        // Captura a inspeção selecionada e exclui
        // Se não houver conexão, nega a ação
        public async void ListView_Inspection_Delete(Inspection inspection)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }
            int logged_id = UserService.GetLoggedUserId();
            if (inspection.supervisor != logged_id)
            {
                await DisplayAlert(Strings.Unauthorized, Strings.MustBeSupervisor, Strings.OK);
                UpdateList();
                return;
            }
            if ( await InspectionService.DeleteInspection(inspection))
            {
                await DisplayAlert(Strings.Success, Strings.DeletedInspection, Strings.OK);
                UpdateList();
                return;
            }
            else
            {
                await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
                UpdateList();
                return;
            }
        }

        // Método que chama o evento para atualizar a lista
        private void UpdateList()
        {
            list_view_inspections.BeginRefresh();
        }

        //Esconde animação de carregamento
        private void HideAnimation()
        {
            lbl_syncing.IsVisible = false;
            actInspectionScreen.IsVisible = false;
            actInspectionScreen.IsRunning = false;
        }

        //Exibe animação de carregamento
        private void ShowAnimation()
        {
            lbl_syncing.IsVisible = true;
            actInspectionScreen.IsVisible = true;
            actInspectionScreen.IsRunning = true;
        }

    }
}