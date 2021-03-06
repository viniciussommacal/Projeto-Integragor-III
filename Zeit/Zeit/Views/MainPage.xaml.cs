﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using Acr.UserDialogs;

namespace Zeit
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        string _Cpf;
        public MainPage(string Cpf)
        {
            InitializeComponent();
            _Cpf = Cpf;
            this.Title = "ZEIT";
            IsLoading = false;
            BindingContext = this;
        }
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {                
                try
                {
                IsLoading = true;
                await Task.Delay(300);
                //RELATÓRIO DAS ULTIMAS 5 ENTRADAS
                EntradaDAO entrada = new EntradaDAO();  
                if(entrada.GetLastFive().Count > 0)
                {
                    ltvEntradas.ItemsSource = entrada.GetLastFive();
                }

                //RELATÓRIO DAS ULTIMAS 5 SAIDAS
                RetiradaDAO retirada = new RetiradaDAO();
                if (retirada.GetLastFive().Count > 0)
                {
                    ltvSaidas.ItemsSource = retirada.GetLastFive();
                }


                ProdutoDAO produtos = new ProdutoDAO();
                //RELATÓRIO MICROCHARTS TOPS PRODUTO
                if (produtos.Relatorio1().Count() > 0)
                {
                    TopProduto.Chart = new Microcharts.RadialGaugeChart { Entries = produtos.Relatorio1().OrderBy(x=> x.ValueLabel).ToList() };
                    TopProduto.Chart.LabelTextSize = 25;
                    TopProduto.HeightRequest = 200;
                }
                //RELATÓRIO MICROCHARTS MIN PRODUTO
                if (produtos.Relatorio2().Count() > 0)
                {
                    MinProduto.Chart = new Microcharts.BarChart { Entries = produtos.Relatorio2().OrderBy(x => x.ValueLabel).ToList() };
                    MinProduto.Chart.LabelTextSize = 25;
                    MinProduto.HeightRequest = 200;
                }

                //FRAME CONTADORES  
                produtosCadastrados.Text = $"Totas de itens cadastrados: {Convert.ToString(produtos.listaProduto().Count)}";
                totalItens.Text = $"Total de itens em estoque: {Convert.ToString(produtos.listaProduto().Sum(x => x.quantidade))}";
                totalEntradas.Text = $"Total de Entradas: {Convert.ToString(entrada.GetAll().ToList().Count)}";
                totalRetiradas.Text = $"Total de retiradas: {Convert.ToString(retirada.GetAll().Count)}";
                isLoading = false;
                

            }catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro de banco de dados: " + ex.Message, "Ok");
            }
            finally
            {
                isLoading = false;
            }
        }   
        private async void btnDepartamento_Clicked(object sender, EventArgs e)
        {
            IsLoading = true;
            await Task.Delay(300);
            await Navigation.PushAsync(new ListaDepartamento());
            IsLoading = false;
        }
        private async void btnFornecedor_Clicked(object sender, EventArgs e)
        {
            IsLoading = true;
            await Task.Delay(300);
            await Navigation.PushAsync(new ListaFornecedor());
            IsLoading = false;
        }
        private async void btnPesquisa_Clicked(object sender, EventArgs e)
        {
            IsLoading = true;
            await Task.Delay(300);
            await Navigation.PushAsync(new Adicionar_Retirar(_Cpf));           
            IsLoading = false;
        }

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


    }
}
