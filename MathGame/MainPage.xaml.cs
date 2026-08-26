using System;
using System.Collections.Generic;

namespace MathGame
{
    public partial class MainPage : ContentPage
    {
        int iLB1 = 0, iLB2 = 0, iLB3 = 0;
        float fR = 0.0f;
        int iAcertouCount = 0, iErrouCount = 0;

        List<float> pontos = new List<float>();

        Random rand = new Random();
        private IDispatcherTimer _timer;
        private TimeSpan _timeElapsed = TimeSpan.Zero;

        public MainPage()
        {
            InitializeComponent();
            GerarJogo();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(pkDificuldade.SelectedIndex == -1)
            {
                pkDificuldade.SelectedIndex = 0;
            }
            float fResult = 0.0f;
            try
            {
                fResult = Convert.ToSingle(txR.Text);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("ERRO", "Digite um número válido!", "OK");
                return;
            }

            if(fResult == fR)
            {
                iAcertouCount++;
                lbAcertou.Text = $"Acertos: {iAcertouCount}";
                imR.Source = "win.png";
                if(pkDificuldade.SelectedIndex == 0)
                {
                    lbPontos.Text = $"Pontos: {iAcertouCount * 10 * ((TimeSpan.FromSeconds(40) - _timeElapsed).TotalSeconds * 0.1)}";
                }
                if (pkDificuldade.SelectedIndex == 1)
                {
                    lbPontos.Text = $"Pontos: {iAcertouCount * 50 * ((TimeSpan.FromSeconds(40) - _timeElapsed).TotalSeconds * 0.1)}";
                }
                if (pkDificuldade.SelectedIndex == 2)
                {
                    lbPontos.Text = $"Pontos: {iAcertouCount * 100 * ((TimeSpan.FromSeconds(40) - _timeElapsed).TotalSeconds * 0.1)}";
                }
            }
            else
            {
                iErrouCount++;
                lbErrou.Text = $"Erros: {iErrouCount}";
                imR.Source = "loose.png";
            }

            _timer.Stop();
            _timeElapsed = TimeSpan.Zero;
            pbProgresso.Progress += 0.1;
            await Task.Delay(2000);
            imR.Source = "question.png";
            if(pbProgresso.Progress == 1)
            {
                await DisplayAlertAsync("Fim de Jogo", "Você completou 10 rodadas! Sua pontuação foi de " + lbPontos.Text.Split(':')[1].Trim() + " pontos.", "OK");
                pontos.Add(Convert.ToSingle(lbPontos.Text.Split(':')[1].Trim()));
                lbAcertou.Text = "Acertos: 0";
                lbErrou.Text = "Erros: 0";
                iAcertouCount = 0;
                iErrouCount = 0;
                lbPontos.Text = "Pontos: 0";
                pbProgresso.Progress = 0;
            }
            GerarJogo();
        }

        private async void btPlacar_Clicked(object sender, EventArgs e)
        {
            pontos.Sort();
            pontos.Reverse();
            await DisplayAlertAsync("Placar", "Melhores Pontuações:\n" + string.Join("\n", pontos), "Voltar");
        }

        public void GerarJogo()
        {
            if (pkDificuldade.SelectedIndex == -1)
            {
                iLB1 = rand.Next(1, 11);
                iLB3 = rand.Next(1, 11);
            }
            if(pkDificuldade.SelectedIndex == 0)
            {
                iLB1 = rand.Next(1, 11);
                iLB3 = rand.Next(1, 11);
            }
            if(pkDificuldade.SelectedIndex == 1)
            {
                iLB1 = rand.Next(1, 51);
                iLB3 = rand.Next(1, 51);
            }
            if (pkDificuldade.SelectedIndex == 2)
            {
                iLB1 = rand.Next(1, 101);
                iLB3 = rand.Next(1, 101);
            }
            iLB2 = rand.Next(1, 5);
            lb1.Text = iLB1.ToString();
            lb3.Text = iLB3.ToString();

            switch (iLB2)
            {
                case 1:
                        fR = (iLB1 + iLB3);
                        lb2.Text = "+";
                        break;
                case 2:
                        fR = (iLB1 - iLB3);
                        lb2.Text = "-";
                        break;
                case 3:
                        fR = (iLB1 * iLB3);
                        lb2.Text = "*";
                        break;
                case 4:
                        fR = (Convert.ToSingle(iLB1) / Convert.ToSingle(iLB3));
                        fR = (float)Math.Round(fR, 2);
                        lb2.Text = "/";
                        break;
            }
            txR.Text = "";

            SetupTimer();
        }
        private void SetupTimer()
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            timerBorder.Stroke = Colors.LimeGreen;
            lbTimer.TextColor = Colors.LimeGreen;

            _timer.Start();
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            _timeElapsed = _timeElapsed.Add(TimeSpan.FromSeconds(1));

            lbTimer.Text = (TimeSpan.FromSeconds(30) - _timeElapsed).TotalSeconds.ToString("0");
            if(_timeElapsed.TotalSeconds == 10)
            {
                timerBorder.Stroke = Colors.Orange;
                lbTimer.TextColor = Colors.Orange;
            }
            if(_timeElapsed.TotalSeconds == 20)
            {
                timerBorder.Stroke = Colors.Red;
                lbTimer.TextColor = Colors.Red;
            }
            if(_timeElapsed.TotalSeconds >= 30)
            {
                _timer.Stop();
                await DisplayAlert("Tempo Esgotado", "Você não respondeu a tempo!", "OK");
                iErrouCount++;
                lbErrou.Text = $"Erros: {iErrouCount}";
                imR.Source = "loose.png";
                _timeElapsed = TimeSpan.Zero;
                await Task.Delay(2000);
                imR.Source = "question.png";
                GerarJogo();
            }
        }
    }
}
